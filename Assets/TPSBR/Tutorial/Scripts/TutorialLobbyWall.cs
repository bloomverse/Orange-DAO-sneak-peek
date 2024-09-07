using UnityEngine;

namespace TPSBR
{
    public class TutorialLobbyWall : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Transform m_Player;

        public float m_EnableDistance;
        public float m_MaxValueDistance;

        private void Update()
        {
            if (m_Player)
            {
                float _Distance = Vector3.Distance(transform.position, m_Player.position);

                if (_Distance <= m_EnableDistance)
                {
                    m_SpriteRenderer.color = new Color(1, 0, 0, m_MaxValueDistance / _Distance);
                }
                else
                {
                    m_SpriteRenderer.color = new Color(1, 0, 0, 0);
                }
            }
        }

        public void Init(Transform a_Player) => m_Player = a_Player;
    }
}
