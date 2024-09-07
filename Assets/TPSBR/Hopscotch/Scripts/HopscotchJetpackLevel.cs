using UnityEngine;

namespace TPSBR
{
    public class HopscotchJetpackLevel : MonoBehaviour
    {
        [SerializeField] private HopscotchMovingDonut[] m_MovingDonuts;
        public Transform[] m_CoinLocations;
        [SerializeField] private float m_HeightMaxOffset, m_WidthMaxOffset;

        public void IncreaseSpeedFactor(float a_SpeedFactor)
        {
            for (int i = 0; i < m_MovingDonuts.Length; i++)
            {
                m_MovingDonuts[i].IncreaseSpeedFactor(a_SpeedFactor);
            }
        }
    }
}
