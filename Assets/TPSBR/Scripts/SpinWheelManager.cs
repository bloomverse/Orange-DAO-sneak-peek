using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using TPSBR.UI;
using Fusion;

namespace TPSBR
{
    public enum WheelStatus {
        IDLE,
        SPINNING,
        STOP
    }

    public class SpinWheelManager : ContextBehaviour
    {
        [SerializeField] private Rigidbody rb;
        private WheelStatus _status = WheelStatus.IDLE;
        [SerializeField] private LayerMask sliceLayerMask;
        [SerializeField] Transform leverObj;

        [SerializeField] Prize_SO[] prizes;

         [SerializeField] UserLocalInteraction interaction;

        public Transform origin; // The origin transform
        public Transform[] targets; // Array of target transforms


        public float checkInterval = 0.01f;
        private float checkTimer;
        private Transform closestTarget;
        private Transform previousClosestTarget; // Added to track the previously closest target
        private float spinStartDelay = 2f; // Delay in seconds before starting to check if the wheel has stopped
        private float spinStartTime = 0; // Time when the last spin started

        // Bulb Sequence
        public GameObject[] lightBulbs; // Assign your light bulb prefabs here
        public float duration = 2f; // Duration for the fade in and out
        public float delayBetweenBulbs = 0.5f; // Delay between each bulb's animation start
        private float minEmission = 0f; // Min emission intensity (for fade out)
        private float maxEmission = 19f; 

        private string typeActivity = "eyekonwheel";

        [SerializeField] AudioEffect audioEffect;
        [SerializeField] AudioEffect audioEffect2;
        [SerializeField]
		private AudioSetup _leverSound;
        [SerializeField]
		private AudioSetup _spinSound;
        [SerializeField]
		private AudioSetup _winSound;

        public Animator _animator;

        void Start(){
             DOTween.Init(); // Initialize DOTween
            
        }
         
        public override void Spawned(){

           // PlayerPrefs.DeleteAll();
            
             //AssignPrizes();
          
             getPrizesDB();
             StartCoroutine(FadeSequence());
             DataManager.OnActivitiesFetched += getStateClaimable;

          /*  if(Context.PlayerData.userData==null){
                Debug.Log("ASKING SIGN");
                 interaction.askSignin(false);
                 CryptoManager.OnUserData += changeSignState;
                //interaction.SetActive(true);
            }else{
              interaction.askSignin(true) ;
              DataManager.instance.GeLastActivitiesForUser(typeActivity,1);
              DataManager.OnActivitiesFetched += getStateClaimable;
            }*/
              

             //bool valid = getStateClaimable();
            //Debug.Log("Valid "+ valid);
            
        }

        private void changeSignState(UserData userData){
            Debug.Log("State changed sign in ");
            interaction.askSignin(true) ;
            CryptoManager.OnUserData  -= changeSignState;
             DataManager.instance.GeLastActivitiesForUser(typeActivity,1);
              
        }

        private void getPrizesDB(){
            DataManager.OnSpinResponse += AssignPrizes;
            DataManager.instance.getSpin();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            //bool valid = getStateClaimable();
            //Debug.Log("Valid "+ valid);
            /* if(!valid){                
                interaction.setState(false,GetFormattedRemainingTime());
             }else{
                interaction.setState(true,GetFormattedRemainingTime());
             }*/

        }

        private void  getStateClaimable(Activities activities,string type){

           if(type==typeActivity){
                if(activities.activityData.Length>0 ){
                   ActivityData activityData = activities.activityData[0];
                DateTime lastClaimedTime = activityData.GetDateTime();
                /*if (!DateTime.TryParse(PlayerPrefs.GetString("LastClaimDateWheel2", ""), out lastClaimedTime))
                {
                    return true;
                }*/

                getRemaining(lastClaimedTime);

                DateTime now = DateTime.Now;
                double hoursSinceLastClaim = (now - lastClaimedTime).TotalHours;
                bool valid = hoursSinceLastClaim >= 3; 

                Debug.Log("Time valid " + valid);
                Debug.Log(hoursSinceLastClaim + " Hour since" + lastClaimedTime + " -- " + activityData.ts +  " -- " + activityData.name);

                if(!valid){
                    interaction.setState(false,GetFormattedRemainingTime(lastClaimedTime));
                    //leverObj.DORotate(new Vector3(45,0,0),1,RotateMode.LocalAxisAdd).SetEase(Ease.InOutBack);//.SetLoops(1,LoopType.Yoyo);
                    
                }else{
                    interaction.setState(true,GetFormattedRemainingTime(lastClaimedTime));
                    interaction.SetActive(true);
                }
            }else{
                interaction.setState(true,DateTime.Now.ToString());
                interaction.SetActive(true);
            }
           }
            

         
        }

