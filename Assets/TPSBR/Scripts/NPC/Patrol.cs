using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Fusion;

namespace TPSBR
{
    public class Patrol : NetworkBehaviour
    {
        [SerializeField]
        private bool _searching = true;
        [SerializeField]
        private float _speed = 2.5f;

        public Transform[] points;
        public float minDistanceToReach = 0.5f;

        private int _currentPointIndex = 0;
        private NavMeshAgent _agent;
        private Animator _anim;

        private bool _stopped = false;
        
        [SerializeField]
        private float stopTime = 2f;

        public override void Spawned() //online
        //public void Start() //offline
        {
            _agent = GetComponent<NavMeshAgent>();
            _anim = GetComponent<Animator>();
            _agent.autoBraking = false;
            
            StartCoroutine(SetNextDestination());
        }

        //https://platform.openai.com/account/billing/payment-methods

        
        /// <summary>
        /// Set the agent's next destination based on the patrol points.
        /// </summary>
        IEnumerator SetNextDestination()
        {
            while (true)
            {
                if (points[_currentPointIndex])
                {
                    _agent.SetDestination(points[_currentPointIndex].position);
                }

                if (_stopped)
                {
                    _currentPointIndex += 1;

                    if (_currentPointIndex >= points.Length)
                    {
                        _currentPointIndex = 0;
                    }
                }

                yield return new WaitForSeconds(stopTime);

            }
        }

        public void focusAttention(){
            // Create new point 
            Vector3 myPos = transform.position;
            Vector3 cameraPos = Camera.main.transform.position;
             _agent.SetDestination(cameraPos);
             
            // Animate talking 
            //Debug.Log("Rorro");
            // Wait 


        }

        public override void FixedUpdateNetwork() //online
        //public void FixedUpdate() //offline
        {
            if (_searching)
            {
                _agent.speed = _speed;
                _stopped = ReachedDestinationOrGaveUp();

            }
            else
            {
                _agent.speed = 0f;
            }
        }

        private bool ReachedDestinationOrGaveUp()
        {
//            Debug.Log(_agent.pathPending);
            if (!_agent.pathPending)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                    {
                        _anim.SetTrigger("idle");
                        return true;
                    }
                }
                else
                {
                    _anim.SetTrigger("walk");
                }
            }

            return false;
        }
    }
}


   