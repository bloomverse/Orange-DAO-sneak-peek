using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    [CreateAssetMenu(menuName ="Prizes/SpinWheel")]
    public class Prize_SO : ScriptableObject
    {
        public Material image;
        public string description;
        public string longdescription;
        public Color color;
        public float Probability = 1;        
    }
}
