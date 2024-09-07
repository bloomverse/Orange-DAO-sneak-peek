using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TPSBR.UI;

namespace TPSBR
{


    public class LevelGcustomLobby :  MonoBehaviour
    {

        [Header("Object Spawn")]
		[SerializeField]
		private ItemBox _itemBoxPrefab;

        [SerializeField]
        private Transform[] spawnItemsTransform;

       // [SerializeField]
//private ChatNewGui Chat;

        public List<SpawnData> ObjectsToSpawn  => _objectsToSpawn;
        private List<SpawnData> _objectsToSpawn = new List<SpawnData>(1024);


       
        public NPC_chat[] npcs;

        // Start is called before the first frame update
        public void Generate()
        {
            // Debug.Log("Entrando GENERATE--------------------------");
            spawnItems();
         
           // Chat.StartChat();
            checkURL();
            //addSpawnPoints();
        }

        private void checkURL(){
            Dictionary<string, string>  d = URLParameters.GetSearchParameters();
			
			string value="";
			if(d.TryGetValue("partner",out value)){
                
                ActivityManager.instance.StartActivity();

			}
			
        }


        void spawnItems(){
//            Debug.Log("Entrando spawn objecto--------------------------" +  spawnItemsTransform.Length);
            for (int i = 0; i < spawnItemsTransform.Length; i++)
                    {  
                        //Debug.Log("Agregando objecto--------------------------");
                        _objectsToSpawn.Add(new SpawnData(_itemBoxPrefab, spawnItemsTransform[i].position, spawnItemsTransform[i].rotation));
                    }

        }


        void addSpawnPoints(){

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
