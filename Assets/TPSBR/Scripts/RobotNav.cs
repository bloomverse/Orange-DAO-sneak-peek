using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;


namespace TPSBR
{


public class RobotNav : NetworkBehaviour
{
    [SerializeField]
    private bool _searching = true;
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private float _radius = 18f;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip madsound;
    [SerializeField]
    private AudioClip cleanSound;
    [SerializeField]
    private ParticleSystem cleanEffect;

    private int _layer;
    [SerializeField]
    private List<Transform> _patrolPoints;
    private List<GameObject> _sprayPoints = new List<GameObject>();
    
    private int _index = 0;
    private bool _stopped = false;
    private bool _spray = false;
    private NavMeshAgent _agent;



    public override void Spawned()
    {
        _agent = GetComponent<NavMeshAgent>();
        gameObject.GetComponent<SphereCollider>().radius = _radius;
        _layer = LayerMask.NameToLayer("Spray");
        //Debug.Log("Robot layer mask: " + _layer);

        if (Object.HasStateAuthority == true)
        {
            StartCoroutine(MoveRobot());
        }

        
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.layer == _layer){
           // Debug.Log("Robot trigger spray");

            GameObject waypoint = collider.gameObject;

            _sprayPoints.Add(waypoint);
        }
    }

    public override void FixedUpdateNetwork()
    {
        

        if(_searching){
            _agent.speed = _speed;

            _stopped = ReachedDestinationOrGaveUp();

            if(_sprayPoints.Count > 0){
                _spray = true;
            }else{
                _spray = false;
            }

        }else{
            _agent.speed = 0f;
        }
    }

    private bool ReachedDestinationOrGaveUp()
    {

        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DestroySpray(){
        if (Object.HasStateAuthority == true)
        {
            
            audioSource.PlayOneShot(cleanSound);
            cleanEffect.Play();
            NetworkObject spray_object = _sprayPoints[0].GetComponent<NetworkObject>();
            Runner.Despawn(spray_object);

            // Destroy(_sprayPoints[0]);
            _sprayPoints.Remove(_sprayPoints[0]);
        }
    }

    IEnumerator MoveRobot()
    {
      //  Debug.Log("mogn");
        while (true)
        {

            if(_spray){
                if(_stopped){
                    audioSource.PlayOneShot(madsound);
                    DestroySpray();    
                }

                yield return new WaitForSeconds(1);

                if(_spray){
                    if(_sprayPoints[0]){
                        
                        _agent.SetDestination(_sprayPoints[0].transform.position);
                    }
                }else{
                    _agent.SetDestination(_patrolPoints[_index].position);
                }
            }else{
                yield return new WaitForSeconds(1);
                

                if(_patrolPoints[_index])
                {
                    _agent.SetDestination(_patrolPoints[_index].position);
                }

                if(_stopped){
                    _index += 1;

                    if(_index >= _patrolPoints.Count ){
                        _index = 0;
                    }
                }
            }

            yield return new WaitForSeconds(1);
            
        }
    }
}

}
