
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using TPSBR.UI;
using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using System.Collections;

#pragma warning disable 4014

namespace TPSBR
{
    public class UITournamentON : UIView
	{
       
        [SerializeField]
        private TextMeshProUGUI _name;


        [SerializeField]
        private GameObject _container;

        public DateTime endTime;
        private List<TournamentDetail> _TournamentInfo = new List<TournamentDetail>(32);
        private UITournamentListON _TournamentList;

        private float timeTollerance = 4f;
        

        [SerializeField] GameObject loaderObject;

        protected override void OnInitialize(){
            DataManager.OnTLSUSser_response += ListRes;
            CryptoManager.OnUserData += startSection;
            loaderObject.SetActive(false);
            _TournamentList = GetComponentInChildren<UITournamentListON>();
            _TournamentList.UpdateContent += SetListRes;
            _TournamentList._allowSelection = false;
            _TournamentList.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDeinitialize()
        {
            _container.SetActive(false);
            base.OnDeinitialize();
            DataManager.OnTLSUSser_response -= ListRes;
            CryptoManager.OnUserData -= startSection;
            _TournamentList.UpdateContent -= SetListRes;
            _TournamentList.SelectionChanged -= OnSelectionChanged;
            loaderObject.SetActive(false);

        }

        public void startSection(UserData res){
               mainList();
        }

        private void OnSelectionChanged(int index)
		{

            Debug.Log("index of selection tournament -- " + index);
            if(index!=-1){
                 _TournamentList._allowSelection = false;
            Context.SelectedTournamentDetail = _TournamentInfo[index];

            TournamentRounds currentRound = GlobalFunctions.GetLastRound(Context.SelectedTournamentDetail);
            TournamentMatches currentMatch=null;

            if(currentRound.closed=="true"){
                currentMatch =  GlobalFunctions.GetCurrentMatch(Context.SelectedTournamentDetail,currentRound,Context.PlayerData.userData._id);
                Debug.Log( " match number" + currentMatch.lobby);
            }else{
                currentMatch = null;
            }

            string date = currentRound.date;
			var mongoDate = DateTime.Parse(date);
            DateTime unityDate = DateTime.SpecifyKind(mongoDate, DateTimeKind.Utc);
            endTime = unityDate ;
            TimeSpan timeLeft = endTime - DateTime.Now;  

            if( timeLeft.TotalMinutes<timeTollerance
                &&  timeLeft.TotalMinutes>-5 
                && currentMatch!=null
                && currentMatch.lobby!=null
            ){

            loaderObject.SetActive(true);
                Context.PlayerData.lobbyType = "Tournament";
                //GameManagerWebGL.Instance.LobbyDetailTournament += LobbyDetailTournament;
                GameManagerWebGL.Instance.currentTournamentMatch = currentMatch;
                GameManagerWebGL.Instance.currentTournamentRound = currentRound;
				GameManagerWebGL.Instance.JoinLobbySID(currentMatch.lobby, Context.PlayerData);
			
        }else{
            Switch<UITournamentDetail>();
             _TournamentList._allowSelection = true;
        }
            }
           

       
			
		}

        
        public void mainList(){
              _TournamentList._allowSelection = false;
               loaderObject.SetActive(false);
              Debug.Log("Refresginh tournaments on.....");
                DataManager.instance.tlistSUSRequest("active_ongoing",Context.PlayerData.userData._id,true,false);
        }

        private void SetListRes(int index, UITournamentListedItemON content){
            
             content.SetData(_TournamentInfo[index],Context.PlayerData.userData._id);
        }
        private bool firstLoad;
         private void ListRes(Tournaments  tournaments){
            
            _TournamentList.Clear();
            _TournamentInfo.Clear();

                if(tournaments.tournaments.Length==0){
                    return;
                }

                _container.SetActive(true);
            for (int i = 0; i < tournaments.tournaments.Length; i++)
			{
                TournamentDetail tournament = tournaments.tournaments[i];
				_TournamentInfo.Add(tournament);
			}
            _TournamentList.Refresh(_TournamentInfo.Count);
            _TournamentList._allowSelection = true;
        }

       
        
    }
}
