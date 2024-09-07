using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class UILookAt : MonoBehaviour
    {

        public bool invert= false;
        public float multiply = .120f;

        void Start(){
            if(invert==true){
                multiply =  -multiply;
            }
        }


        void LateUpdate()
        {
          // Debug.Log("rotation");
            //transform.LookAt(transform.position +  Camera.main.transform.rotation* Vector3.forward * multiply,Camera.main.transform.rotation * Vector3.up);

            if (Camera.main.transform != null)
            {
                // Calculate the direction from the character to the camera.
                Vector3 lookDirection = Camera.main.transform.position - transform.position;

                // Make sure the character doesn't tilt upwards or downwards when rotating.
                lookDirection.y = 0;

                // Use Quaternion.LookRotation to smoothly rotate the character towards the camera.
                Quaternion rotation = Quaternion.LookRotation(lookDirection);

                // Apply the rotation to the character.
                transform.rotation = rotation;
                transform.Rotate(Vector3.up, 180f);
                
            }
            else
            {
                Debug.LogError("Camera transform is null. Make sure you have a camera in the scene with the correct tag or provide the camera transform in a different way.");
            }

        }
    }
}
