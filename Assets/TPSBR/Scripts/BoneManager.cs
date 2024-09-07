using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class BoneManager : MonoBehaviour
    {
        private Animator _animator;
        public Vector3 jawAngles;
        // Start is called before the first frame update
        void Start()
        {
            _animator = this.gameObject.GetComponent<Animator>();
            jawAngles = _animator.GetBoneTransform(HumanBodyBones.Jaw).localEulerAngles;
        }
    
        void LateUpdate()
        {
            _animator.GetBoneTransform(HumanBodyBones.Jaw).localEulerAngles = jawAngles;
        }
    }
}
