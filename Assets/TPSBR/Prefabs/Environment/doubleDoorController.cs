using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fusion;

public class doubleDoorController : NetworkBehaviour
{
    
    [Networked] public int openStatus {get;set;} = 0;
    // 0 closed
    // 1 Openning

    public bool invert;


    [SerializeField]
    private float distanceX = .9f;
    [SerializeField]
    private float distanceY = 0f;
    [SerializeField]
    private float distanceZ = 0f;
    public float speed = 1f;
    public float waitTime = 3f;

    public Transform doorLeft;
    public Transform doorRight;
    
    public AudioClip soundEffect;
    public AudioClip soundClose;
    public AudioSource source;

    [SerializeField] ParticleSystem effect1;
    [SerializeField] ParticleSystem effect2;


    private Transform orPos;

    public override void Spawned(){
        // Invert dor displacement 
        //distance = invert ? distance : distance *= -1;
        orPos = this.transform;
    }

    void OnTriggerStay(Collider other) {
       // Debug.Log("opennign door");
        if(other.gameObject.tag=="Player"){
            StartInteraction();
        }
         
    }

    public void StartInteraction(){
        if(openStatus==0){
            openDoors();
        }
    }

    private void openDoors(){
        if(effect1!=null) { effect1.Play();}
        if(effect2!=null) { effect2.Play();}
        
        openStatus = 1;
        source.PlayOneShot(soundEffect);
        doorLeft.DOLocalMove(new Vector3(distanceX,distanceY,distanceZ),speed).SetEase(Ease.InSine).OnComplete(()=>{
                StartCoroutine(waitingTime());
            });

        doorRight.DOLocalMove(new Vector3(distanceX*-1,distanceY*-1,distanceZ*-1), speed).SetEase(Ease.InSine).OnComplete(()=>{
            });
    }
    private void closeDoors(){
        doorLeft.DOLocalMove(new Vector3(0,0,0), speed).SetEase(Ease.OutSine).OnComplete(()=>{
                openStatus = 0;
                source.PlayOneShot(soundClose);
            });

        doorRight.DOLocalMove(new Vector3(0,0,0),speed).SetEase(Ease.OutSine).OnComplete(()=>{
            });
    }


    IEnumerator waitingTime(){       
        yield return  new WaitForSeconds(waitTime);
        closeDoors();
    }

 
}

