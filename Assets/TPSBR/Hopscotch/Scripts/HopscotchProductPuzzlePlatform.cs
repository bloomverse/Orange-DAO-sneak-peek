using System.Collections;
using UnityEngine;

namespace TPSBR
{
    public class HopscotchProductPuzzlePlatform : MonoBehaviour
    {
        [SerializeField] private HopscotchProductPuzzle m_ProductPuzzle;
        [SerializeField] private Rigidbody m_Rigidbody;

        private bool m_Correct = false;

        public void Init(bool a_Correct)
        {
            m_Correct = a_Correct;

            GetComponent<BoxCollider>().enabled = m_Correct;
        }
    }
}
