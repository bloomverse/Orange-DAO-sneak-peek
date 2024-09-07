using UnityEngine;

namespace TPSBR
{
    public class HopscotchProductPuzzleActivationPlatform : MonoBehaviour
    {
        [SerializeField] private HopscotchProductPuzzle m_ProductPuzzle;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_ProductPuzzle.TogglePuzzle(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_ProductPuzzle.TogglePuzzle(false);
            }
        }
    }
}
