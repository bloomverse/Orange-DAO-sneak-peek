using DG.Tweening;
using UnityEngine;

namespace TPSBR
{
    public class TutorialBoostDoor : MonoBehaviour
    {
        [SerializeField] private Transform m_LeftBoostDoor, m_RightBoostDoor;
        [SerializeField] private float m_LeftDoorXTarget, m_RightDoorXTarget, m_TimeToClose;
        private float m_LeftDoorXStart, m_RightDoorXStart;
        private bool m_Active = false, m_MissionStarted = false;


        private void Start()
        {
            m_LeftDoorXStart = m_LeftBoostDoor.localPosition.x;
            m_RightDoorXStart = m_RightBoostDoor.localPosition.x;
        }
        public void CloseDoors()
        {
            m_Active = true;
            m_LeftBoostDoor.DOLocalMoveX(m_LeftDoorXTarget, m_TimeToClose).SetEase(Ease.Linear);
            m_RightBoostDoor.DOLocalMoveX(m_RightDoorXTarget, m_TimeToClose).SetEase(Ease.Linear).OnComplete(() =>
            {
                TutorialManager.Instance.CloseBoostDoors();
            });
        }

        public void Setup()
        {
            if (!m_Active)
            {
                m_LeftBoostDoor.gameObject.SetActive(true);
                m_RightBoostDoor.gameObject.SetActive(true);
            }
            else
            {
                m_Active = false;
                m_LeftBoostDoor.localPosition = new Vector3(m_LeftDoorXStart, m_LeftBoostDoor.localPosition.y, m_LeftBoostDoor.localPosition.z);
                m_RightBoostDoor.localPosition = new Vector3(m_RightDoorXStart, m_LeftBoostDoor.localPosition.y, m_LeftBoostDoor.localPosition.z);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !m_Active)
            {
                CloseDoors();

                if (!m_MissionStarted)
                {
                    MissionSystem.Instance.BeginMission(1);
                    m_MissionStarted = true;
                }
            }
        }
    }
}
