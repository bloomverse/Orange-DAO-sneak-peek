using Fusion;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TPSBR
{
    public class SkinLiveAdaptor :  ContextBehaviour
    {

        [SerializeField]
        private GameObject[] defaultSkin;
        [SerializeField]
        private GameObject[] commonSkin;
        [SerializeField]
        private GameObject[] epicSkin;
        [SerializeField]
        private GameObject[] legendarySkin;
        [SerializeField]
        private GameObject[] mythicSkin;

        
        // Start is called before the first frame update

     //   private Agent _agent;
//
        
       // [Networked(OnChanged = nameof(OnSkinChanged), OnChangedTargets = OnChangedTargets.All), HideInInspector]
		//public  string _selSkin { get; set; } 
        
        //public static void  OnSkinChanged(Changed<SkinLiveAdaptor> changed){
           //changed.Behaviour.setRandom(changed.Behaviour._selSkin);
      // }



        public void OnSpawned(Agent agent){
        
           // _agent  = agent;
            
            
           // if (Object.HasInputAuthority == true){
              //  var player =  Context.NetworkGame.GetPlayer(Context.LocalPlayerRef);
                //_selSkin = player.SelectedSkin;
             //   setRandom(player.SelectedSkin);
               // Debug.Log("Entrando a onSpawned" + player.SelectedSkin);
          //  }

            

             //setRandom(player.SelectedSkin);

            
            //if (Object.HasInputAuthority == true){
            //    var player = Context.NetworkGame.GetPlayer(Object.InputAuthority);
            //    _selSkin =  Context.PlayerData.SelectedSkin;
            //}
            
          // if (Object.HasInputAuthority == true){
           //     RPC_setSkin(Context.PlayerData.SelectedSkin);
           //}
			//	return;

            

            //var player = Context.NetworkGame.GetPlayer(Object.InputAuthority);
            //Debug.Log(player.Context.PlayerData.SelectedSkin  + " player selected---------------");
            //_selSkin =  Context.PlayerData.SelectedSkin;
        }

      /*  [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
		private void RPC_setSkin(string selectedSkin)
		{
            //_selSkin = selectedSkin;
            setRandom(_selSkin);
        }*/

        void Start()
        {
            
            

           
        /*    var player2 = Context.NetworkGame.GetPlayer(Object.InputAuthority);

            

            Debug.Log(player.Context.PlayerData.userData._id + " @ player" );
            Debug.Log(player.Context.PlayerData.SelectedSkin + " @ player skin" );



            Debug.Log(player.Context.PlayerData.userData._id + " @ player2" );
            Debug.Log(player.Context.PlayerData.SelectedSkin + " @ player skin2" );

            */

           // if(Context.LocalPlayerRef== Object.InputAuthority){
            //    var player = Context.NetworkGame.GetPlayer(Object.InputAuthority);
             //    Debug.Log(player.Context.PlayerData.SelectedSkin  + " Selected SKin");
            //hideAll();
           // setRandom(player.SelectedSkin);
                //
                
            //}   
            
           //Debug.Log(Context.PlayerData.UserID + " user id");
            
        }

        void hideAll(){
            foreach(GameObject item in defaultSkin){
                
                item.transform.SetActive(false);
            };
            foreach(GameObject item in commonSkin){
                item.transform.SetActive(false);
            };
            foreach(GameObject item in epicSkin){
                item.transform.SetActive(false);
            };
            foreach(GameObject item in legendarySkin){
                item.transform.SetActive(false);
            };
            foreach(GameObject item in mythicSkin){
                item.transform.SetActive(false);
            };
        }

        public void setRandom(string ID){
           // var r = Random.Range(0,5);
//            Debug.Log("skinr " + ID);
            hideAll();
            switch(ID.ToLower()){
                case "common" :  activateArr(commonSkin);break;
                case "epic" :  activateArr(epicSkin);break;
                case "legendary" :  activateArr(legendarySkin);break;
                case "mythical" :  activateArr(mythicSkin);break;
                default  : activateArr(defaultSkin);break;
            }

            if(ID=="" || ID==null){
                 activateArr(defaultSkin);
            }


        }

        void activateArr(GameObject[] gameArray){
            foreach(GameObject item in gameArray){
                item.transform.SetActive(true);
            };
        }

   
    }
}
