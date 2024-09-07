using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR.UI
{
    public class SkinAdaptor : ContextBehaviour
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

        public string IDon;
        // Start is called before the first frame update
        void Start()
        {
            //activateArr(defaultSkin);
            //setRandom(PersistentStorage.GetString("Skin"));
            if(PersistentStorage.GetString("Skin")!=null && Context!=null ){
			//	Context.PlayerData.SelectedSkin = PersistentStorage.GetString("Skin");
			}
            
            UINFTSelectionSkin.selectionChangedList += setRandom;
           // setRandom("Legendary");
        }
        void OnDestroy(){
             UINFTSelectionSkin.selectionChangedList -= setRandom;
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
           // Debug.Log("r " + r);
         
            hideAll();
            IDon = ID;

          switch(ID.ToLower()){
                case "common" :  activateArr(commonSkin);break;
                case "epic" :  activateArr(epicSkin);break;
                case "legendary" :  activateArr(legendarySkin);break;
                case "mythical" :  activateArr(mythicSkin);break;
                default  : activateArr(defaultSkin);break;
            }

           
        }

        void activateArr(GameObject[] gameArray){
            foreach(GameObject item in gameArray){
                item.transform.SetActive(true);
            };

        }

      
    }
}