using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.Networking; 

namespace TPSBR.UI
{
    public class UITournamentListedItemAV : UIBehaviour
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
        // PUBLIC METHODS

        public bool imagesLoaded;
        public TournamentDetail tournamentInfo;

       

        public void SetData(TournamentDetail tournament)
        {
            
            Debug.Log("setting object data ");
            gameObject.SetActive(true);
            logoImage.SetActive(false);
            backgroundImage.SetActive(false);
            if (tournament == null)
                return;

            tournamentInfo = tournament;
            string name = tournament.name;
            string date = tournament.rounds[0].date;
            //determinate which is the last date on array by closed bool parameter
            for (int i = 0; i < tournament.rounds.Length; i++)
            {
                if(tournament.rounds[i].closed == "false"){
                    break;
                }else {
                    Debug.Log("setting date"  + tournament.rounds[i].date);
                    date = tournament.rounds[i].date;
                }
            }


            
            string prize1 = tournament.prizes[0].prize;

            _name.text = name;
            _date.text = date;
            _1prize.text = prize1 + "";

           

             var mongoDate = DateTime.Parse(date);
            DateTime unityDate = DateTime.SpecifyKind(mongoDate, DateTimeKind.Utc);

            endTime = unityDate ;

            
        }



         IEnumerator DownloadImage(string MediaUrl, RawImage image)
        {   
            Debug.Log(MediaUrl +  "media url");
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError) 
                Debug.Log(request.error);
            else
                image.SetActive(true);
                
                image.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
        } 

         void Update()
        {

            if(this.isActiveAndEnabled && !imagesLoaded && tournamentInfo != null){
                 StartCoroutine(DownloadImage(tournamentInfo.background, backgroundImage));
                StartCoroutine(DownloadImage(tournamentInfo.logo, logoImage));
                imagesLoaded = true;
            }
           // if(endTime!=null){
            TimeSpan timeLeft = endTime - DateTime.Now;   
            
            var pre = "First Match in ";
            if(timeLeft.Days > 0){
                pre += "{0} days,";
            }

            if(timeLeft.Hours < 1){
                _date.color = Color.red;
            }
             _date.color = Color.green;
            _date.text = string.Format(pre + " {1:00}:{2:00}:{3:00}", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
          //  }
            
        }

    }
}
