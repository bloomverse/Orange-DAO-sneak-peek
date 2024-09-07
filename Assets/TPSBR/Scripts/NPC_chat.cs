using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace TPSBR
{
    public class NPC_chat : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
		private TextMeshProUGUI chatText;

     
        [SerializeField]
		private CanvasGroup bubbleContainer;
		private RectTransform rectTransform;
		private  Rect rect;

        public Image image;


        [SerializeField]
        public string id="NPC";



        void Start()
        {
        
        }

     
     
        public void setChatText(string text){
           


			

                 chatText.text = text;
            // RectTransform objectRectTransform = background.GetComponent<RectTransform>();
           // objectRectTransform.sizeDelta = chatText.rectTransform;
        }

        public void suscribeGPT(string token, string user){
             chatText.text = "...";
             bubbleContainer.DOFade(1f,.5f);
				bubbleContainer.DOFade(0f,.5f).SetDelay(1f);
            DataManager.OnGPTNPC += gptRes;
            DataManager.instance.gptRequestNPC(token);
        }

        public void gptRes(string res, string user){
            chatText.text = res;
            	bubbleContainer.DOFade(1f,.5f);
				bubbleContainer.DOFade(0f,.5f).SetDelay(8f);
            DataManager.OnGPTNPC -= gptRes;
        }
    }
}
