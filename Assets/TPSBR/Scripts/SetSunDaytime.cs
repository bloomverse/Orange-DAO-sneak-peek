
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SetSunDaytime : MonoBehaviour {
    
   void Start(){

         float hourAngle = 360f / 24f;
        DateTime currenthour = System.DateTime.Now;
        Debug.Log(currenthour.Hour + " current hour " + hourAngle) ;
         transform.Rotate(transform.right * (360- (currenthour.Hour * hourAngle)), Space.World);
         Shader.SetGlobalMatrix("_MainLightMatrix", transform.localToWorldMatrix);
   }

}
