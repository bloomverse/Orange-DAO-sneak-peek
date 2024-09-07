using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class HopscotchProjectileSpawner : NetworkBehaviour
    {
        private List<HopscotchGummyProjectile> m_GummyProjectiles = new List<HopscotchGummyProjectile>();
        [SerializeField] private Transform m_LaunchTransform;
        public Transform[] m_CoinPositions;
        [SerializeField] private BoxCollider m_InitialBoxCollider;
        private float m_ProjectileLaunchSpeed, m_InitialProjectileLaunchSpeed, m_ProjectileMaxLaunchSpeed, m_ProjectileLaunchAngle, m_InitialProjectileLaunchAngle, m_ProjectileLaunchTimer, m_ProjectileLaunchInterval;
        private bool m_Enabled = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (m_InitialBoxCollider.enabled)
                {
                    for (int i = 0; i < m_GummyProjectiles.Count; i++)
                    {
                        m_GummyProjectiles[i].SetActive(false);
                    }

                    m_Enabled = true;
                    m_ProjectileLaunchTimer += m_ProjectileLaunchInterval;
                    m_InitialBoxCollider.enabled = false;
                }
                else
                {
                    m_Enabled = false;
                }
            }
        }

        public HopscotchProjectileSpawner Init(NetworkRunner a_Runner, GameObject a_GummyProjectilePrefab, Material[] a_Materials, int a_GummyProjectilePoolSize, float a_ProjectileLaunchAngle, float a_ProjectileLaunchSpeed, float a_ProjectileLaunchInterval, float a_ProjectileMaxLaunchSpeed)
        {
            m_ProjectileLaunchSpeed = a_ProjectileLaunchSpeed;
            m_InitialProjectileLaunchSpeed = a_ProjectileLaunchSpeed;
            m_ProjectileMaxLaunchSpeed = a_ProjectileMaxLaunchSpeed;
            m_ProjectileLaunchAngle = a_ProjectileLaunchAngle;
            m_InitialProjectileLaunchAngle = a_ProjectileLaunchAngle;
            m_ProjectileLaunchInterval = a_ProjectileLaunchInterval;
            m_LaunchTransform.rotation = Quaternion.Euler(0, -a_ProjectileLaunchAngle, 0);

            for (int i = 0; i < a_GummyProjectilePoolSize; i++)
            {
                NetworkObject _GummyProjectile = a_Runner.Spawn(a_GummyProjectilePrefab, new Vector3(m_LaunchTransform.position.x, -100f, m_LaunchTransform.position.z), Quaternion.identity);
                
                m_GummyProjectiles.Add(_GummyProjectile.GetComponent<HopscotchGummyProjectile>().Init(a_Materials[Random.Range(0, a_Materials.Length)]));
            }

            return this;
        }

        void Update()
        {
            if (m_Enabled && m_ProjectileLaunchTimer > 0)
            {
                m_ProjectileLaunchTimer -= Time.deltaTime;

                if (m_ProjectileLaunchTimer <= 0)
                {
                    FireProjectile();
                }
            }
        }

        private void FireProjectile()
        {
            m_LaunchTransform.rotation = Quaternion.Euler(m_LaunchTransform.rotation.x, Random.Range(-m_ProjectileLaunchAngle, m_ProjectileLaunchAngle), m_LaunchTransform.rotation.z);

            for (int i = 0; i <  m_GummyProjectiles.Count; i++)
            {
                if (!m_GummyProjectiles[i].isActiveAndEnabled)
                {
                    m_GummyProjectiles[i].gameObject.SetActive(true);
                    m_GummyProjectiles[i].FireProjectile(m_LaunchTransform, m_ProjectileLaunchSpeed);

                    break;
                }
            }

            m_ProjectileLaunchTimer += m_ProjectileLaunchInterval;
        }

        public void AdjustLaunchAngle(bool a_Reset)
        {
            if (a_Reset) m_ProjectileLaunchAngle = m_InitialProjectileLaunchAngle;
            else m_ProjectileLaunchAngle *= 1.75f;
        }
        public void AdjustLaunchSpeed(float a_SpeedFactor)
        {
            m_InitialBoxCollider.enabled = true;
            m_ProjectileLaunchSpeed = m_InitialProjectileLaunchSpeed + (a_SpeedFactor * (m_ProjectileMaxLaunchSpeed - m_InitialProjectileLaunchSpeed));
        }
    }
}
