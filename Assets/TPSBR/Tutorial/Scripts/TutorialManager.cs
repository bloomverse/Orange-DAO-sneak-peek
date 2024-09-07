using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using Fusion;
using Fusion.Photon.Realtime;
using UnityEngine.SceneManagement;

namespace TPSBR
{
    using System;

    public class TutorialManager : ContextBehaviour
    {
        private static TutorialManager m_TutrialManager;
        public static TutorialManager Instance { get { return m_TutrialManager; } }

        [SerializeField] private TutorialElevator m_Evelator;
        [SerializeField] private TutorialDoor m_BoostDoor, m_BossDoor;
        [SerializeField] private AudioSource m_MusicAudioSource, m_SFXAudioSource;
        [SerializeField] private GameObject[] m_Drones;
        [SerializeField] private Transform[] m_Checkpoints;
        [SerializeField] private Animator m_AnnouncerAnim, m_TutorialKeysAnim;
        [SerializeField] private TMP_Text m_AnnouncerText;
        [SerializeField] private AudioClip m_LevelMusic, m_BossMusic, m_DroneExplosion, m_OpenDoor, m_CloseDoor, m_BeginBoostDoors, m_CloseBoostDoors;
        [SerializeField] private AudioClip[] m_AnnouncerClips;
        [SerializeField] private TutorialRobot m_FinalBoss;
        [SerializeField] private Transform m_LeftCover, m_RightCover;
        [SerializeField] private CanvasGroup m_FinalBossHP;
        [SerializeField] private GameObject m_Explosion, m_LargeExplosion, m_LaserTarget, m_ItemBox, m_ZoomDrones;
        [SerializeField] private string[] m_AnnouncerSteps;

        private int m_DronesDestroyed = 0, m_AnnouncerSubtitleIndex, m_AnnouncerAudioIndex = 0, m_CheckpointIndex = 0;
        public int CheckpointIndex { get { return m_CheckpointIndex; } set { m_CheckpointIndex = value; } }

        public bool bossAlive = true;


        private void Awake()
        {
            //PlayerPrefs.DeleteAll();
            /*   if (m_TutrialManager == null)
               {
                   m_TutrialManager = this;
               }
               else
               {
                   Destroy(gameObject);
               }*/
        }

        private void Start()
        {
            m_TutrialManager = this;
            m_ItemBox.SetActive(false);
            //MissionSystem.Instance.m_MissionEntryPrefab.SetActive(true);
            
        }

        public void StartDroneSection()
        {
            m_Drones[1].SetActive(false);
            m_Drones[2].SetActive(false);


        }



        public void EnableBoostDoor()
        {
            m_BoostDoor.EnableDoor();
        }

        public void EnableBossDoor()
        {
            m_BossDoor.EnableDoor();
            m_FinalBoss.gameObject.SetActive(true);
        }

        public void AnnouncerStep(float a_TextDurationSeconds, bool a_ShowSubtitle = true) => StartCoroutine(ShowAnnouncerText(a_TextDurationSeconds, a_ShowSubtitle));

        public IEnumerator ShowAnnouncerText(float a_TextDurationSeconds, bool a_ShowSubtitle = true)
        {
            m_SFXAudioSource.Stop();
            m_SFXAudioSource.PlayOneShot(m_AnnouncerClips[m_AnnouncerAudioIndex], 1);
            m_AnnouncerAudioIndex++;

            if (a_ShowSubtitle)
            {
                m_AnnouncerText.gameObject.SetActive(a_ShowSubtitle);
                m_AnnouncerText.text = "ANNOUNCER: " + m_AnnouncerSteps[m_AnnouncerSubtitleIndex];
                m_AnnouncerSubtitleIndex++;

                yield return new WaitForSeconds(a_TextDurationSeconds);

                m_AnnouncerText.gameObject.SetActive(false);
            }
        }

        public void SpawnDroneWave(int a_Level)
        {
            m_Drones[a_Level].SetActive(true);

            if (a_Level == 1)
            {
                m_Drones[a_Level].transform.DORotate(new Vector3(0, 45, 0), 3).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            }
            else if (a_Level == 2)
            {
                m_Drones[a_Level].transform.DORotate(new Vector3(0, -45, 0), 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutBack);
            }
        }

        public void SetTutorialKeys(string a_Parameter, bool a_State)
        {
            m_TutorialKeysAnim.SetBool(a_Parameter, a_State);
        }

        public void DestroyDrone(Vector3 a_Position)
        {
            m_DronesDestroyed++;
            Instantiate(m_Explosion, a_Position, Quaternion.identity);

            try { m_SFXAudioSource.PlayOneShot(m_DroneExplosion); } catch (System.Exception e) { Debug.Log(e); }

            MissionSystem.Instance.Progress(1, 777);

            if (m_DronesDestroyed <= 9)
            {
                if (m_DronesDestroyed % 3 == 0)
                {
                    m_Evelator.CompleteLevel();

                    if (m_DronesDestroyed == 6)
                    {
                        SpawnDroneWave(2);
                    }
                }
            }
        }



        public void OpenDoor()
        {
            m_SFXAudioSource.PlayOneShot(m_OpenDoor);
        }

        public void CloseDoor()
        {
            m_SFXAudioSource.PlayOneShot(m_CloseDoor);
        }

        public void BeginBoostChallenge()
        {
            m_SFXAudioSource.PlayOneShot(m_CloseBoostDoors);
        }

        public void CloseBoostDoors()
        {
            m_SFXAudioSource.PlayOneShot(m_CloseBoostDoors);
        }

        public void ActivateChest()
        {
            m_ItemBox.SetActive(true);
        }

        public void BeginFinalBoss()
        {
            float _Alpha = 0;

            m_LeftCover.DOLocalMoveY(44, 2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_LeftCover.DOLocalMoveY(0, 2f).SetEase(Ease.Linear).SetDelay(10f);

            });



            m_RightCover.DOLocalMoveY(44, 2f).SetDelay(10f).OnComplete(() =>
            {
                m_LeftCover.DOLocalMoveY(0, 2f).SetEase(Ease.Linear).SetDelay(10f);

            });

            m_MusicAudioSource.clip = m_BossMusic;
            m_MusicAudioSource.Play();

            DOTween.To(() => _Alpha, x => _Alpha = x, 1f, 1f).OnUpdate(() =>
            {
                m_FinalBossHP.alpha = _Alpha;
            });

            Instantiate(m_LaserTarget, FindObjectOfType<Agent>().transform.position, Quaternion.identity);
            m_FinalBoss.Init();

        }



        public void EndFinalBoss(Vector3 a_Position)
        {
            float _Alpha = 1f;

            bossAlive = false;
            m_MusicAudioSource.Stop();

            DOTween.To(() => _Alpha, x => _Alpha = x, 0f, 1f).OnUpdate(() =>
            {
                m_FinalBossHP.alpha = _Alpha;
            });

            FindObjectOfType<MissionEntry>().CloseMissionEntry();
            MissionSystem.Instance.m_MissionEntryPrefab.SetActive(false);
            MissionSystem.Instance.m_MissionObjectivePrefab.SetActive(false);


            m_MusicAudioSource.clip = m_LevelMusic;
            m_MusicAudioSource.volume = 0.3f;
            m_MusicAudioSource.Stop();

            

            StartCoroutine(Startsplash());

        }

        public IEnumerator Startsplash()
        {

            yield return new WaitForSeconds(2f);

            Context.GameplayMode.State = GameplayMode.EState.Finished;
            yield return null;
        }

        public Transform GetCurrentCheckpoint() => m_Checkpoints[m_CheckpointIndex];
    }
}
