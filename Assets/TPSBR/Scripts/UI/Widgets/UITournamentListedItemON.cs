using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.Networking;
using MoreMountains.Feedbacks;

namespace TPSBR.UI
{
    public class UITournamentListedItemON : UIBehaviour
	{
        // PRIVATE MEMBERS
        private MapSettings MapSettings;

        [SerializeField]
        private TextMeshProUGUI _name;
        [SerializeField]
        private TextMeshProUGUI _date;
        [SerializeField]
        private TextMeshProUGUI _1prize;

         [SerializeField]
        private RawImage backgroundImage;

        [SerializeField]
        private RawImage logoImage;
       
        public DateTime endTime;

        [SerializeField]
        private MMF_Player mmFeedback;
        // PUBLIC METHODS

        public bool imagesLoaded;
        public TournamentDetail tournamentInfo;

        public TournamentRounds currentRound;
        public TournamentMatches currentMatch;

        public void SetData(TournamentDetail tournament,string userId)
        {
            currentRound = GlobalFunctions.GetLastRound(tournament);
            
            Debug.Log(currentRound.closed + " Round state");
            if(currentRound.closed=="true"){
                currentMatch =  GlobalFunctions.GetCurrentMatch(tournament,currentRound,userId);
                Debug.Log( " match number" + currentMatch.lobby);
            }else{
                currentMatch = null;
            }
            
            Debug.Log(currentMatch + " Current match");
            
            gameObject.SetActive(true);
            logoImage.SetActive(false);
            backgroundImage.SetActive(false);

            if (tournament == null)
                return;

            tournamentInfo = tournament;
            string name = tournament.name + " - Round " + currentRound.number;
            string date = currentRound.date;
          
            string prize1 = tournament.prizes[0].prize;

            _name.text = name;
            _date.text = date;
            _1prize.text = prize1 + "";

            var mongoDate = DateTime.Parse(date);
            DateTime unityDate = DateTime.SpecifyKind(mongoDate, DateTimeKind.Utc);
            endTime = unityDate ;
            imagesLoaded = false;
        }

         void Update()
        {
             if(this.isActiveAndEnabled && !imagesLoaded && tournamentInfo != null){
                StartCoroutine(DownloadImage(tournamentInfo.background, backgroundImage));
                StartCoroutine(DownloadImage(tournamentInfo.logo, logoImage));
                imagesLoaded = true;
            }
            
            TimeSpan timeLeft = endTime - DateTime.Now;   
            //Debug.Log(currentMatch);
            var pre = "";
             _date.text = string.Format(pre + " {1:00}:{2:00}:{3:00}", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);

            if(timeLeft.Days > 0){
                pre += "{0} days,";
            }

            if(timeLeft.Hours < 1){
                _date.color = Color.yellow;
            }

            if(timeLeft.Hours < 1 && timeLeft.Minutes<15){
                _date.color = Color.red;
            }

            if( timeLeft.Hours < 1 && timeLeft.Minutes<4 && timeLeft.Minutes>-9 &&  currentMatch!=null ){
                _date.color = Color.red;
                _date.text = "MATCH READY!";
                mmFeedback.PlayFeedbacks();
            }

             if(timeLeft.TotalMinutes<-5){
                _date.color = Color.white;
                _date.text = "MATCH FINISHED!";
            }

            
            
        }

         IEnumerator DownloadImage(string MediaUrl, RawImage image)
        {   
//            Debug.Log(MediaUrl +  "media url");
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError) 
                Debug.Log(request.error);
            else
                image.SetActive(true);
                image.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
        } 

       

    }

    
}
