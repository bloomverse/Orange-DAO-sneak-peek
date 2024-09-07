using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace TPSBR
{
    public class bloomieCoinFinal : MonoBehaviour
    {

        [SerializeField]
		private AudioEffect _audioEffect;
        [SerializeField]
		private AudioSetup _cooldown;

        [SerializeField]
		private ParticleSystem _particle;

        [SerializeField]
		private TextMeshProUGUI _bloomieCount;

        private int bloomieCount=0;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        public float xForce = 1f;   
        public float yForce = 1f;   
        public float time = .3f;

        //public Audio audio;

        public void shake(){
            transform.DOKill(true);
            transform.DOShakePosition(time, new Vector3(xForce, yForce, 0f), 10, 90f, false, true);
            //transform.DOShakeScale(time, .6f, 10, 0f, false);
            bloomieCount += 1;
            //transform.DOScale(new Vector3(.69f, .69f, 0f), .15f).SetLoops(1,LoopType.Yoyo);
            _audioEffect.Play(_cooldown, EForceBehaviour.ForceAny);
            //_particle.Play();
            _particle.Emit(10);
//            _bloomieCount.SetText("+" + bloomieCount.ToString());

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
