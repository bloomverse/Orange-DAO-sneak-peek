using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TPSBR.UI;
using DG.Tweening;

namespace TPSBR
{
    public class PrizeManager : ContextBehaviour
    {
        [SerializeField] TextMeshProUGUI announcer;
        [SerializeField] ParticleSystem celebrationEffect;

        private CryptoManager cryptoManager;

         [SerializeField] AudioClip Token;
        [SerializeField] AudioClip FireWorks;


        [SerializeField] ChestPrize chest1;
        [SerializeField] ChestPrize chest2;
        [SerializeField] ChestPrize chest3;

        [SerializeField] GameObject chestButton1;
        [SerializeField] GameObject chestButton2;
        [SerializeField] GameObject chestButton3;
        

        [SerializeField] AudioSource audioSource;

        public Animator _animator;



        void Start()
        {
            announcer.SetText("Claim your prize"); 
        }

        public void OpenChest1(){
               Agent agent = FindAnyObjectByType<Agent>();

            _animator = agent.transform.GetComponentInChildren<Animator>();

            chest1.Open();
            chestButton1.transform.SetActive(false);
            //_animator.CrossFade("celebraton",0.2f);
            _animator.SetBool("emote3", true);
            StartCoroutine(stopCheer());

        }

        IEnumerator stopCheer(){
            yield return new WaitForSeconds(1);
            _animator.SetBool("emote3", false);

        }

        public void OpenChest2(){
            Agent agent = FindAnyObjectByType<Agent>();

            _animator = agent.transform.GetComponentInChildren<Animator>();

            chest2.Open();
            chestButton2.transform.SetActive(false);
            //_animator.CrossFade("celebraton",0.2f);
            _animator.SetBool("emote3", true);
            StartCoroutine(stopCheer());
        }

        public void OpenChest3(){
            chest3.Open();
            chestButton3.transform.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
