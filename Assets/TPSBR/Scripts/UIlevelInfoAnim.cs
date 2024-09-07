using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace TPSBR.UI
{
    public class UIlevelInfoAnim : ContextBehaviour
    {
       [SerializeField]
        private TextMeshProUGUI XPdesc;

        [SerializeField]
        private TextMeshProUGUI XPtext;

        [SerializeField]
        private TextMeshProUGUI LVLdesc;
     
        [SerializeField]
		private Image levelProgress;

        [SerializeField]
		private GameObject LevelParent;

        [SerializeField]
		private ParticleSystem levelUpEffect;

        [SerializeField]
		private AudioEffect _audioEffect;
        [SerializeField]
		private AudioSetup _levelUpSound;


        void Awake(){
           
        }

        public void StartAnim()
        {
 
        }
         public void OnDestroy(){
        
        }

         private float currentPerc=0;
        public void setLevelC(int addAmount){
               
            LevelParent.SetActive(true);
            var exp = 5750;
            if(Global.PlayerService.PlayerData.userData!=null){
                exp = Global.PlayerService.PlayerData.userData.userExp;
            }

            var totalToAdd =  addAmount;


            // current level ---------------------------------------------------------
            int level = levelByExp((float)exp);
            var levelTotalNext = requeiredExpforNext(level+1);  // EXP FOR NEXT LEVEL 4444
            var levelExpNext = requeiredExpforNext(level+1) - exp;  // EXP TO LEVEL UP 
            var remaining = requeiredExpforNext(level+1) - levelExpNext ;  //  

            Debug.Log(exp + " exp");
            Debug.Log(levelTotalNext + " levelTotalExpNext");
            Debug.Log(levelExpNext + " levelExpNext");
            Debug.Log(remaining + " remaining");

            var fillAmount = 100- (levelExpNext  * 100  / levelTotalNext )  ;
            //Debug.Log(fillAmount + " Fill mount");
            XPdesc.SetText( exp.ToString("#,##0") +  " / " + levelTotalNext.ToString("#,##0") );
            LVLdesc.SetText(level.ToString());
            levelProgress.fillAmount = (float)fillAmount/100;
            //XPtext.SetText((exp.ToString("#,##0")));
            
            //-----------------------------------------------------------------------
            StartCoroutine(AddExp(totalToAdd,exp));

        }

         IEnumerator AddExp(float totalAmount, float  currentExp){

            Debug.Log(totalAmount + " totalAmount" + currentExp + " currentExp") ;

            while(totalAmount>0){

                int level = levelByExp((float)currentExp);

                var levelTotalNext = requeiredExpforNext(level+1);  // EXP FOR NEXT LEVEL 4444
                var levelExpNext = requeiredExpforNext(level+1) - currentExp;  // EXP TO LEVEL UP 
                var remaining = requeiredExpforNext(level+1) - levelExpNext ;  //  
                // Subtract from total amount
                var toAdd = 0f;
                if(totalAmount>levelExpNext){
                    toAdd =  levelExpNext;
                    totalAmount -= levelExpNext;

                }else{
                    toAdd = totalAmount;
                    totalAmount = 0;
                }
                // Add to current exp
                var initialValue = currentExp;
                var finalValue = currentExp + toAdd;

                var finalDif = levelTotalNext - initialValue;




                //Debug.Log(initialValue + " initialValue" + finalValue + " finalValue") ;
                DOTween.To(() => initialValue, x => initialValue = x, finalValue, 3).SetEase(Ease.OutQuad)
                .SetDelay(0)
                .OnStart(()=> {

                })
                .OnUpdate(() => {
                    var currFill = finalDif - (levelTotalNext  -initialValue);
                    //Debug.Log(currFill + " currFill" + finalDif + " finalDif");

                   var fillAmount =  (currFill * 100  / finalDif ) ;
                    levelProgress.fillAmount = (float)fillAmount/100;
                     XPdesc.SetText( initialValue.ToString("#,##0") +  " / " + levelTotalNext.ToString("#,##0") );
            
                    //Debug.Log(number);
                })
                .OnComplete(()=>{
                    var oldLevel = level;
                    currentExp += toAdd;
                    int leveln = levelByExp((float)currentExp+1);
                    LVLdesc.SetText(level.ToString());
                    Debug.Log(oldLevel + " oldLevel" + leveln + " leveln");

                    if(leveln>oldLevel){
                        levelUpEffect.Emit(50);
                        Debug.Log("LEVEL UP ANIM");
                        _audioEffect.Play(_levelUpSound, EForceBehaviour.ForceAny);
                    }
                   
                    //DOTween.To(() => cgValue, x => cgValue = x, 0, 1).SetEase(Ease.OutQuad);
                });
                 yield return new WaitForSeconds(3.1f);
                
            }
          
        }

        private int levelByExp(float exp){
            //return (int)Mathf.Round(0.3f * (exp*exp)) ;
            Debug.Log("EXP " + exp);
            return (int)Mathf.Floor((0.03f * Mathf.Sqrt(exp)));
        }

        private float requeiredExpforNext(int level){
            return (float)Mathf.Pow(level/0.03f,2);
        }
    }
}
