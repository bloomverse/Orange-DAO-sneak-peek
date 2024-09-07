using TMPro;
using UnityEngine;
using DG.Tweening;
using ReadyPlayerMe;
using System.Collections;
using Fusion.Addons.KCC;
using System.Collections.Generic;

namespace TPSBR
{
    public class GummyGambolManager : MonoBehaviour
    {
        private static GummyGambolManager m_GummyGambolManager;
        public static GummyGambolManager Instance { get { return m_GummyGambolManager; } }

        private Agent m_PlayerAgent;
        private MoveState m_PlayerAgentMoveState;

        private GummyGambolControls m_Controls;

        [SerializeField] private GummyGambolSettings m_GummyGambolSettings;
        [SerializeField] private GameObject m_PlanePrefab, m_ProductObstaclePrefab, m_ProductObstaclePlanePrefab, m_LevelEndPadPrefab, m_CoinPrefab, m_EmptyPlanePrefab, m_UpwardSpeedLines, m_DownwardSpeedLines, m_GoFor;
        [SerializeField] private Transform m_GroundPivot, m_Camera, m_PlayerLeftLane, m_CameraLeftLane, m_PlayerCenterLane, m_CameraCenterLane, m_PlayerRightLane, m_CameraRightLane;
        private Transform m_CurrentLane;
        [SerializeField] private TMP_Text m_Countdown, m_ProductName;
        [SerializeField] private UnityEngine.UI.Image m_ProductRoulette1, m_ProductRoulette2, m_ProductRoulette3;
        private WaitForSeconds m_RouletteWait = new WaitForSeconds(0.1f);
        private List<GameObject> m_Planes = new List<GameObject>();

        private Vector3 m_LevelDirection = Vector3.back, m_Zero = Vector3.zero, m_Up = Vector3.up, m_Forward = Vector3.forward;

        private int m_CurrentLevel = 0, m_CurrentPlane = 0;
        private float m_RouletteTimer = 0, m_StartingCountdown = 0, m_DownGravityMultiplier = 0;
        private bool m_Game = false, m_Pregame = false, m_PlayerBusy = false;

