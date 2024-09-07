using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Fusion;
using TPSBR.UI;
using UnityEngine.SceneManagement;
using System;

namespace TPSBR
{
    public class LobbyTutorialManager : NetworkBehaviour
    {
        private static LobbyTutorialManager m_LobbyTutorialManager;
        public static LobbyTutorialManager Instance { get { return m_LobbyTutorialManager; } }

        [SerializeField] private AudioClip[] m_AnnouncerClips;
        [SerializeField] private GameObject m_ArrowPrefab, m_WallPrefab, m_LobbyTrigger;
        private GameObject m_HVArrow, m_HVConsoleArrow, m_HVGameArrow, m_ArcadeButton;
        [SerializeField] private TMP_Text m_AnnouncerText;
        [SerializeField] private AudioSource m_AudioSource;

        [SerializeField] private string[] m_AnnouncerSteps;

        private int m_AnnouncerSubtitleIndex = 0, m_AnnouncerAudioIndex = 0;


        private void Start()
        {
            m_LobbyTutorialManager = this;
            m_AnnouncerText = GameObject.Find("NewCanvas")?.transform.GetChild(1).GetComponent<TMP_Text>();
        }

        public void AnnouncerStep(float a_TextDurationSeconds, bool a_ShowSubtitle = true) => StartCoroutine(ShowAnnouncerText(a_TextDurationSeconds, a_ShowSubtitle));
        public void SetAnnouncerStep(int a_Step) { m_AnnouncerAudioIndex = a_Step; m_AnnouncerSubtitleIndex = a_Step; }

        public IEnumerator ShowAnnouncerText(float a_TextDurationSeconds, bool a_ShowSubtitle = true)
        {
            m_AudioSource.Stop();
            m_AudioSource.PlayOneShot(m_AnnouncerClips[m_AnnouncerAudioIndex], 1);
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

        public void SpawnLobbyObjects(Transform a_Player)
        {
            Runner.Spawn(m_WallPrefab, new Vector3(-20, -1.5f, -3), Quaternion.identity).GetComponent<TutorialLobbyWall>().Init(a_Player);
            Runner.Spawn(m_WallPrefab, new Vector3(-20, -1.5f, -44.25f), Quaternion.Euler(0, 180, 0)).GetComponent<TutorialLobbyWall>().Init(a_Player);
            m_HVArrow = Instantiate(m_ArrowPrefab, new Vector3(-8, 0, -19.28f), Quaternion.Euler(0, 90, 90));
            m_HVConsoleArrow = Instantiate(m_ArrowPrefab, new Vector3(-3f, -3, -19.28f), Quaternion.Euler(0, 90, 90));
            m_HVConsoleArrow.transform.localScale = Vector3.one * .25f;
            m_HVGameArrow = Instantiate(m_ArrowPrefab, new Vector3(-5.215f, -2.5f, -23.25f), Quaternion.Euler(0, 180, 90));
            m_HVGameArrow.transform.localScale = Vector3.one * .25f;
            m_ArcadeButton = GameObject.Find("HVGame");
            m_ArcadeButton.SetActive(false);
        }

        public void SpawnLobbyTrigger()
        {
            Debug.Log(Runner);
            Runner.Spawn(m_LobbyTrigger, new Vector3(-6, -3.5f, -19.28f), Quaternion.identity).GetComponent<LobbyTutorialTrigger>().Init(1, 14, false); ;
            //Instantiate(m_LobbyTrigger, new Vector3(-6, -3.5f, -19.28f), Quaternion.identity).GetComponent<LobbyTutorialTrigger>().Init(1, 14, false);
        }

        public void LoadHVMinigame()
        {
            //NetworkProjectConfig.Global.PeerMode = NetworkProjectConfig.PeerModes.Single;

          
				Global.Networking.StopGame();
              
            Global.Networking.StartGame(new SessionRequest
            {
                UserID = new Guid().ToString(),
                GameMode = Fusion.GameMode.Host,
                SessionName = Guid.NewGuid().ToString(),
                DisplayName = "Happy Vibes",
                ScenePath = "Assets/TPSBR/Scenes/Hopscotch.unity",
                GameplayType = EGameplayType.Hopscotch,
                ExtraPeers = 0,
                MaxPlayers = 1,
                CustomLobby = "HV." + Application.version,
            }); 

			

           
           

            

            // Global.Networking.StartLocalScene("Assets/TPSBR/Scenes/Hopscotch.unity");
         
        }

        public void ToggleHVArrow(bool a_State) => m_HVArrow.SetActive(a_State);
        public void ToggleHVConsoleArrow(bool a_State) => m_HVConsoleArrow.SetActive(a_State);
        public void ToggleHVGameArrow(bool a_State) => m_HVGameArrow.SetActive(a_State);
        public void ToggleArcadeButton(bool a_State) => m_ArcadeButton.SetActive(a_State);
    }
}
