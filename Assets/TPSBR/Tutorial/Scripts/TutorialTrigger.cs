using UnityEngine;

namespace TPSBR
{
    public class TutorialTrigger : MonoBehaviour
    {
        [SerializeField] private int m_ObjectiveId;
        [SerializeField] private bool m_Start;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (m_Start)
                {
                    MissionSystem.Instance.BeginMission(1);
                }
                else
                {
                    MissionSystem.Instance.Progress(1, m_ObjectiveId);
                }

                gameObject.SetActive(false);
            }
        }
    }
}
