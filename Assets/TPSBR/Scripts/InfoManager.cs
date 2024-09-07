using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class InfoManager : MonoBehaviour
{
     private bool active;
     private float timeActive= 2f;
     private float timeAct;

     public static InfoManager instance = null;
     public GameObject UIGO;
     private Label labelSection;
     private  VisualElement root;


    private void Awake() {
        
    }

    void Start()
    {
        instance = this;
       
    }


    public void Update(){
        if(active){
            timeAct -= Time.deltaTime ;
            if(timeAct<=0){
                Debug.Log("deactivantdo");
                active = false;
                Deactivate();
                
            }
        }
    }

    public void Activate(string text){
         UIGO.SetActive(true);
         root = UIGO.gameObject.GetComponent<UIDocument>().rootVisualElement;
          labelSection = root.Q<Label>("info");
        labelSection.text = text;

        if(!active){
            timeAct = timeActive;
            active= true;
            
            //logo.transform.DOScale(1,1).SetEase(Ease.OutBack);
            //backEffect.transform.DOScale(1,1).SetEase(Ease.OutBack);
            
            
        }else{
            timeAct = timeActive;
        }
        
        
    }

     public void Deactivate(){
        if(!active){
            UIGO.SetActive(false);
            //logo.transform.DOScale(0,1).SetEase(Ease.InBounce);
       // backEffect.transform.DOScale(0,1).SetEase(Ease.InBounce).OnComplete(()=>{
           // logo.SetActive(false);
           // backEffect.SetActive(false);
       // });
        }
       
    }
}
