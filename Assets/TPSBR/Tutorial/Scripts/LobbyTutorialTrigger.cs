using Fusion;
using UnityEngine;

namespace TPSBR
{
    public class LobbyTutorialTrigger : NetworkBehaviour
    {
        [SerializeField] private int m_MissionId;
        [SerializeField] private int m_ObjectiveId;
        [SerializeField] private bool m_Start;
        private void OnTriggerStay(Collider other)
        {
           // Debug.LogError("Enter Trigger");
            if (other.CompareTag("Player"))
            {
             //   Debug.LogError("Gotcha");

                Agent[] _Agents = FindObjectsOfType<Agent>();
                int _LocalPlayerIndex = -1;
                int i = 0;

                foreach (Agent _Agent in _Agents)
                {
                    if (_Agent.IsLocal) _LocalPlayerIndex = i;
                    i++;
                }

                if (_LocalPlayerIndex >= 0)
                {
                    if (_Agents[_LocalPlayerIndex].IsLocal)
                    {
                        if (m_Start)
                        {
                            MissionSystem.Instance.BeginMission(m_MissionId);
                        }
                        else
                        {
                            MissionSystem.Instance.Progress(m_MissionId, m_ObjectiveId);
                        }

                        gameObject.SetActive(false);
                    }
                }
            }
        }

        public void Init(int a_MissionId, int a_ObjectiveId, bool a_Start)
        {
            m_MissionId = a_MissionId;
            m_ObjectiveId = a_ObjectiveId;
            m_Start = a_Start;
        }
    }
}
