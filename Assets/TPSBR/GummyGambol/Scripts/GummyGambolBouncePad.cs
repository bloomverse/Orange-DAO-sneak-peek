using UnityEngine;

namespace TPSBR
{
    public class GummyGambolBouncePad : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(GummyGambolManager.Instance.SwitchLevel());
            }
        }
    }
}
