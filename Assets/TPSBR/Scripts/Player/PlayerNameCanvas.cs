using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TPSBR
{
    public class PlayerNameCanvas : MonoBehaviour
    {


        // Update is called once per frame
        void Update()
        {
//            Debug.Log("pasando");
          
                Camera camera = Camera.main;
                transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
              //  _nameText.transform.Rotate(0,180,0);
           
        }
    }
}