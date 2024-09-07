using UnityEngine;

namespace TPSBR
{
    public class HopscotchEndPlatform : MonoBehaviour
    {
        [SerializeField] private GameObject m_Portal;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_Portal.SetActive(true);
                HopscotchManager.Instance.ActivatePortal();
            }
        }
    }
}
