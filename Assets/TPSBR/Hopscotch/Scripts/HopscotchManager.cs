using DG.Tweening;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TPSBR.UI;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;

namespace TPSBR
{

    public enum statusHop{
        Idle,
        Started,
        Ended
    }
    public class HopscotchManager : ContextBehaviour
    {
        private static HopscotchManager m_HopscotchManager;
        public static HopscotchManager Instance { get { return m_HopscotchManager; } }

        [SerializeField] private HopscotchSettings m_Settings;
        [SerializeField] private GameObject m_JetpackUI, m_BoostUI;
        private GameObject m_GameplayUI;
        private List<NetworkObject> m_Coins = new List<NetworkObject>(); 
        [SerializeField] private AudioSource m_AudioSource, m_AudioSourceMusic;
        [SerializeField] private Image[] m_Hearts;
        [SerializeField] private Image m_Jetpack, m_Boost;
        [SerializeField] private TMP_Text m_CurrentCoins, m_Announcement, _timeOn;
        [SerializeField] private List<Transform> m_Checkpoints = new List<Transform>();
        [SerializeField] private Transform m_StartingPlatformColliderTransform;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private AudioClip m_Announcer, m_JetpackCollect, m_JetpackEnd, m_BoostCollect, m_BoostEnd, m_GummyCollision, m_GummyNearMiss, m_LifeLost, m_PortalActivated, m_PortalEntered, m_CoinCollected;
        private WaitForSeconds m_TutorialKeysWait = new WaitForSeconds(5f);

        private statusHop statusHop = statusHop.Idle;

        // Custom Coin positions for stage 1 
        [SerializeField] private List<Transform> m_coinPosStage1;
        [SerializeField] private GameObject CoinCollectedEffect;
        [SerializeField] private GameObject coinsCanvas;
        [SerializeField] private GameObject userPanelCanvas;
        #region LevelAdjustment

        private List<HopscotchCheckpoint> m_LevelCheckpoints = new List<HopscotchCheckpoint>();
        private List<HopscotchMovingPlatform> m_Stage1Platforms = new List<HopscotchMovingPlatform>();
        private List<HopscotchProductPuzzle> m_Stage2Puzzles = new List<HopscotchProductPuzzle>();
        private HopscotchJetpackLevel m_Stage3;
        private List<HopscotchBoostLine> m_Stage4BoostLines = new List<HopscotchBoostLine>();
        private HopscotchProjectileSpawner m_Stage4;

        

        #endregion

        private Agent m_LocalAgent;

        private Vector3 m_Zero = Vector3.zero, m_Up = Vector3.up, m_Forward = Vector3.forward;

        private EGameplayInputActionStates m_DefaultState = EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Look;

        private float m_JetpackTimer = 0, m_BoostTimer = 0, m_JetpackBarFactor, m_BoostBarFactor, m_PlayTimer;
        private int m_CurrentCheckpoint = 0, m_Lives = 3, m_CoinsCollected = 0, m_Level = 1, m_Score = 0;
        private bool m_FirstJetpackPowerup = true, m_FirstBoostPowerup = true;
        public bool m_Tutorial = false;
        private void Awake()
        {
            m_HopscotchManager = this;

            m_JetpackBarFactor = m_Jetpack.fillAmount;
            m_BoostBarFactor = m_Boost.fillAmount;
            m_CurrentCoins.text = "0";
            statusHop = statusHop.Idle;
        }

        private void Update()
        {
            if (m_JetpackTimer > 0)
            {
                m_JetpackTimer -= Time.deltaTime;

                m_Jetpack.fillAmount = m_JetpackTimer / m_Settings.m_JetpackBarMaxValue * m_JetpackBarFactor;

                if (m_JetpackTimer <= 0) DeactivateJetpack();
            }

            if (m_BoostTimer > 0)
            {
                m_BoostTimer -= Time.deltaTime;

                m_Boost.fillAmount = m_BoostTimer / m_Settings.m_BoostBarMaxValue * m_BoostBarFactor;

                if (m_BoostTimer <= 0) DeactivateBoost();
            }

          
            if(statusHop==statusHop.Started){
                m_PlayTimer += Time.deltaTime;
                int currentTime= Mathf.CeilToInt(m_PlayTimer);
		    	_timeOn.text = $"{currentTime / 60} : {currentTime % 60 :00}";
            }
               

        }

       


