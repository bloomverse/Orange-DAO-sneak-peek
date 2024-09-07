using Fusion;
using UnityEngine;
using static TPSBR.GameplayMode;
using Tarodev;

namespace TPSBR
{
    public class HomingProjectile : Projectile
    {

        // Variables for bullet attributes
        public int damage = 100;
        public float speed = 10f;
        public Transform target;

        private bool hasHit = false;
        GameObject player;

        //[SerializeField] private float _savedTime = 4;
        //private float _currentSavedTime = 4;
        [SerializeField] private float _searchTick = 1;
        private float _currentSearchTime = 1;

        public float attackRange = 150f;


        public float time = 0f;
        public GameObject _impactEffect;

        [Header("MOVEMENT")]
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _rotateSpeed = 95;

        [Header("PREDICTION")]
        [SerializeField] private float _maxDistancePredict = 100;
        [SerializeField] private float _minDistancePredict = 5;
        [SerializeField] private float _maxTimePrediction = 5;
        private Vector3 _standardPrediction, _deviatedPrediction;

        [Header("DEVIATION")]
        [SerializeField] private float _deviationAmount = 50;
        [SerializeField] private float _deviationSpeed = 2;

        public TargetRocket targetRocket;
        private Rigidbody rb;

        [SerializeField]
        private LayerMask _hitMask;


        public override void Fire(NetworkObject owner, Vector3 firePosition, Vector3 initialVelocity, LayerMask hitMask, EHitType hitType)
        {
            //Debug.Log("HomingProjectile.Fire");
            rb = this.GetComponent<Rigidbody>();
        }

        void FindNearestTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");

            Debug.Log("enemies: " + enemies.Length);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                if (enemy.name == "PLAYER COLLIDER")
                {

                    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;

                    }
                }
            }

           //Debug.Log(nearestEnemy + " " + shortestDistance + " " + attackRange + " " + nearestEnemy.gameObject.GetComponentInParent<Health>().IsAlive);


            if (shortestDistance <= attackRange && nearestEnemy.gameObject.GetComponentInParent<Health>().IsAlive)
            {
                //Debug.Log("Target : " + nearestEnemy);
                target = nearestEnemy.transform;
                targetRocket = target.GetComponentInParent<Agent>().transform.Find("HitIndicatorPivot").GetComponent<TargetRocket>();
            }
            else
            {
                /* if(target != null)
                 {
                     ShootTarget();
                     Invoke("playaudio", 0.25f);
                 }

                 target = null;*/
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "DontTrigger") return;

            if(hasHit) return;
            hasHit = true;

            

            Debug.Log(other.gameObject.name + " OBject occiled");
             RPC_SpawnEffect();
           
            

            
/*
            if (other.gameObject.name == "PLAYER COLLIDER" || other.gameObject.name == "KCCCollider")
            {
                player = other.gameObject;
                var enemyHealth = other.gameObject.GetComponentInParent<Health>();
                var amount = enemyHealth.ApplyDamage(damage);


                HitData hitData = new HitData();
                hitData.Action = EHitAction.Damage;
                hitData.Amount = amount;
                hitData.Position = other.gameObject.transform.position;
                hitData.InstigatorRef = enemyHealth.Object.InputAuthority;
                hitData.Direction = (other.gameObject.transform.position - transform.position).normalized;
                hitData.Normal = Vector3.up;
                hitData.Target = enemyHealth;
                hitData.HitType = EHitType.Grenade;


                HitUtility.ProcessHit(ref hitData);

                var victimRef = enemyHealth.GetComponent<Agent>().Object.InputAuthority;
                var victimPlayer = enemyHealth.Context.NetworkGame.GetPlayer(victimRef);
                var victimStatistics = victimPlayer != null ? victimPlayer.Statistics : default;

                if (amount <= 0f && victimStatistics.IsAlive)
                {
                    //Debug.Log("AgentDeath : " + victimStatistics.IsAlive);
                    hitData.IsFatal = true;
                    enemyHealth.Context.GameplayMode.AgentDeath(enemyHealth.GetComponent<Agent>(), hitData);
                }
                //Debug.Log("hit player");
                RPC_SpawnEffect();
                FixedUpdateNetwork();
                
               // Runner.Despawn(Object);
                Destroy(gameObject,.3f);



            }
            else
            {

                if (other.gameObject.name != "PLAYER COLLIDER") {
                    Instantiate(_impactEffect, transform.position, Quaternion.identity);
                    Debug.Log("hit other than player");
                    //RPC_SpawnEffect();
                    FixedUpdateNetwork();
                   //  Runner.Despawn(Object);
                    Destroy(gameObject,0.3f);

              //  if (other.gameObject.name != "PLAYER COLLIDER")
               // {
                

                    //Instantiate(_impactEffect, transform.position, Quaternion.identity);

                }
            }*/
        }



        [Rpc]
        public void RPC_SpawnEffect()
        {
            Debug.Log("RPC_SpawnEffect");

         /*   if (_impactEffect != null)
            {
                var networkBehaviour = _impactEffect.GetComponent<NetworkBehaviour>();
                if (networkBehaviour != null)
                {
                    if (Object.HasStateAuthority == true)
                    {
                        Runner.Spawn(networkBehaviour, transform.position, Quaternion.identity, Object.InputAuthority);
                    }
                }
                else
                {
                    //var effect = Context.ObjectCache.Get(_impactEffect);
                    //effect.transform.SetPositionAndRotation(position, Quaternion.LookRotation(normal));
                }
            }*/
            //Instantiate(_impactEffect, transform.position, Quaternion.identity);
            Runner.Spawn(_impactEffect, transform.position, Quaternion.identity, Object.InputAuthority);
             Destroy(gameObject,.1f);
            
        }

        public override void FixedUpdateNetwork()
        {
          /*  base.FixedUpdateNetwork();
            switch (FindObjectOfType<GameplayMode>().State)
            {
                case EState.Active: 
                    FindObjectOfType<GameplayMode>().FixedUpdateNetwork_Active();
                    break;
                case EState.Finished: 
                    FindObjectOfType<GameplayMode>().FixedUpdateNetwork_Finished();
                    break;
            }*/


        }

        private void PredictMovement(float leadTimePercentage)
        {
            var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

            _standardPrediction = targetRocket.Rb.position + targetRocket.Rb.velocity * predictionTime;
        }

        private void AddDeviation(float leadTimePercentage)
        {
            var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);

            var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

            _deviatedPrediction = _standardPrediction + predictionOffset;
        }

        private void RotateRocket()
        {
            var heading = _deviatedPrediction - transform.position;
            var rotation = Quaternion.LookRotation(heading);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
        }



        void FixedUpdate()
        {

            
// sometimes its null here 
            rb.velocity = transform.forward * _speed;

            if (_currentSearchTime > 0)
            {
                _currentSearchTime -= Time.deltaTime;
            }
            else
            {
                _currentSearchTime = _searchTick;
                FindNearestTarget();
            }


            if (targetRocket == null) return;

            var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, targetRocket.transform.position));
            PredictMovement(leadTimePercentage);
            AddDeviation(leadTimePercentage);
            RotateRocket();



        }


    }

}
