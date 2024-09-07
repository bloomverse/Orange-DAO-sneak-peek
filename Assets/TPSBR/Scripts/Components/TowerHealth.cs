using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    public class TowerHealth : ContextBehaviour
    {
        public int maxHealth;
        public int currentHealth;

        public GameObject objecttoDestroy;
        public GameObject collidedObject;
        public Image health;
        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "bullet")
            {
                collidedObject = other.gameObject;
                if (collidedObject.GetComponent<DummyProjectile>())
                {
                    currentHealth -= 10;
                }
                if (collidedObject.GetComponent<HomingProjectile>())
                {
                    currentHealth -= 50;
                }
                health.fillAmount = (currentHealth * 0.01f);
                objecttoDestroy = other.gameObject;
                if (currentHealth <= 0)
                {
                    Invoke("TurnOffBulletDelay", 1f);
                }
                else
                {
                    Invoke("TurnOffBulletDelay2", 0.5f);
                }
            }
        }

        [Rpc]
        public void RPC_UpdateHealth()
        {
            if (collidedObject.GetComponent<DummyProjectile>())
            {
                currentHealth -= 10;
            }
            if (collidedObject.GetComponent<HomingProjectile>())
            {
                currentHealth -= 50;
            }
            health.fillAmount = (currentHealth * 0.01f);
            objecttoDestroy = collidedObject;
            if (currentHealth <= 0)
            {
                Invoke("TurnOffBulletDelay", 1f);
            }
            else
            {
                Invoke("TurnOffBulletDelay2", 0.5f);
            }

        }

        public void TurnOffBulletDelay()
        {
            RPC_TurnOffBulletDelay();
        }
        public void TurnOffBulletDelay2()
        {
            RPC_TurnOffBulletDelay2();
        }

        [Rpc]
        public void RPC_TurnOffBulletDelay()
        {
            objecttoDestroy.SetActive(false);
            this.GetComponentInParent<TowerController>().SetActive(false);
        }

        [Rpc]
        public void RPC_TurnOffBulletDelay2()
        {
            objecttoDestroy.SetActive(false);
        }
    }
}