        public static void SaveLastClaimDate(string internalId)
        {
            DateTime now = DateTime.Now;
            //PlayerPrefs.SetString("LastClaimDateWheel2", now.ToString("o")); // "o" is the round-trip date/time pattern.
            //PlayerPrefs.Save();
            ActivityData ad = new ActivityData();
            ad.type = "eyekonwheel";
            ad.name = "Eyekon Wheel Spin";
            ad.description = internalId;
            DataManager.instance.AddActivityStart(ad);

        }
          public static double getRemaining(DateTime lastClaimedTime){
            //DateTime lastClaimedTime;
            /*if (!DateTime.TryParse(PlayerPrefs.GetString("LastClaimDateWheel2", ""), out lastClaimedTime))
            {
                return 0; // If there's no stored time, a prize can be claimed immediately.
            }*/

            DateTime now = DateTime.Now;
            double hoursSinceLastClaim = (now - lastClaimedTime).TotalHours;
            if (hoursSinceLastClaim >= 3) return 0; // A prize can be claimed now.

            return 3 - hoursSinceLastClaim; // Hours remaining until the next prize can be claimed.
        }

        public static string GetFormattedRemainingTime(DateTime lastClaimedDate){
            double remainingHours = getRemaining(lastClaimedDate);
            if (remainingHours <= 0)
            {
                return "Spin now!";
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromHours(remainingHours);
                return string.Format("{0} H and {1} M", 
                                    (int)timeSpan.TotalHours, 
                                    timeSpan.Minutes);
            }
        }


       void AssignPrizes(SpinPrizes prizes)
        {
        //Debug.Log("Assigning prizes " +  prizes.SpinArr);
        //if(prizes.SpinArr.Length == 0) return; // Exit if no prizes
        int cIndex = 0;

        foreach (Transform target in targets)
        {
            var currentPrize = prizes.SpinArr[cIndex];

            SpinPrizeDesc spinPrizeDesc = target.GetComponentInChildren<SpinPrizeDesc>();
//            Debug.Log(currentPrize.item_3d);
            Material loadedMaterial = Resources.Load<Material>("Materials/" + currentPrize.item_3d);
            if (loadedMaterial != null)
            {
                spinPrizeDesc.prizeMat.GetComponent<Renderer>().material = loadedMaterial;
            }else
            {
                Debug.LogError("Material not found: " + currentPrize.item_3d);
            }
            
            spinPrizeDesc.description.SetText(currentPrize.name);
            spinPrizeDesc.longDescription = currentPrize.description;
            spinPrizeDesc.prizeId = currentPrize.internalId;
            cIndex++;
        }
    }

       void Update()
            {
            if (_status == WheelStatus.SPINNING)
            {
                checkTimer += Time.deltaTime;

                if (checkTimer >= checkInterval)
                {
                
                    checkTimer = 0;
                }

                FindClosestTarget();
//                Debug.Log("magnitude " +  rb.angularVelocity.magnitude);
                audioEffect2.AudioSource.pitch = rb.angularVelocity.magnitude/2;
                // Check if it's time to start checking for the stop condition
                if (Time.time - spinStartTime > spinStartDelay)
                {
                    // Now check the angular velocity to decide if we should stop
                    if (rb.angularVelocity.magnitude < 0.01f) // Adjust this threshold as needed
                    {
                        if (_status != WheelStatus.STOP) // Check if the status hasn't already been set to STOP
                        {
                            _status = WheelStatus.STOP;
                            HighlightPrize();
                            audioEffect2.Stop();
                            audioEffect.Play(_winSound, EForceBehaviour.ForceAny);
                        }
                    }
                }
            }
        }

