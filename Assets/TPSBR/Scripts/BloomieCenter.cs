using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace TPSBR
{
    public class BloomieCenter : ContextBehaviour
    {
        [Header("Audio")]
		[SerializeField]
		private AudioEffect _audioEffect;
		[SerializeField]
		private AudioSetup _cooldown;
		[SerializeField]
		private AudioSetup _coreReady;
		[SerializeField]
		private AudioSetup _harvesting;
		[SerializeField]
		private AudioSetup _harvestingEnd;
        [SerializeField]
		private AudioSetup _coin;


        float t;
        public float duration = 1.5f;

        //private string openStatus = "idle";

        [SerializeField]
        private EliminationGameplayModeCustom _eliminationMode;

        private string playerOn = "";
        Player player ;

        private bool intialized;

       public override void Spawned()
		{
//            Debug.Log("Starting bloomie listeners");
           
        }

        public void cooldown_Sound(){
            //Debug.Log("cooldown sound");
            _audioEffect.Play(_cooldown, EForceBehaviour.ForceAny);
        }
        public void coreReady_Sound(){
            //Debug.Log("cooldownReady_Sound");
            _audioEffect.Play(_coreReady, EForceBehaviour.ForceAny);
        }
        public void harvesting_Sound(){
            //Debug.Log("harvesting_Sound");
            _audioEffect.Play(_harvesting, EForceBehaviour.ForceAny);
        }
        public void harvestingEnd_Sound(){
            //Debug.Log("harvestingEnd_Sound");
            _audioEffect.Play(_harvestingEnd, EForceBehaviour.ForceAny);
           // _audioEffect.Play(_coin, EForceBehaviour.ForceAny);
        }

       string oldVar = "";

        void OnTriggerStay(Collider other) {
            if(other.gameObject.tag=="Player"){
                 player = Context.NetworkGame.GetPlayer(Context.LocalPlayerRef);
                 playerOn = player.DBID;
                 Debug.Log("Player on bloomie center " + playerOn);
            }
        }

        void OnTriggerExit(Collider other) {
            
            if(other.gameObject.tag=="Player"){
                 player = null;
                 playerOn ="";
                // Debug.Log("Seetings player on ");
            }
        }
 


        public override void FixedUpdateNetwork(){
            //if(playerOn!=null){
                //Debug.Log("PLAYER ON BLOOMIE CENTER CALLING STARTBLOOMIEACTIV" + playerOn);
              // if(_eliminationMode==null){
              /*  if(!intialized && _eliminationMode!=null){
                    _eliminationMode = Context.GameplayMode as EliminationGameplayModeCustom;  
                    _eliminationMode.Cooling += cooldown_Sound;
                    _eliminationMode.CoreReady += coreReady_Sound;
                    _eliminationMode.Harvesting += harvesting_Sound;
                    _eliminationMode.HarvestFinish += harvestingEnd_Sound;
                    intialized = true;
                }*/
                
               //  
               //}
           /*    if(oldVar!=playerOn){
                    _eliminationMode.StartBloomieActivity(player);
                    oldVar = playerOn;
               }*/
                

            
            
        }


              /* void OnTriggerExit(Collider other) {
            
            if(other.gameObject.tag=="Player"){
                
                 playerOn ="";
                // Debug.Log("Seetings player on ");
            }
        }*/



    }
}