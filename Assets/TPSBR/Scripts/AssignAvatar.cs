using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class AssignAvatar : MonoBehaviour
    {

        public string bodyPartName;
        public float addition;
        public bool adjustRotation;
        public bool pistol;
        public bool gun;
        public bool rocket;
        public bool delay;

        private void Start()
        {
            
        }

        public void DeactivateAll()
        {
            pistol = false;
            rocket = false;
            gun = false;
        }
        private void Update()
        {
            Debug.Log(this.GetComponentInParent<Weapons>().CurrentWeapon.name);
            DeactivateAll();
            if (this.GetComponentInParent<Weapons>().CurrentWeapon.name.Contains("Pistol"))
            {
                pistol = true;
            }
            else if (this.GetComponentInParent<Weapons>().CurrentWeapon.name.Contains("Rocket"))
            {
                rocket = true;
            }
            else if (this.GetComponentInParent<Weapons>().CurrentWeapon.name.Contains("Rifle"))
            {
                gun = true;
            }
            if (delay)
            {
                Invoke("AssignAvatarParts",0.5f);
            }
            else
            {
                AssignAvatarParts();
            }
            
        }

        public void AssignAvatarParts()
        {
            GameObject obj = GameObject.Find(bodyPartName); 
            if(obj == null)
            {
                return;
            }
            delay = false;
            //this.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
            this.transform.SetParent(obj.transform);
            if (adjustRotation && pistol)
            {
                //adjustRotation = false;
                Vector3 rotationAngles = new Vector3(10.793f, -7.862f, -90.059f);
                this.transform.localPosition = new Vector3(0.04592672f, 0.03485007f, 0.01746031f);
                Quaternion newRotation = Quaternion.Euler(rotationAngles);
                transform.localRotation = newRotation;
            }
            if (adjustRotation && rocket)
            {
                //adjustRotation = false;
                Vector3 rotationAngles = new Vector3(11.028f, -20.212f, -99.3f);
                this.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
                Quaternion newRotation = Quaternion.Euler(rotationAngles);
                transform.localRotation = newRotation;
            }
        }
    }
}
