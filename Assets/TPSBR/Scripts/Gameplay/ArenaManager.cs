
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Plugins;

using Cinemachine;
using TMPro;
using System;


namespace TPSBR
{

public class ArenaManager : MonoBehaviour
{

    public static ArenaManager instance;
    public  CinemachineVirtualCamera virtualCamera;

    [SerializeField] Color original;
    [SerializeField] Color green;
    [SerializeField] Color red;
    [SerializeField] Color white;

    [SerializeField] private AudioClip counterTick;
    [SerializeField] private AudioClip counterDone;

    [SerializeField] private AudioClip minremaining;
    [SerializeField] private AudioClip finalSound;

    public AudioSource source;

    //public static event Action<bool> CountDownDone;


    new Renderer renderer;
    public Material material;
    private bool CountDone;


    void Awake(){
        //if(instance==null){
            instance = this;
       // }
        
     
    }
    
    void Start()
    {
        material.DOColor(original * 5, "_EmissionColor", 0f);

    }

    public TextMeshProUGUI textItem;

    public void startWaiting(string text){
        //textItem.text = text;
        textItem.SetText(text);
    }

     public void StartCountDown(){
        StartCoroutine(CountDownStart());
    }


    public void finalSequence(){
        countdownTime = 60;
         textItem.SetText("1 Minute remaining");
         source.PlayOneShot(minremaining);
         textItem.DOColor(Color.yellow, 1f).SetLoops(12,LoopType.Yoyo);
         textItem.DOFade(0, 1f).SetLoops(20,LoopType.Yoyo);
        StartCoroutine(CountDownEnd());
    }


    public void deactivateCameras(){
        //  virtualCamera.Priority = -10;
        virtualCamera.gameObject.SetActive(false);
    }

   


   [SerializeField] private int countdownTime;


   IEnumerator CountDownStart(){
        

        textItem.DOColor(green, 1f).SetLoops(4,LoopType.Yoyo);
       while(countdownTime > 0){
        textItem.SetText(countdownTime.ToString());
           yield return new WaitForSeconds(1f);

           countdownTime --;
           if(countdownTime<4){
            
               source.PlayOneShot(counterTick);
           }
       }

        textItem.SetText("Go!");
        startSequence();
       
       yield return new WaitForSeconds(1f);
        textItem.SetText("");
   }

   IEnumerator CountDownEnd(){
       while(countdownTime > 0){
        textItem.SetText(countdownTime.ToString());
           yield return new WaitForSeconds(1f);

           countdownTime --;
           if(countdownTime<20){

               
               source.PlayOneShot(counterTick);

               if(!endStarted){
                    textItem.DOColor(red, 1f).SetLoops(18,LoopType.Yoyo);
                    textItem.DOFade(0, 1f).SetLoops(12,LoopType.Yoyo);

                    endSequence();
                    endStarted = true;
               }
                
           }
       }

        textItem.SetText("Match Complete!");
       source.PlayOneShot(finalSound);
       
       yield return new WaitForSeconds(1f);
        textItem.SetText("");
   }

    public void startSequence(){  
       
        Sequence mySequence = DOTween.Sequence(); 
        mySequence.Append(material.DOColor(green * 17, "_EmissionColor", 1f).SetLoops(12,LoopType.Yoyo));
        mySequence.PrependInterval(0);
        source.PlayOneShot(counterDone);
    }

    private bool endStarted;
    public void endSequence(){
         Sequence mySequence = DOTween.Sequence(); 
        mySequence.Append(material.DOColor(red * 17, "_EmissionColor", 1f).SetLoops(12,LoopType.Yoyo));
        mySequence.PrependInterval(0);
        
    }

}

}