        public void Begin(NetworkRunner a_Runner, Agent a_LocalAgent)
        {
            m_LocalAgent = a_LocalAgent;
            transform.parent = m_LocalAgent.transform;
            Vector3 _Position = m_Settings.m_StartingPosition;

            #region LEVEL SPAWN

            for (int i = 0; i < m_Settings.m_Platforms.Length; i++)
            {
                m_Stage1Platforms.Add(a_Runner.Spawn(m_Settings.m_MovingPlatformPrefab, _Position + (m_Settings.m_Platforms[i].m_Direction * m_Settings.m_Platforms[i].m_Radius * -0.5f), Quaternion.Euler(m_Settings.m_StartingRotation)).GetComponent<HopscotchMovingPlatform>().Init(m_Settings.m_Platforms[i].m_Material, m_Settings.m_Platforms[i].m_Direction, m_Settings.m_Platforms[i].m_Radius, m_Settings.m_Platforms[i].m_Speed, m_Settings.m_Platforms[i].m_MaxSpeed));

                _Position += m_Settings.m_Platforms[i].m_NextPlatformOffset;
            }

            m_LevelCheckpoints.Add(a_Runner.Spawn(m_Settings.m_CheckpointPlatformPrefab, _Position, Quaternion.Euler(m_Settings.m_StartingRotation)).GetComponent<HopscotchCheckpoint>());

            _Position += m_Forward * 15f;

            for (int i = 0; i < m_Settings.m_ProductPuzzleCount; i++)
            {
                m_Stage2Puzzles.Add(a_Runner.Spawn(m_Settings.m_ProductPuzzlePrefab, _Position, Quaternion.identity).GetComponent<HopscotchProductPuzzle>().Init(m_Settings.m_PuzzleMaterials, UnityEngine.Random.Range(0, 2) == 0));
                _Position += m_Forward * m_Settings.m_ProductPuzzleZOffset;
            }

            _Position -= m_Forward * 5;

            m_LevelCheckpoints.Add(a_Runner.Spawn(m_Settings.m_CheckpointPlatformPrefab, _Position, Quaternion.Euler(m_Settings.m_StartingRotation)).GetComponent<HopscotchCheckpoint>());
            a_Runner.Spawn(m_Settings.m_JetpackPowerUp, _Position + m_Up * 3f, Quaternion.Euler(m_Settings.m_StartingRotation));
            m_Stage3 = a_Runner.Spawn(m_Settings.m_JetpackLevel, _Position, Quaternion.identity).GetComponent<HopscotchJetpackLevel>();

            _Position += m_Forward * 290f;

            m_LevelCheckpoints.Add(a_Runner.Spawn(m_Settings.m_CheckpointPlatformPrefab, _Position, Quaternion.Euler(m_Settings.m_StartingRotation)).GetComponent<HopscotchCheckpoint>());
            a_Runner.Spawn(m_Settings.m_BoostPowerUp, _Position + m_Up * 3f, Quaternion.Euler(m_Settings.m_StartingRotation));

            _Position += m_Forward * 5f;

            m_Stage4 = a_Runner.Spawn(m_Settings.m_BoostLevel, _Position, Quaternion.identity).GetComponentInChildren<HopscotchProjectileSpawner>().Init(a_Runner, m_Settings.m_GummyProjectilePrefab, m_Settings.m_GummyProjectileMaterials, m_Settings.m_GummyProjectilePoolCount, m_Settings.m_GummyProjectileLaunchAngle, m_Settings.m_GummyProjectileLaunchSpeed, m_Settings.m_ProjectileLaunchInterval, m_Settings.m_GummyProjectileLaunchMaxSpeed);

            for (int i = 0; i < m_Settings.m_BoostPowerUpLines; i++)
            {
                _Position += m_Forward * m_Settings.m_BoostPowerUpDistance + (m_Up * 0.185f);
                m_Stage4BoostLines.Add(a_Runner.Spawn(m_Settings.m_BoostPowerUpLinePrefab, _Position, Quaternion.identity).GetComponent<HopscotchBoostLine>());
            }

            _Position += m_Forward * m_Settings.m_BoostPowerUpDistance;
            _Position += m_Forward * 5f;

            a_Runner.Spawn(m_Settings.m_EndPlatformPrefab, _Position, Quaternion.Euler(m_Settings.m_StartingRotation));

            #endregion

            #region COINS

           /* for (int i = 0, j = 20; i < 5; i++, j += 5)
            {
                m_Coins.Add(a_Runner.Spawn(m_Settings.m_BloomieCoinPrefab, new Vector3(UnityEngine.Random.Range(-5, 5), 2.5f, j), Quaternion.Euler(m_Settings.m_StartingRotation)));
            }*/

            GameObject[] coins = GameObject.FindGameObjectsWithTag("CoinH");
            List<GameObject> m_coinPosStage1 = new List<GameObject>(coins);

            foreach (var a in m_coinPosStage1){
                 m_Coins.Add(a_Runner.Spawn(m_Settings.m_BloomieCoinPrefab, a.transform.position, Quaternion.Euler(m_Settings.m_StartingRotation)));
            }

            for (int i = 0; i < m_Stage3.m_CoinLocations.Length; i++)
            {
                m_Coins.Add(a_Runner.Spawn(m_Settings.m_BloomieCoinPrefab, m_Stage3.m_CoinLocations[i].position + new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), Quaternion.Euler(m_Settings.m_StartingRotation)));
            }

