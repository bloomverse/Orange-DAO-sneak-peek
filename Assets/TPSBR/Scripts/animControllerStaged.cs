using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class animControllerStaged : MonoBehaviour
    {

        [SerializeField]
        private GameObject effect1;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(waitSeconds(3));
        }

        // Update is called once per frame
        void Update()
        {
        
        }

         IEnumerator waitSeconds(int sec){
         yield return new WaitForSeconds(sec);
        effect1.transform.SetActive(true);

        }
    }
}