        void FindClosestTarget()
        {
            if (origin == null)
            {
                Debug.LogError("Origin transform is not assigned.");
                return;
            }

            if (targets == null || targets.Length == 0)
            {
                Debug.LogError("Targets array is empty.");
                return;
            }

            float minDistance = Mathf.Infinity; // Initialize with a very high value
            Transform newClosestTarget = null; // Use a local variable to find the new closest target

            // Iterate through each target transform
            foreach (Transform target in targets)
            {
                if (target != null)
                {
                    // Calculate the distance to the target transform from the origin
                    float distance = Vector3.Distance(origin.position, target.position);

                    // Check if the current distance is less than the minimum distance found so far
                    if (distance < minDistance)
                    {
                        minDistance = distance; // Update the minimum distance
                        newClosestTarget = target; // Update the new closest target
                    }
                }
                else
                {
                    Debug.LogError("One of the target transforms is not assigned.");
                }
            }

            // Check if the closest target has changed
            if (newClosestTarget != closestTarget)
            {
                if (closestTarget != null)
                {
                    // Revert the previous closest target to its original state
                    SetNormalState(closestTarget);
                }

                closestTarget = newClosestTarget; // Update the closest target

                if (closestTarget != null)
                {
                    // Highlight the new closest target
                    SetHighlightState(closestTarget);
                }
            }
        }

        void SetHighlightState(Transform target)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", Color.white); // Set to highlighted color
            }
        }

        void SetNormalState(Transform target)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.SetColor("_EmissionColor", Color.black); // Assuming black is the normal state
                renderer.material.DisableKeyword("_EMISSION");
            }
        }

        void HighlightPrize()
        {
            if (closestTarget != null)
            {
                Renderer renderer = closestTarget.GetComponent<Renderer>();
                if (renderer != null)
                {
                    
                    Color targetEmissionColor = Color.white * 5f; // Prize glow color

                    renderer.material.EnableKeyword("_EMISSION");

                    // Glow in and out 8 times
                    DOTween.To(() => renderer.material.GetColor("_EmissionColor"), 
                            x => renderer.material.SetColor("_EmissionColor", x), 
                            targetEmissionColor, 0.5f)
                        .SetLoops(16, LoopType.Yoyo); // 16 loops because each in and out is considered 2 loops
                }
                _status = WheelStatus.STOP;

                UIGameplayMenu dialog = GameObject.Find("UIGameplayMenuView").GetComponent<UIGameplayMenu>();
                var desc = closestTarget.GetComponentInChildren<SpinPrizeDesc>().longDescription;
                var internalId = closestTarget.GetComponentInChildren<SpinPrizeDesc>().prizeId;

                DataManager.spinClaimedReturn += claimSpinConfirm;
			    DataManager.instance.spinPrize(Context.PlayerData.DBID,internalId,Context.PlayerData.Nickname,desc);

                dialog.Notification(desc.ToString(),internalId+ "BV");

                  Agent agent = FindAnyObjectByType<Agent>();

            _animator = agent.transform.GetComponentInChildren<Animator>();

       
            //_animator.CrossFade("celebraton",0.2f);
            _animator.SetBool("emote3", true);

             // Save activity to database
             SaveLastClaimDate(internalId);

            StartCoroutine(stopCheer());
               // audioEffect2.Stop();
            }
        }

        void claimSpinConfirm(string res){

        }

         IEnumerator stopCheer(){
            yield return new WaitForSeconds(1);
            _animator.SetBool("emote3", false);

        }

        public void SpinOnce()
        {
            leverObj.DORotate(new Vector3(45,0,0),1,RotateMode.LocalAxisAdd).SetEase(Ease.InOutBack);//.SetLoops(1,LoopType.Yoyo);
            audioEffect.Play(_leverSound, EForceBehaviour.ForceAny);
            int randomNumber = new System.Random().Next(800, 1400 + 1);
            Debug.Log("Spinning" + randomNumber);
            Vector3 localTorque = new Vector3(0, 0, randomNumber*-1);
            Vector3 globalTorque = rb.transform.TransformDirection(localTorque);
            rb.AddTorque(globalTorque);
            _status = WheelStatus.SPINNING;
            spinStartTime = Time.time;
            audioEffect2.Play(_spinSound, EForceBehaviour.ForceAny);
           
        }

   private IEnumerator FadeSequence()
    {
        foreach (var bulb in lightBulbs)
        {
            Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
            Renderer renderer = bulb.GetComponent<Renderer>();
            Material mat = renderer.material;
            mat.DOColor(randomColor * maxEmission, "_EmissionColor", duration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(duration);
            mat.DOColor(randomColor * minEmission, "_EmissionColor", duration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(delayBetweenBulbs);
        }
         StartCoroutine(FadeSequence());
    }

    }
}
