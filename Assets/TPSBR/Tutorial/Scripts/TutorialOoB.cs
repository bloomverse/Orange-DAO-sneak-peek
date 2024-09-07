using UnityEngine;

namespace TPSBR
{
    public class TutorialOoB : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Agent _TutorialPlayer = GameObject.FindObjectOfType<Agent>();
                Transform _Checkpoint = TutorialManager.Instance.GetCurrentCheckpoint();

                _TutorialPlayer.Character.CharacterController.SetDynamicVelocity(Vector3.zero);
                _TutorialPlayer.Character.CharacterController.SetExternalVelocity(Vector3.zero);
                _TutorialPlayer.Character.CharacterController.SetPosition(_Checkpoint.transform.position);
                _TutorialPlayer.Character.CharacterController.SetLookRotation(_Checkpoint.transform.rotation);
            }
        }
    }
}
