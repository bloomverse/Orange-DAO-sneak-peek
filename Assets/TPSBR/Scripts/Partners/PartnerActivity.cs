using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    [CreateAssetMenu(fileName ="New PartnerActivity", menuName = "Partners/Activity")]
    public class PartnerActivity : ScriptableObject{
       
        public string partnerName;
        public string urlString;

    }
}