        private void Awake()
        {
            m_GummyGambolManager = this;
            m_Controls = new GummyGambolControls();

            m_Controls.Player.Left.performed += Left_performed;
            m_Controls.Player.Right.performed += Right_performed;
        }
        private void OnEnable() => m_Controls.Enable();
        private void OnDisable() => m_Controls.Disable();
        private void Start()
        {
            m_CurrentLane = m_PlayerCenterLane;
            m_StartingCountdown = m_GummyGambolSettings.m_StartingCountdown + 1f;
            int _Sum = 0;

            for (int i = 0; i < m_GummyGambolSettings.m_LevelPlanes.Length; i++)
            {
                _Sum += m_GummyGambolSettings.m_LevelPlanes[i];
            }

            for (int i = 0, j = 0, _Level = 0, _CurrentPlaneOffset = (int)-m_GummyGambolSettings.m_PlaneOffset, _CurrentLevelOffset = 0; i < _Sum; i++, _CurrentPlaneOffset += (int)m_GummyGambolSettings.m_PlaneOffset, j++)
            {
                if (i < m_GummyGambolSettings.m_InitialPlaneCount)
                {
                    m_Planes.Add(Instantiate(m_PlanePrefab, new Vector3(0, _CurrentLevelOffset, _CurrentPlaneOffset), Quaternion.identity, m_GroundPivot).GetComponent<GummyGambolPlane>().Init(m_GummyGambolSettings.m_LevelMaterials[_Level]));
                    m_Planes[m_Planes.Count - 1].SetActive(true);
                }
                else
                {
                    if (Random.Range(0f, 1f) <= m_GummyGambolSettings.m_PlaneObstacleProbability[_Level])
                    {
                        m_Planes.Add(Instantiate(m_PlanePrefab, new Vector3(0, _CurrentLevelOffset, _CurrentPlaneOffset), Quaternion.identity, m_GroundPivot).GetComponent<GummyGambolPlane>().Init(m_GummyGambolSettings.m_LevelMaterials[_Level], m_GummyGambolSettings.m_PlaneObstacleDifficultyPool[_Level].m_PlaneObstacleDifficultySetups[Random.Range(0, m_GummyGambolSettings.m_PlaneObstacleDifficultyPool[_Level].m_PlaneObstacleDifficultySetups.Length)]));
                    }
                    else if (Random.Range(0f, 1f) <= m_GummyGambolSettings.m_ProductObstacleProbability[_Level])
                    {
                        m_Planes.Add(Instantiate(m_ProductObstaclePrefab, new Vector3(0, _CurrentLevelOffset, _CurrentPlaneOffset), Quaternion.identity, m_GroundPivot));

                        GummyGambolProductObstacle _ProductObstacle = m_Planes[m_Planes.Count - 1].GetComponent<GummyGambolProductObstacle>();

                        _ProductObstacle.Init(m_ProductObstaclePlanePrefab, m_EmptyPlanePrefab, m_GummyGambolSettings.m_LevelMaterials[_Level], ref _CurrentPlaneOffset, (int)m_GummyGambolSettings.m_PlaneOffset, _Level, m_GummyGambolSettings.m_ProductMaterials);
                    }
                    else
                    {
                        m_Planes.Add(Instantiate(m_PlanePrefab, new Vector3(0, _CurrentLevelOffset, _CurrentPlaneOffset), Quaternion.identity, m_GroundPivot).GetComponent<GummyGambolPlane>().Init(m_GummyGambolSettings.m_LevelMaterials[_Level]));
                    }
                }

                if (j >= m_GummyGambolSettings.m_LevelPlanes[_Level] - 1)
                {
                    //m_Planes[i] = Instantiate(m_LevelEndPadPrefab, new Vector3(0, _CurrentLevelOffset, _CurrentPlaneOffset), Quaternion.identity, m_GroundPivot);
                    _Level++;
                    j = -1;
                    //_CurrentLevelOffset += (int)m_GummyGambolSettings.m_LevelOffset;
                }
            }
        }
        private void Update()
        {
            if (m_Game)
            {
                m_GroundPivot.Translate(m_LevelDirection * m_GummyGambolSettings.m_LevelSpeeds[m_CurrentLevel] * Time.deltaTime);
            }
            else if (m_StartingCountdown > 0 && m_Pregame)
            {
                m_StartingCountdown -= Time.deltaTime;
                m_Countdown.text = m_StartingCountdown > 1f ? Mathf.FloorToInt(m_StartingCountdown).ToString() : "GO";

                if (m_StartingCountdown <= 0)
                {
                    m_Countdown.gameObject.SetActive(false);
                    m_Game = true;
                    m_PlayerAgent.AgentInput.SetActionStates(EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.Crouch);
                }
            }

            if (m_RouletteTimer > 0) m_RouletteTimer -= Time.deltaTime;
        }

        public void Begin(Agent a_PlayerAgent)
        {
            m_PlayerAgent = a_PlayerAgent;
            m_PlayerAgentMoveState = a_PlayerAgent.GetComponentInChildren<CharacterAnimationController>().FindState<MoveState>();
            m_PlayerAgent.Character.CharacterController.enabled = false;
            m_Pregame = true;
            m_Countdown.gameObject.SetActive(m_Pregame);
        }
        public void Checkpoint()
        {
            m_Planes[m_CurrentPlane].SetActive(false);
            
            if (m_CurrentPlane < m_Planes.Count - m_GummyGambolSettings.m_InitialPlaneCount)
            {
                m_Planes[m_CurrentPlane + m_GummyGambolSettings.m_InitialPlaneCount].SetActive(true);
            }

            m_CurrentPlane++;
        }
        public IEnumerator ShowProductObstacleClue(int a_ProductIndex)
        {
            m_RouletteTimer = m_GummyGambolSettings.m_RouletteTimer;
            m_GoFor.SetActive(true);
            m_ProductRoulette1.gameObject.SetActive(true);

            while (m_RouletteTimer > 0)
            {
                m_ProductRoulette1.sprite = m_GummyGambolSettings.m_ProductSprites[Random.Range(0, m_GummyGambolSettings.m_ProductSprites.Length)];
                yield return m_RouletteWait;
            }

            m_ProductRoulette1.sprite = m_GummyGambolSettings.m_ProductSprites[a_ProductIndex];

            m_ProductName.text = m_GummyGambolSettings.m_ProductSprites[a_ProductIndex].name;
            m_ProductName.gameObject.SetActive(true);

            yield return new WaitForSeconds(2f);

            m_ProductName.gameObject.SetActive(false);
            m_ProductRoulette1.gameObject.SetActive(false);
            m_GoFor.SetActive(false);
        }
        public IEnumerator EndLevel()
        {
            m_PlayerAgent.AgentInput.SetActionStates(EGameplayInputActionStates.None);
            m_CurrentLevel++;

            yield return null;
        }
        public IEnumerator SwitchLevel()
        {
            m_Game = false;
            m_GroundPivot.DOMove(new Vector3(0, -m_GummyGambolSettings.m_LevelOffset * m_CurrentLevel, m_GroundPivot.position.z - 7f) , 10f).OnUpdate(() =>
            {
                m_PlayerAgent.Character.CharacterController.SetPosition(new Vector3(m_PlayerAgent.transform.position.x, 1f, 0));
            })
            .OnComplete(() =>
            {
                m_Game = true;
                m_PlayerAgent.AgentInput.SetActionStates(EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.Crouch);
                m_UpwardSpeedLines.SetActive(false);
            })
            .SetEase(Ease.Linear);

            m_UpwardSpeedLines.SetActive(true);

            yield return null;
        }


