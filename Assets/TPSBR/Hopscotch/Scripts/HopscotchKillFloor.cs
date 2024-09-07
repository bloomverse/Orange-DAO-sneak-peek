using UnityEngine;

namespace TPSBR
{
    public class HopscotchKillFloor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HopscotchManager.Instance.Fall();
            }
        }
    }
}
