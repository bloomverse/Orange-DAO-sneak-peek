using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    public class HopscotchCheckpoint : MonoBehaviour
    {
        [SerializeField] private BoxCollider m_CheckpointTrigger;
        [SerializeField] private RawImage m_Display;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HopscotchManager.Instance.Checkpoint();
                m_CheckpointTrigger.enabled = false;
            }
        }
        public RawImage Init() => m_Display;
        public void Reactivate() => m_CheckpointTrigger.enabled = true;
    }
}
