using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class TutorialLaserTarget : MonoBehaviour
    {
        private Transform m_Player;
        [SerializeField] private Vector3 m_LaserTargetOffset;
        public float m_Speed;

        private void Start()
        {
            m_Player = FindObjectOfType<Agent>().transform;
        }

        void Update()
        {
//            Debug.Log("finding player tutorial... " + m_Player);
            if (!m_Player) return;

            transform.position = Vector3.MoveTowards(transform.position, m_Player.position , m_Speed * Time.deltaTime);
        }
    }
}
