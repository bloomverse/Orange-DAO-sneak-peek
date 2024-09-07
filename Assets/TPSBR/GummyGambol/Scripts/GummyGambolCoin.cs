using UnityEngine;

namespace TPSBR
{
    public class GummyGambolCoin : MonoBehaviour
    {
        [SerializeField] private float m_RotationSpeed;

        private Vector3 m_RotationDirection = Vector3.forward;

        private void Update()
        {
            transform.Rotate(m_RotationDirection * m_RotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HopscotchManager.Instance.CoinCollected(gameObject);
            }
        }
    }
}
