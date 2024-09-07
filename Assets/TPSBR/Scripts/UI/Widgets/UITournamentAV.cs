
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using TPSBR.UI;
using System.Collections.Generic;

#pragma warning disable 4014

namespace TPSBR
{
    public class UITournamentAV : UIView
	{
       
        [SerializeField]
        private TextMeshProUGUI _name;
      
        //public UITournamenListHandler listHandler;

        private List<TournamentDetail> _TournamentInfo = new List<TournamentDetail>(32);
        private UITournamentListAV _TournamentList;

        
        
        public UITournamentListedItemAV tournamentDetail;

        public Button testBt;

        protected override void OnInitialize(){

            DataManager.OnTLUNser_response += ListRes;

             CryptoManager.OnUserData += startSection;

            //testBt.onClick.AddListener(mainList);
            _TournamentList = GetComponentInChildren<UITournamentListAV>();
            _TournamentList.UpdateContent += SetListRes;
          
            _TournamentList._allowSelection = false;
          
            _TournamentList.SelectionChanged += OnSelectionChanged;
  
        }

          protected override void OnDeinitialize()
        {
            base.OnDeinitialize();
            DataManager.OnTLUNser_response -= ListRes;
            CryptoManager.OnUserData -= startSection;
            _TournamentList.UpdateContent -= SetListRes;
            _TournamentList.SelectionChanged -= OnSelectionChanged;

        }

         public void startSection(UserData res){
              
               mainList();
        }

        

        private void OnSelectionChanged(int index)
		{
			Debug.Log(index + " index");
            Context.SelectedTournamentDetail = _TournamentInfo[index];

            
			Switch<UITournamentDetail>();
			//selectionChangedList?.Invoke();
		}

        //protected override void OnInitialize()
		//{
		//	base.OnInitialize();
           
        
        //}

        public void mainList(){
//            Debug.Log("iniciando torneos disponibles list" + Context.PlayerData.userData._id);
            if(Context.PlayerData.userData!=null){
                DataManager.instance.tlistUNRequest("active",Context.PlayerData.userData._id,false,true);
            }
            
        }

        private void SetListRes(int index, UITournamentListedItemAV content){
             content.SetData(_TournamentInfo[index]);
            //

        }
         private void ListRes(Tournaments  tournaments){

                //Debug.Log("recibio llita de eventos");

                if(tournaments.tournaments.Length==0){
                    this.SetActive(false);
                    return;
                }

            	for (int i = 0; i < tournaments.tournaments.Length; i++)
			{
                TournamentDetail tournament = tournaments.tournaments[i];

				_TournamentInfo.Add(tournament);
			}


            _TournamentList.Refresh(tournaments.tournaments.Length);
            //Debug.Log(res.tournaments[0].name);
            _TournamentList._allowSelection = true;
        }

       
        
    }
}
