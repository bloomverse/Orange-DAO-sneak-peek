using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR.UI
{
    public class SkinAdaptorTextures : ContextBehaviour
    {
        [SerializeField] private GameObject[] meshObjects;
        [SerializeField] private Material[] defaultSkin;
        [SerializeField] private Material[] commonSkin;
        [SerializeField] private Material[] epicSkin;
        [SerializeField] private Material[] legendarySkin;
        [SerializeField] private Material[] mythicalSkin;
        public string IDon;
        void Start()
        {
            
            if(PersistentStorage.GetString("Skin")!=null && Context!=null ){
				Context.PlayerData.SelectedSkin = PersistentStorage.GetString("Skin");
			}  
            UINFTSelectionSkin.selectionChangedList += setRandom;
        }
        void OnDestroy(){
             UINFTSelectionSkin.selectionChangedList -= setRandom;
        }
        void hideAll(){}
        public void setRandom(string ID){
            hideAll();
            IDon = ID;
             ChangeMeshMaterials(IDon);
        }

        private void ChangeMeshMaterials(string skinType)
        {
            Material[] selectedMaterials = defaultSkin; // Default selection

            Debug.Log(skinType.ToLower() + " sKIN LOWER");
            switch (skinType.ToLower())
            {
                case "default": selectedMaterials = defaultSkin; break;
                case "common": selectedMaterials = commonSkin; break;
                case "epic": selectedMaterials = epicSkin; break;
                case "legendary": selectedMaterials = legendarySkin; break;
                case "mythical": selectedMaterials = mythicalSkin;
           
            break;
                // No default case needed as defaultSkin is already selected
            }

            for (int i = 0; i < meshObjects.Length; i++)
            {
                if (meshObjects[i] != null && i < selectedMaterials.Length)
                {
                    meshObjects[i].GetComponent<Renderer>().material = selectedMaterials[i];
                }
            }
        }      
    }
}