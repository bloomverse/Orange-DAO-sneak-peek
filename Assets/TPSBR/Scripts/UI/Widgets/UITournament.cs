using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using TPSBR.UI;
using System.Collections.Generic;

#pragma warning disable 4014

namespace TPSBR
{
    public class UITournament : UICloseView
	{
       
        [SerializeField]
        private TextMeshProUGUI _name;
      
        //public UITournamenListHandler listHandler;

        private List<TournamentDetail> _TournamentInfo = new List<TournamentDetail>(32);
        private UITournamentList _TournamentList;

        public UITournamentListedItem tournamentDetail;

        [SerializeField] private UIButton _onBackButton;
        

        public Button testBt;

        protected override void OnOpen(){
            DataManager.OnTLAllresponse += ListRes;

            _TournamentList.UpdateContent += SetListRes;
             _TournamentList._allowSelection = false;
            _TournamentList.SelectionChanged += OnSelectionChanged;
            mainList();
            //testBt.onClick.AddListener(mainList);
            _onBackButton.onClick.AddListener(OnBackScreen);
        }


         private void OnSelectionChanged(int index)
		{
			Debug.Log(index + " index");
            if(index!= -1){
                Context.SelectedTournamentDetail = _TournamentInfo[index];
			    Switch<UITournamentDetail>();
                _TournamentList.Clear();
            }
            
			//selectionChangedList?.Invoke();
		}

        protected override void OnInitialize()
		{
			base.OnInitialize();
            _TournamentList = GetComponentInChildren<UITournamentList>();
        
        }

        public void mainList(){
            DataManager.instance.tlistAllRequest("active","");
        }

        private void SetListRes(int index, UITournamentListedItem content){
             content.SetData(_TournamentInfo[index]);
           

        }

          protected override void OnClose()
		{
			
			//base.OnClose();
            Open<UIMainMenuView>();
			//Context.PlayerPreview._animator.SetBool("Aim",false);
		}
        private void OnBackScreen(){
            Debug.Log("openning main from ui lis 2 t");
			Switch<UIMainMenuView>();
		}
        // protected override bool OnBackAction()
		//{
			//Debug.Log("openning main from ui list");
			//Open<UIMainMenuView>();
			//return true;
		//}


         private void ListRes(Tournaments  tournaments){

            

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
