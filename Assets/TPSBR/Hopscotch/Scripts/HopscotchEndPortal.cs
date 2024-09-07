using UnityEngine;

namespace TPSBR
{
    public class HopscotchEndPortal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HopscotchManager.Instance.CompleteLevel(gameObject);
            }
        }
    }
}
