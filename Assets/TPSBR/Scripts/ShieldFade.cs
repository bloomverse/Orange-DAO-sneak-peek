using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace TPSBR
{
    public class ShieldFade : MonoBehaviour
    {
        
        [SerializeField] private float _time = 0.5f;
        [SerializeField] private Material _shieldMat;
        private float _cAlpha = 1;
        void Start()
        {

            DOTween.To(() => _cAlpha, x => _cAlpha = x, 0, _time).SetEase(Ease.OutQuad);
        }

        // Update is called once per frame
        void Update()
        {
            _shieldMat.SetFloat("_Alpha",_cAlpha);
            if(_cAlpha==0){
                Destroy(this);
            }
        }
    }
}
