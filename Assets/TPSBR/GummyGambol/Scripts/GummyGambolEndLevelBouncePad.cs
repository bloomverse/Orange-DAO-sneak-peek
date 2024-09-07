using UnityEngine;

namespace TPSBR
{
    public class GummyGambolEndLevelBouncePad : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(GummyGambolManager.Instance.EndLevel());
            }
        }
    }
}
