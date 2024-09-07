using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class HopscotchBoostPowerUp : MonoBehaviour
    {
        [SerializeField] private MeshRenderer m_MeshRenderer;
        [SerializeField] private BoxCollider m_BoxCollider;
        [SerializeField] private float m_TimeToRespawn, m_BoostTime, m_RotationSpeed;

        private WaitForSeconds m_RespawnTime;

        private void Start()
        {
            m_RespawnTime = new WaitForSeconds(m_TimeToRespawn);
        }

        private void Update()
        {
            transform.Rotate(Vector3.forward * m_RotationSpeed * Time.deltaTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Collect();
            }
        }

        public void Collect()
        {
            HopscotchManager.Instance.ActivateBoost(m_BoostTime);
            m_MeshRenderer.enabled = false;
            m_BoxCollider.enabled = false;

            StartCoroutine(Respawn());
        }

        public IEnumerator Respawn()
        {
            yield return m_RespawnTime;

            m_MeshRenderer.enabled = true;
            m_BoxCollider.enabled = true;
        }
    }
}
