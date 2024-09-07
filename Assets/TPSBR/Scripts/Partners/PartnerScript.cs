using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class PartnerScript : MonoBehaviour
    {
        
        [SerializeField] PartnerActivity partnerActivity;

        void Start()
        {
            Debug.Log(partnerActivity.partnerName);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
