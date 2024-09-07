using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;


namespace TPSBR
{
    public class Tooltip : MonoBehaviour
    {
        private static Tooltip instance;

        [SerializeField] Camera uiCamera;

         [SerializeField] float XOffset ;
         [SerializeField] float YOffset;

        private float extraOffset = 0;

        private TextMeshProUGUI tooltipText;
        private RectTransform backgroundRectTransform;

        private void Awake(){

            instance = this;
           // gameObject.SetActive(false);
            backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
            tooltipText = transform.Find("text").GetComponent<TextMeshProUGUI>();

            //showToolTip("Buy Bloomie Bundles");
            hideToolTip();
        }
        private Vector2 mousePosition;

       

       private void Update(){
        mousePosition = Mouse.current.position.ReadValue();

         Vector2 localPoint;
         RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(),mousePosition,uiCamera,out localPoint);
        localPoint.x += XOffset + extraOffset;
        
        localPoint.y += YOffset;
         transform.localPosition = localPoint;
         
       }

       private void showToolTip(string tooltipString,float _XOffSet){
            gameObject.SetActive(true);

            extraOffset = _XOffSet;
            tooltipText.SetText(tooltipString);
            float textPaddingSize = 12f;
            Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth , tooltipText.preferredHeight+ textPaddingSize * 2f);
            backgroundRectTransform.sizeDelta = backgroundSize;
            


       }
       
       private void hideToolTip(){
        gameObject.SetActive(false);
       }

       public static void ShowToolTip_Static(string tooltipString,float _XOffSet=0){
            try{
                    instance.showToolTip(tooltipString,_XOffSet);
            }catch(System.Exception e){
        }
         
       }

       public static void HideToolTip_Static(){
        try{
            instance.hideToolTip();
        }catch(System.Exception e){

        }
          
       }

    }
}