        private void Right_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (!m_Game || m_PlayerBusy || m_CurrentLane == m_PlayerRightLane) return;

            m_PlayerBusy = true;

            float _PosX = m_CurrentLane.position.x;

            DOTween.To(() => _PosX, x => _PosX = x, m_CurrentLane == m_PlayerCenterLane ? m_PlayerRightLane.position.x : m_PlayerCenterLane.position.x, m_GummyGambolSettings.m_ChangeLaneSpeed).
            OnUpdate(() =>
            {
                m_PlayerAgent.Character.CharacterController.SetPosition(new Vector3(_PosX, m_PlayerAgent.transform.position.y, 0));
            }).
            OnComplete(() =>
            {
                m_CurrentLane = m_CurrentLane == m_PlayerCenterLane ? m_PlayerRightLane : m_PlayerCenterLane;
                m_PlayerBusy = false;
            }).
            SetEase(Ease.Linear);

            m_Camera.DOMoveX(m_CurrentLane == m_PlayerCenterLane ? m_CameraRightLane.position.x : m_CameraCenterLane.position.x, m_GummyGambolSettings.m_ChangeLaneSpeed);
        }
        private void Left_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (!m_Game || m_PlayerBusy || m_CurrentLane == m_PlayerLeftLane) return;

            m_PlayerBusy = true;

            float _PosX = m_CurrentLane.position.x;

            DOTween.To(() => _PosX, x => _PosX = x, m_CurrentLane == m_PlayerCenterLane ? m_PlayerLeftLane.position.x : m_PlayerCenterLane.position.x, m_GummyGambolSettings.m_ChangeLaneSpeed).
            OnUpdate(() =>
            {
                m_PlayerAgent.Character.CharacterController.SetPosition(new Vector3(_PosX, m_PlayerAgent.transform.position.y, 0));
            }).
            OnComplete(() =>
            {
                m_CurrentLane = m_CurrentLane == m_PlayerCenterLane ? m_PlayerLeftLane : m_PlayerCenterLane;
                m_PlayerBusy = false;
            }).
            SetEase(Ease.Linear);

            m_Camera.DOMoveX(m_CurrentLane == m_PlayerCenterLane ? m_CameraLeftLane.position.x : m_CameraCenterLane.position.x, m_GummyGambolSettings.m_ChangeLaneSpeed);
        }
    }

    [System.Serializable]
    public struct GGPlaneObstacleLaneData
    {
        public GGLanes m_Lane;
        public int[] m_ObstaclesToActivate;
    }
    [System.Serializable]
    public struct GGPlaneObstacleData
    {
        public GGPlaneObstacleLaneData[] m_ObstacleLaneDatas;
    }

    [System.Serializable]
    public struct GGProductObstacleData
    {
        public int m_PlanesToObstacle, m_SeparatingPlanes;

    }

    public enum GGObstacleDifficulty
    {
        Novice,
        Easy,
        Normal,
        Hard,
        Insane
    }

    public enum GGLanes
    {
        Left,
        Center,
        Right
    }
}
