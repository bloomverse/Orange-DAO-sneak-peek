using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class TutorialLaserHit : MonoBehaviour
    {
        public float m_DPS;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Agent>().Health.ApplyDamage(m_DPS * Time.deltaTime);
            }
        }
    }
}