            for (int i = 0; i < m_Stage4.m_CoinPositions.Length; i++)
            {
                m_Coins.Add(a_Runner.Spawn(m_Settings.m_BloomieCoinPrefab, m_Stage4.m_CoinPositions[i].position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, 0), Quaternion.Euler(m_Settings.m_StartingRotation)));
            }

            #endregion

            GameObject _ScoreboardMini = FindObjectOfType<UIScoreboardMini>()?.gameObject;
            _ScoreboardMini.SetActive(false);

            UIGameplayPanel _GameplayUI = FindObjectOfType<UIGameplayPanel>();
            _GameplayUI.transform.gameObject.SetActive(false);
            _GameplayUI.transform.GetChild(1).gameObject.SetActive(false);
            _GameplayUI.transform.GetChild(2).gameObject.SetActive(false);



            StartCoroutine(AdjustCamera());
        }
        private IEnumerator AdjustCamera()
        {
            yield return new WaitForSeconds(2);
            Camera.main.farClipPlane = m_Settings.m_CameraFarClippingPlane;
            m_StartingPlatformColliderTransform.DOLocalMoveY(-0.5f, 3f).OnComplete(() =>
            {
                m_StartingPlatformColliderTransform.gameObject.SetActive(false);
                m_PlayTimer = 0;
                statusHop = statusHop.Started;
            });
        }
        public void Checkpoint()
        {
            m_CurrentCheckpoint++;
            m_Score += m_Settings.m_ScorePerCheckpoint;
            StartCoroutine(AnnounceTextMessage("Checkpoint!", 2f));
        }
        public IEnumerator AnnounceTextMessage(string a_Announcement, float a_Duration)
        {
            m_Announcement.text = a_Announcement;
            m_Announcement.gameObject.SetActive(true);
            m_AudioSource.PlayOneShot(m_Announcer);

            yield return new WaitForSeconds(a_Duration);

            m_Announcement.gameObject.SetActive(false);
        }
        public void Fall()
        {
            m_Lives--;

            m_Hearts[m_Lives].gameObject.SetActive(false);
            m_AudioSource.PlayOneShot(m_LifeLost, 1);

            if (m_Lives <= 0)
            {
                EndGame();
                return;
            }

            m_LocalAgent.Character.CharacterController.SetDynamicVelocity(m_Zero);
            m_LocalAgent.Character.CharacterController.SetPosition(m_Checkpoints[m_CurrentCheckpoint].position);
            m_LocalAgent.Character.CharacterController.SetLookRotation(m_Checkpoints[m_CurrentCheckpoint].rotation);

            m_Stage4.AdjustLaunchAngle(true);
            for (int i = 0; i < m_Stage4BoostLines.Count; i++)
            {
                m_Stage4BoostLines[i].ToggleCollider(true);
            }
        }
        public void CoinCollected(GameObject a_Coin)
        {
            m_CoinsCollected++;
            m_Score += m_Settings.m_ScorePerCoin;
            m_AudioSource.PlayOneShot(m_CoinCollected, 1);

            Instantiate(CoinCollectedEffect, a_Coin.transform.position, Quaternion.identity);

            DOTween.To(() => m_CurrentCoins.fontSize, x => m_CurrentCoins.fontSize = x, 72, 0.25f).OnComplete(() =>
            {
                m_CurrentCoins.text = m_CoinsCollected.ToString();

                DOTween.To(() => m_CurrentCoins.fontSize, x => m_CurrentCoins.fontSize = x, 48, 0.25f);
            });

            a_Coin.SetActive(false);
        }
        private void RespawnCoins()
        {
            int n = 0;

            for (int i = n, j = 20; i < 3; i++, n++, j += 20)
            {
                m_Coins[i].transform.position = new Vector3(UnityEngine.Random.Range(-5, 5), 2.5f, j);
                m_Coins[i].SetActive(true);
            }
            for (int i = n; i < m_Stage3.m_CoinLocations.Length; i++, n++)
            {
                m_Coins[i].transform.position = m_Stage3.m_CoinLocations[i].position + new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0);
                m_Coins[i].SetActive(true);
            }
            for (int i = n; i < m_Stage4.m_CoinPositions.Length; i++, n++)
            {
                m_Coins[i].transform.position = m_Stage4.m_CoinPositions[i].position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, 0);
                m_Coins[i].SetActive(true);
            }
        }

        public void AdjustProjectileLauncherAngle(bool a_Reset = false)
        {
            m_Stage4.AdjustLaunchAngle(a_Reset);
        }
        private IEnumerator SetTutorialAnimation(string a_Param)
        {
            m_Animator.SetBool(a_Param, true);

            yield return m_TutorialKeysWait;

            m_Animator.SetBool(a_Param, false);
        }
        public void ActivateJetpack(float a_Time)
        {
            if (m_FirstJetpackPowerup)
            {
                StartCoroutine(SetTutorialAnimation("jetpack"));
                m_FirstJetpackPowerup = !m_FirstJetpackPowerup;
            }

            m_AudioSource.PlayOneShot(m_JetpackCollect, 1);
            m_JetpackUI.SetActive(true);

            if (!m_LocalAgent.AgentInput.GetActionStates().HasFlag(EGameplayInputActionStates.ToggleJetpack))
            {
                m_LocalAgent.AgentInput.SetActionStates(m_DefaultState ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust);
            }

            m_LocalAgent.Jetpack.AddFuel(m_LocalAgent.Jetpack.MaxFuel);
            m_JetpackTimer = a_Time;
        }

        public void ActivateBoost(float a_Time)
        {
            if (m_FirstBoostPowerup)
            {
                StartCoroutine(SetTutorialAnimation("boost"));
                m_FirstBoostPowerup = !m_FirstBoostPowerup;
            }

            DeactivateJetpack();
            m_AudioSource.PlayOneShot(m_BoostCollect, 1);
            m_BoostUI.SetActive(true);

            if (!m_LocalAgent.AgentInput.GetActionStates().HasFlag(EGameplayInputActionStates.ToggleSpeed))
            {
                m_LocalAgent.AgentInput.SetActionStates(m_DefaultState ^ EGameplayInputActionStates.ToggleSpeed);
            }

            m_LocalAgent.Jetpack.AddFuel(m_LocalAgent.Jetpack.MaxFuel);
            m_BoostTimer = a_Time;
        }
        private void DeactivateJetpack()
        {
            m_JetpackTimer = 0;
            m_AudioSource.PlayOneShot(m_JetpackEnd, 1);
            m_JetpackUI.SetActive(false);
            m_LocalAgent.AgentInput.SetActionStates(m_DefaultState);
            m_LocalAgent.Jetpack.Deactivate();
        }
        private void DeactivateBoost()
        {
            m_BoostTimer = 0;
            m_AudioSource.PlayOneShot(m_BoostEnd, 1);
            m_BoostUI.SetActive(false);
            m_LocalAgent.AgentInput.SetActionStates(m_DefaultState);
        }
        public void CompleteLevel(GameObject a_Portal)
        {
            StartCoroutine(AnnounceTextMessage(m_Level + (m_Level == 1 ? " run" : " runs") + " complete\nDifficulty Up!", 3f));
            RespawnCoins();

            m_Score += m_Settings.m_ScorePerRun;
            m_Level++;

            if (m_Level >= m_Settings.m_MaxLevel)
            {
                EndGame();
                return;
            }

            m_CurrentCheckpoint = 0;
            m_AudioSource.PlayOneShot(m_PortalEntered, 1);
            m_LocalAgent.Character.CharacterController.SetDynamicVelocity(m_Zero);
            m_LocalAgent.Character.CharacterController.SetPosition(m_Zero);
            m_LocalAgent.Character.CharacterController.SetLookRotation(m_Zero);
            m_LocalAgent.AgentInput.SetActionStates(m_DefaultState);
            IncreaseDifficulty();
            DeactivateBoost();
            a_Portal.SetActive(false);
        }
        public void BoopPlayer(Vector3 a_Force)
        {
            m_LocalAgent.Character.CharacterController.AddExternalImpulse(a_Force);
            m_AudioSource.PlayOneShot(m_GummyCollision, 1);
        }
        public void WhooshPlayer()
        {
            m_AudioSource.PlayOneShot(m_GummyNearMiss, 1);
        }
        public void ActivatePortal()
        {
            m_AudioSource.PlayOneShot(m_PortalActivated, 1);
        }
        private void EndGame()
        {

            
            m_AudioSourceMusic.volume = 0;
            Camera.main.transform.parent = null;
            Camera.main.transform.Translate(m_Up * 5);

            coinsCanvas.transform.SetActive(false);
            userPanelCanvas.transform.SetActive(false); 
            //m_GameplayUI?.SetActive(true);
            Debug.Log(m_PlayTimer+ " -  mplayertime"  );

            m_Score -= (int)m_PlayTimer;

            if (PlayerPrefs.GetInt("HV1") == 0) m_Tutorial = true;
            PlayerPrefs.SetInt("HV1", 1);

            try{
                MinigScore minigScore = new MinigScore();
            minigScore.score = m_Score;
            minigScore.username = Context.PlayerData.userData.nickname ;
            minigScore.typeofGame = "hopscotch";
            minigScore.time = m_PlayTimer;
            DataManager.instance.RegisterScoreStart(minigScore);
            }catch(System.Exception e) { Debug.Log(e);}
            

            statusHop = statusHop.Ended;
            
            StartCoroutine(SplashResults());
            
        }
        private void IncreaseDifficulty()
        {
            float _LevelFactor = (float)m_Level / (float)m_Settings.m_MaxLevel;

            for (int i = 0; i < m_LevelCheckpoints.Count; i++)
            {
                m_LevelCheckpoints[i].Reactivate();
            }
            for (int i = 0; i < m_Stage1Platforms.Count; i++)
            {
                m_Stage1Platforms[i].SetSpeedByLevel(_LevelFactor);
            }
            for (int i = 0; i < m_Stage2Puzzles.Count; i++)
            {
                m_Stage2Puzzles[i].Init(m_Settings.m_PuzzleMaterials, UnityEngine.Random.Range(0, 2) == 0);
            }

            m_Stage3.IncreaseSpeedFactor(_LevelFactor);
            m_Stage4.AdjustLaunchSpeed(_LevelFactor);
        }
        private IEnumerator SplashResults()
        {
            yield return null;
            Context.GameplayMode.State = GameplayMode.EState.Finished;
            
        }
        public HopscotchResults GetGameResults()
        {
            return new HopscotchResults { m_CoinsCollected = m_CoinsCollected, m_Score = m_Score, m_finalTime = (int)m_PlayTimer };
        }
        public void restartGame(){
           /* m_CurrentCoins.text = "0";
            m_PlayTimer = 0;
            m_Score = 0;
            m_CoinsCollected = 0;
            RespawnCoins();*/
            //Context.GameplayMode.State = GameplayMode.EState.Finished;

            Global.Networking.StopGame();

			bool _BackToTutorial = HopscotchManager.Instance.m_Tutorial;

          Global.Networking.StartGame(new SessionRequest
            {
                UserID = new Guid().ToString(),
                GameMode = Fusion.GameMode.Host,
                SessionName = Guid.NewGuid().ToString(),
                DisplayName = "Happy Vibes",
                ScenePath = "Assets/TPSBR/Scenes/Hopscotch.unity",
                GameplayType = EGameplayType.Hopscotch,
                ExtraPeers = 0,
                MaxPlayers = 2,
                CustomLobby = "HV." + Application.version,
            });
        

        }
    }
    public struct HopscotchResults
    {
        public int m_CoinsCollected, m_Score;
        public int m_finalTime; 
    }
}
