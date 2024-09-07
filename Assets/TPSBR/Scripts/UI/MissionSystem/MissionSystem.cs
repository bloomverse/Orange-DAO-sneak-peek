using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class MissionSystem : MonoBehaviour
    {
        private static MissionSystem m_MissionSystem;
        public static MissionSystem Instance { get { return m_MissionSystem; } }

        [SerializeField] private List<Mission> m_Missions = new List<Mission>();
        [SerializeField] private Dictionary<int, MissionEntry> m_MissionEntries = new Dictionary<int, MissionEntry>();

        [SerializeField] public GameObject m_MissionEntryPrefab;
        [SerializeField] public GameObject m_MissionObjectivePrefab;

        [SerializeField] private AudioSource m_AudioSorce;
        [SerializeField] private AudioClip m_MissionObjectiveCompleteFX, m_NewMissionObjectiveFX, m_MissionCompleteFX;

        private void Awake()
        {
            //DontDestroyOnLoad(this);

            if (m_MissionSystem == null) m_MissionSystem = this;
            // else Destroy(gameObject);




        }

        public void Progress(int a_MissionId, int a_ObjectiveId)
        {
            if (m_MissionEntries.ContainsKey(a_MissionId))
            {
                if (m_MissionEntries[a_MissionId].Progress(a_ObjectiveId))
                {
                    m_AudioSorce.PlayOneShot(m_MissionObjectiveCompleteFX, 1);
                }
            }
        }

        public void BeginMission(int a_MissionId, bool a_OpenPanel = true)
        {
            Debug.Log("BeginMission");
            m_MissionEntries[a_MissionId].BeginMissionObjectives(a_OpenPanel);

            if (a_OpenPanel) m_AudioSorce.PlayOneShot(m_NewMissionObjectiveFX, 1);
        }

        public bool AllCurrentRankMissionObjectivesComplete(int a_MissionId) => m_MissionEntries[a_MissionId].AllCurrentRankObjectivesDone();
        public bool AllLastRankMissionObjectivesComplete(int a_MissionId) => m_MissionEntries[a_MissionId].AllLastRankObjectivesDone();
        public bool AllRankMissionObjectivesComplete(int a_MissionId, int a_Rank) => m_MissionEntries[a_MissionId].AllRankObjectivesDone(a_Rank);

        public void Initialize(int a_MissionId, bool a_OpenPanel = true)
        {
            BeginMission(a_MissionId, a_OpenPanel);
        }
        public void Initialize(List<Mission> a_Missions, bool a_OpenPanel = true)
        {
            m_Missions = a_Missions;

            foreach (Mission _Mission in m_Missions)
            {
                m_MissionEntryPrefab.SetActive(true);
                m_MissionEntries.Add(_Mission.missionId, Instantiate(m_MissionEntryPrefab, transform).GetComponent<MissionEntry>().Init(_Mission.missionId, _Mission.missionTitle, _Mission.objectives, m_MissionObjectivePrefab));
                BeginMission(_Mission.missionId, a_OpenPanel);
            }
        }
        public void Open(int a_MissionId)
        {
            if (m_MissionEntries.ContainsKey(a_MissionId))
            {
                m_MissionEntries[a_MissionId].OpenMissionEntry();
            }
            m_AudioSorce.PlayOneShot(m_NewMissionObjectiveFX, 1);
        }
    }
}
