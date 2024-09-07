using DG.Tweening;
using UnityEngine;

namespace TPSBR
{
    public class TutorialElevator : MonoBehaviour
    {
        private bool m_IsOn = false;
        private byte m_CurrentLevel = 0;
        private float[] m_Levels = new float[5] {-1f, 4.75f, 10.5f, 16.25f, 24f };

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player") && !m_IsOn)
            {
                Agent _TutorialPlayer = GameObject.FindObjectOfType<Agent>();
                _TutorialPlayer.Character.CharacterController.SetDynamicVelocity(Vector3.zero);
                _TutorialPlayer.Character.CharacterController.SetExternalVelocity(Vector3.zero);
                _TutorialPlayer.Character.CharacterController.SetPosition(transform.position);

                m_IsOn = true;

                MissionSystem.Instance.Progress(1, 4);
                CompleteLevel();
                GetComponent<SphereCollider>().enabled = false;
            }
        }

        public void CompleteLevel()
        {
            m_CurrentLevel++;
            transform.DOMoveY(m_Levels[m_CurrentLevel], 3f);
        }
    }
}
