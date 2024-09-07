using UnityEngine;

namespace TPSBR
{
    public class HopscotchBoostLine : MonoBehaviour
    {
        [SerializeField] private BoxCollider m_BoxCollider;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HopscotchManager.Instance.AdjustProjectileLauncherAngle();
                m_BoxCollider.enabled = false;
            }
        }

        public void ToggleCollider(bool a_State)
        {
            m_BoxCollider.enabled = a_State;
        }
    }
}
