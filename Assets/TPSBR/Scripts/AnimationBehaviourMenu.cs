using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class AnimationBehaviourMenu : MonoBehaviour
    {

        [SerializeField]
        ParticleSystem Rocket1;
        
        [SerializeField]
        ParticleSystem Rocket2;

        [SerializeField]
        ParticleSystem Smoke1;
        [SerializeField]
        ParticleSystem Smoke2;
        
       	public void StartFly(){
			//Debug.Log("Start Fly");

            var em = Rocket1.emission;
            em.enabled = true;
            em.rateOverTime = 20.0f;

            em.SetBursts(
            new ParticleSystem.Burst[]{
                new ParticleSystem.Burst(4.0f, 20),
                
            });

              var em2 = Rocket2.emission;
            em2.enabled = true;
            em2.rateOverTime = 20.0f;

            em2.SetBursts(
            new ParticleSystem.Burst[]{
                new ParticleSystem.Burst(4.0f, 20),
                
            });

            var em3 = Smoke1.emission;
            em3.enabled = true;
            em3.rateOverTime = 40.0f;
            //----- SMOKE

            em3.SetBursts(
            new ParticleSystem.Burst[]{
                new ParticleSystem.Burst(1.0f, 40),
                
            });

             var em4 = Smoke2.emission;
            em4.enabled = true;
            em4.rateOverTime = 40.0f;

            em4.SetBursts(
            new ParticleSystem.Burst[]{
                new ParticleSystem.Burst(1.0f, 40),
                
            });
            
            Smoke1.Play();
            Smoke2.Play();
            Rocket1.Play();
            Rocket2.Play();
		}

		public void StopFly(){

            var em = Rocket1.emission;
            em.enabled = false;
            em.rateOverTime = 0.0f;

			var em2 = Rocket2.emission;
            em2.enabled = false;
            em2.rateOverTime = 0.0f;

            var em3 = Smoke1.emission;
            em3.enabled = false;
            em3.rateOverTime = 0.0f;

            var em4 = Smoke2.emission;
            em4.enabled = false;
            em4.rateOverTime = 0.0f;
           // Rocket1.Stop();
           // Rocket2.Stop();
		}
    }
}
