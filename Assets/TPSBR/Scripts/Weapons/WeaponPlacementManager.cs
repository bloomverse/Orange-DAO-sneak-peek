using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TPSBR
{

    public class WeaponPlacementManager : ContextBehaviour
    {
        public GameObject weaponPrefab; // Assign this in the inspector
        public ParticleSystem placementEffect; // Assign this in the inspector
        public Button placementButton; // Assign this in the inspector

        private bool isPlacingWeapon = false;
        public GameObject weaponPlaceholder;
        public float raycastDistance = 6f; // the distance to cast the ray
        public LayerMask layerMask; // the layer mask to detect 

        public Color rayColor = Color.red;
        private void Start()
        {
            //placementButton.onClick.AddListener(StartPlacingWeapon);
        }

        private void FixedUpdate()
        {
           /* if (Input.GetKeyDown(KeyCode.T))
            {                
                StartPlacingWeapon();
            }
            if (isPlacingWeapon && weaponPlaceholder != null)
            {
                weaponPlaceholder.SetActive(true);
                if (Input.GetKeyUp(KeyCode.T)) 
                {
                    EndPlacingWeapon();
                }

                if (Input.GetKeyDown(KeyCode.Y))
                {
                    weaponPrefab.SetActive(true);
                    placementEffect.SetActive(true);
                    placementEffect.Play();
                    placementEffect.transform.SetParent(null);
                    
                    weaponPrefab.transform.SetParent(null);
                    EndPlacingWeapon();
                    isPlacingWeapon = false;
                    Destroy(weaponPlaceholder);
                    
                }
            }*/
        }

        private void StartPlacingWeapon()
        {
            
            //if (weaponPlaceholder == null)
            //{
                isPlacingWeapon = true;
                //weaponPlaceholder = Instantiate(new GameObject("WeaponPlaceholder"), Vector3.zero, Quaternion.identity);
            //} 
            
        }

        private void EndPlacingWeapon()
        {
            isPlacingWeapon = false;
            weaponPlaceholder.SetActive(false);
            //Destroy(weaponPlaceholder);
        }
    }
}
