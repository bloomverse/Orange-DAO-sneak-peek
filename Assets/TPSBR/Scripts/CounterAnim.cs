using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


namespace TPSBR.UI
{
    public class CounterAnim : UIWidget
    {
       [SerializeField]
       public TextMeshProUGUI numberField;

       [SerializeField]
       private TextMeshProUGUI secondaryText;

        [SerializeField]
       private float duration=1f;

       [SerializeField]
       private string pre = "+";

       [SerializeField]
       private int initialValue=0;

       [SerializeField]
       public int finalValue;

       [SerializeField]
		private AudioSetup _Sound;

       private CanvasGroup cg;
       private float cgValue = 0;

       [SerializeField]
		public ParticleSystem[] _particles;
        public int ParticleIndex = 0;
        

        void Start()
        {
            Debug.Log("Starting anim");
            //startAnim();
           cg = GetComponent<CanvasGroup>();
           cg.alpha = 0;
        }

        public event Action animDone;

        public void startAnim(float delay=0,string description="",int _finalValue=0){

            finalValue = _finalValue;
            DOTween.To(() => cgValue, x => cgValue = x, 1, 1).SetEase(Ease.OutQuad);
            
            secondaryText.SetText(description);
            numberField.SetText(pre + initialValue);
            DOTween.To(() => initialValue, x => initialValue = x, finalValue, duration).SetEase(Ease.OutQuad)
            .SetDelay(delay)
            .OnStart(()=> {
               PlaySound(_Sound);
               _particles[ParticleIndex].Play();   
                 
               

            })
            .OnUpdate(() => {
                numberField.SetText(pre + initialValue);
                

                //Debug.Log(number);
            })
            .OnComplete(()=>{
                animDone?.Invoke();
                //DOTween.To(() => cgValue, x => cgValue = x, 0, 1).SetEase(Ease.OutQuad);
            });

           //GameObject.DOScale(1.05f,1).SetLoops(2);
        }

        // Update is called once per frame
        void Update()
        {
             cg.alpha = cgValue;
        }
    }
}
