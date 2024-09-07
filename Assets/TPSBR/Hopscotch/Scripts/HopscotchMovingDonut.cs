using UnityEngine;

namespace TPSBR
{
    public class HopscotchMovingDonut : MonoBehaviour
    {
        [SerializeField] private float m_Speed, m_Width, m_Height;
        private float m_StartX, m_StartY, m_SpeedFactor = 0.1f;
        private void Start()
        {
            m_StartX = transform.position.x;
            m_StartY = transform.position.y;
        }
        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector3(m_StartX + (Mathf.Cos(Time.time) * m_Width * m_SpeedFactor), m_StartY + (Mathf.Sin(Time.time) * m_Height * m_SpeedFactor), transform.position.z);
        }

        public void IncreaseSpeedFactor(float a_SpeedFactor)
        {
            m_SpeedFactor = a_SpeedFactor;
        }
    }
}
