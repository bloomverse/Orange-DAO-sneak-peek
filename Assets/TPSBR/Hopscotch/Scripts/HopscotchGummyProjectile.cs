using Fusion.Addons.KCC;
using System.Collections;
using UnityEngine;

namespace TPSBR
{
    public class HopscotchGummyProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_Rigidbody;
        private WaitForSeconds m_DisableWait = new WaitForSeconds(8.5f);

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                HopscotchManager.Instance.BoopPlayer(collision.contacts[0].normal * -100f + Vector3.up * 5f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HopscotchManager.Instance.WhooshPlayer();
            }
        }

        private void OnEnable()
        {
            StartCoroutine(Disable());
        }

        public HopscotchGummyProjectile Init(Material a_Material)
        {
            GetComponent<MeshRenderer>().material = a_Material;
            m_Rigidbody.isKinematic = true;
            return this;
        }

        public void FireProjectile(Transform a_LaunchTransform, float a_LaunchSpeed)
        {
            m_Rigidbody.isKinematic = false;
            m_Rigidbody.transform.position = a_LaunchTransform.position;
            m_Rigidbody.transform.rotation = Quaternion.Euler(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));
            m_Rigidbody.velocity = a_LaunchTransform.forward * -1 * a_LaunchSpeed;
        }

        private IEnumerator Disable()
        {
            yield return m_DisableWait;

            gameObject.SetActive(false);
        }

        public void UpdateDespawnTime(float a_WaitTime)
        {
            m_DisableWait = new WaitForSeconds(a_WaitTime);
        }
    }
}
