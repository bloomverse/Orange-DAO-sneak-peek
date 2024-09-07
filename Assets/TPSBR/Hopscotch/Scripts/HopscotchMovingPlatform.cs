using UnityEngine;

namespace TPSBR
{
    public class HopscotchMovingPlatform : MovingPlatform 
    {
        [SerializeField] private MeshRenderer m_MeshRenderer;
        [SerializeField] private Transform m_Tube;
        private float m_InitialSpeed, m_MaxSpeed;
        public HopscotchMovingPlatform Init(Material a_Material, Vector3 a_MovingDirection, float a_Radius, float a_Speed, float a_MaxSpeed)
        {
            m_Tube.transform.position = new Vector3(0, -3f, transform.position.z);
            m_Tube.transform.rotation = Quaternion.Euler((a_MovingDirection * 90) + (Vector3.up * 90));
            m_Tube.transform.parent = null;
            m_MeshRenderer.material = a_Material;
            m_MaxSpeed = a_MaxSpeed;
            m_InitialSpeed = a_Speed;
            _moveDirection = a_MovingDirection;
            _height = a_Radius;
            _speed = a_Speed;

            base.Spawned();

            return this;
        }

        public void SetSpeedByLevel(float a_Factor) => _speed = m_InitialSpeed + (a_Factor * (m_MaxSpeed - m_InitialSpeed));    }
}
