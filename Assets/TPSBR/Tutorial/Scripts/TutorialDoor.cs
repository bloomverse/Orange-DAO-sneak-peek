using DG.Tweening;
using UnityEngine;

namespace TPSBR
{
    public class TutorialDoor : MonoBehaviour
    {
        [SerializeField] private GameObject m_Door;
        [SerializeField] private MeshRenderer m_LightMaterial;
        private Color m_Green = Color.green;
        [SerializeField] private float m_Offset;
        private float m_DoorYStart;
        [SerializeField] private bool m_IsBoostEntrance;

        private void Start()
        {
            GetComponent<BoxCollider>().enabled = false;
            m_DoorYStart = transform.localPosition.y;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (m_IsBoostEntrance)
                {
                    FindObjectOfType<TutorialBoostDoor>().Setup();
                }

                m_Door.transform.DOLocalMoveY(m_DoorYStart - m_Offset, 0.5f).SetEase(Ease.Linear);
                TutorialManager.Instance.OpenDoor();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_Door.transform.DOLocalMoveY(m_DoorYStart + 0.5f, 0.5f).SetEase(Ease.Linear);
                TutorialManager.Instance.CloseDoor();
            }
        }

        public void EnableDoor()
        {
            m_LightMaterial.material.SetColor("_BaseColor", m_Green);
            m_LightMaterial.material.SetColor("_EmissionColor", m_Green);
            GetComponent<BoxCollider>().enabled = true;
        }
    }


}
