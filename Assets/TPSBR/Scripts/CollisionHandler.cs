using Fusion;
using Fusion.Addons.KCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TPSBR.GameplayMode;
using Fusion.Addons.AnimationController;

namespace TPSBR
{
    public class CollisionHandler : NetworkBehaviour
    {
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.collider.CompareTag("Floor"))
        //    {
        //        this.gameObject.GetComponentInParent<AnimationController>().AnimatorTest.SetBool("fall", false);
        //        this.gameObject.GetComponentInParent<CharacterAnimationController>().onGround = true;
        //    }
        //}
        private void OnTriggerEnter(Collider other)
        {
          /*  Debug.Log("Punched Triggered " + other.gameObject.tag);
            if (other.gameObject.tag == "Floor")
            {
                //this.gameObject.GetComponentInParent<AnimationController>().AnimatorTest.SetBool("fall", false);
                this.gameObject.GetComponentInParent<CharacterAnimationController>().onGround = true;
                this.gameObject.GetComponentInParent<Agent>().mecan.Animator.SetBool("IsGrounded", true);
            }
*/
           
        }

        private CharacterAnimationController _cac;
        private Agent _agent;
        private KCC _kcc;


        public override void Spawned(){
            _agent = this.gameObject.GetComponentInParent<Agent>();
            _cac = this.gameObject.GetComponentInParent<CharacterAnimationController>();
            _kcc = this.GetComponentInParent<KCC>();
        }

        public override void FixedUpdateNetwork()
        {
               
            RaycastHit hit;
            float distanceToGround = .2f; // Adjust this value based on your character's height
            Vector3 origin = transform.position; // Starting point of the raycast, typically the character's position

            // Raycast downwards from the character's position
            if (Runner.GetPhysicsScene().Raycast(origin, Vector3.down, out hit, distanceToGround))
            {
                //Debug.Log(distanceToGround + " " + hit.collider.name);
               
                   // _cac.onGround = true;
                    if(_agent.mecan!=null){
                        _agent.mecan.Animator.SetBool("IsGrounded", true);
                    }
                    
            }
            if (_agent.AgentInput.HasActive(EGameplayInputAction.Crouch) == true)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x, 0.5f, this.transform.localScale.z);
                _kcc.SetHeight(1.2f);

            }
            else
            {
                this.transform.localScale = Vector3.one;
                _kcc.SetHeight(1.95f);
            }
        }

        private void Update()
        {
            
        }

    }
}
