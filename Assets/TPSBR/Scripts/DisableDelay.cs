using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TPSBR.GameplayMode;

namespace TPSBR
{
    public class DisableDelay : NetworkBehaviour
    {
        public GameplayMode gameplay;

        public void Start()
        {
            gameplay = FindObjectOfType<GameplayMode>();
        }
        public void DisableCollider()
        {
            StartCoroutine(Delay());
            
        }

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.1f);
            this.GetComponentInParent<NetworkMecanimAnimator>().Animator.SetBool("punch", false);
            yield return new WaitForSeconds(1f);
            this.GetComponent<SphereCollider>().enabled = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            //other.gameObject.name == "PLAYER COLLIDER" || 
            if (other.gameObject.GetComponentInParent<Health>()!= null)
            {
                //other.gameObject;
                var enemyHealth = other.gameObject.GetComponentInParent<Health>();
                var amount = enemyHealth.ApplyDamage(25);

                HitData hitData = new HitData();
                hitData.Action = EHitAction.Damage;
                hitData.Amount = amount;
                hitData.Position = this.gameObject.transform.position;
                hitData.InstigatorRef = enemyHealth.Object.InputAuthority;
                hitData.Direction = (other.gameObject.transform.position - transform.position).normalized;
                hitData.Normal = Vector3.up;
                hitData.Target = enemyHealth;

                hitData.HitType = EHitType.Pistol;

                HitUtility.ProcessHit(ref hitData);

                var victimRef = enemyHealth.GetComponent<Agent>().Object.InputAuthority;
                var victimPlayer = enemyHealth.Context.NetworkGame.GetPlayer(victimRef);
                var victimStatistics = victimPlayer != null ? victimPlayer.Statistics : default;

                if (amount <= 0f && victimStatistics.IsAlive)
                {
                    Debug.Log("AgentDeath : " + victimStatistics.IsAlive);
                    hitData.IsFatal = true;
                    enemyHealth.Context.GameplayMode.AgentDeath(enemyHealth.GetComponent<Agent>(), hitData);
                }
                FixedUpdateNetwork();
                //Destroy(gameObject);
                // RPC_SpawnEffect();
            }
        }

        public override void FixedUpdateNetwork()
        {

            if(gameplay!=null){
                base.FixedUpdateNetwork();
            switch (gameplay.State)
            {
                case EState.Active:
                   // gameplay.FixedUpdateNetwork_Active();
                    break;
                case EState.Finished:
                   // gameplay.FixedUpdateNetwork_Finished();
                    break;
            }
            }
            
        }
    }
}
