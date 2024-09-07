using System;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace TPSBR.UI
{
	public class UINFTSelectionSkin : UICloseView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private UIList _skinList;
		[SerializeField]
		private UIButton _selectButton;
		[SerializeField]
		private TextMeshProUGUI _skinName;
		[SerializeField]
		
		private TextMeshProUGUI _skinRarity;
		[SerializeField]
		private string _skinNameFormat = "{0}";
		[SerializeField]
		private UIBehaviour _selectedSkinGroup;
		[SerializeField]
		private UIBehaviour _selectedEffect;
		[SerializeField]
		private AudioSetup _selectedSound;
		[SerializeField]
		private float _closeDelayAfterSelection = 0.5f;

		private string _previewSkin;

		private NFT[]  nfts = new NFT[0] ;

		[SerializeField]
		private CryptoManager cryptoManager;

		[SerializeField]
		private GameObject walletAccess;

		public GameObject loadNFTSloader;

		// UIView INTERFACE

		/*void SetList(string text){
			
			 nfts = Array.FindAll(JsonUtility.FromJson<NFTS>(text).nfts, c => (c.collectionAddress=="bffoiWfiVYfT7b4NpXexhLmDLC6B6CZNHPtW6R1R46y"));
				if(nfts.Length>0){
					var lng = nfts.Length>8 ? 8 : nfts.Length;
			_skinList.Refresh(lng, false);
			UpdateAgent();
			}
		}	*/		

		protected override void OnInitialize()
		{
			base.OnInitialize();
			_skinList.SelectionChanged += OnSelectionChanged;
			_skinList.UpdateContent += OnListUpdateContent;
			CryptoManager.OnReceiveNFTS += filterNFTS;
			//CryptoManager.OnBloomies += getNFTS;
			_selectButton.onClick.AddListener(OnSelectButton);
		}

		void getNFTS(){
			loadNFTSloader.SetActive(true);
			_skinRarity.text = "Reading NFT's from wallet...";
			cryptoManager.getNFTSPF(PersistentStorage.GetString("wallet"));
		}

		private List<String> allowedTypes = new List<string>();
		

		void filterNFTS(string text){ // JUST FOR BFF JSON STRUCTURE 

			nfts = Array.FindAll(JsonUtility.FromJson<NFTS>(text).nfts, c => (c.collectionAddress=="bffoiWfiVYfT7b4NpXexhLmDLC6B6CZNHPtW6R1R46y"));
//			Debug.Log(nfts.Length + " NFT SELECTION SKIN");

				if(nfts.Length>0){
					for (int i = 0; i < nfts.Length; i++)
					{
						if(nfts[i].traits.Length>0){
							for (int a = 0; a < nfts[i].traits.Length; a++)
							{
								
								if(nfts[i].traits[a].trait_type=="Rarity"){
//									Debug.Log(nfts[i].traits[a].value + " Rarity NFTS");
									if(!allowedTypes.Contains(nfts[i].traits[a].value)){
										allowedTypes.Add(nfts[i].traits[a].value.ToLower());
									}
									
								}
							}
						}
					}
				}

			foreach( var x in allowedTypes) {
//				Debug.Log( x.ToString() + " ------------------------------------LIST ITEM");
			}

			loadNFTSloader.SetActive(false);
			_skinRarity.color = Color.white;

//			Debug.Log(_previewSkin + " Previously selected " + allowedTypes.Contains(_previewSkin.ToLower()));

			if(_previewSkin!=null){
					if(_previewSkin!="defaultSkin" &&  allowedTypes.Contains(_previewSkin.ToLower()) ){
				_skinRarity.color = Color.green;
				_skinRarity.text = "Skin selected";
				Context.PlayerData.SelectedSkin = _previewSkin;
			}else{
				
				_skinRarity.color = Color.white;
				_skinRarity.text = "Select your skin";
				Debug.Log("AQUI SE ESTA ASIGNANDO SIN QUE ESTE ACTIVO...");
				_previewSkin = "defaultSkin";
				//UpdateSkin();
			}
			}

		
		

			
			
		}

		private void OnListUpdateContent(int index, MonoBehaviour content)
		{
			var behaviour = content as UIBehaviour;
			var setup = Context.Settings.Skins.Skins[index];

			behaviour.Image.sprite = setup.Icon;
			/*if(nfts.Length>0){
				var setup = nfts[index];
				Debug.Log(setup.imageUrl + " " + setup.name);
				Davinci.get().load(setup.imageUrl).setCached(true).into(behaviour.Image).start();
			}*/
			
			
			//behaviour.Image.sprite = setup.Icon;
		}

	

		protected override void OnOpen()
		{
			base.OnOpen();
			walletAccess.SetActive(false);
			_selectedEffect.SetActive(false);

			//_previewSkin = Context.PlayerData.SelectedSkin;
			if (!String.IsNullOrEmpty(PersistentStorage.GetString("Skin")))
			{
				_previewSkin = PersistentStorage.GetString("Skin");
			}
			else 
			{
				_previewSkin = Context.PlayerData.SelectedSkin;
			}

			_skinList.Refresh(Context.Settings.Skins.Skins.Length, false);

			UpdateSkin();

			//_skinRarity.text = "Connect your wallet to use your skins";
			_skinRarity.text = "";

//	Debug.Log(PersistentStorage.GetString("wallet") + " Wallet for fnts?");

			if(PersistentStorage.GetString("wallet")!=""){
				getNFTS();
			}else{
				loadNFTSloader.SetActive(false);
				walletAccess.SetActive(true);
			}

			//cryptoManager.startManager();
			
		}


		protected override void OnClose()
		{
			base.OnClose();
		}

		protected override void OnDeinitialize()
		{
			_skinList.SelectionChanged -= OnSelectionChanged;
			_skinList.UpdateContent -= OnListUpdateContent;
			CryptoManager.OnReceiveNFTS -= filterNFTS;
			//CryptoManager.OnBloomies -= getNFTS;
			//CryptoManager.OnReceiveNFTS -= SetList;
			_selectButton.onClick.RemoveListener(OnSelectButton);

			base.OnDeinitialize();
		}

		// PRIVATE METHODS

		public static Action<String> selectionChangedList ;

		private void OnSelectionChanged(int index)
		{
			/*if(nfts.Length>0){
		   		_previewNFT = nfts[index].tokenAddress;
				Debug.Log(_previewNFT);
				UpdateAgent();
				
			}*/
	
			
			Debug.Log(index + " index Skin");
			if(index>-1){
				_previewSkin = Context.Settings.Skins.Skins[index].ID;

				bool allowed = false;

			foreach (var a in  allowedTypes)
			{
				if(a.ToString().ToLower()==_previewSkin.ToLower()){
					allowed=true;
				}
			}
			
			
			UpdateSkin();
			selectionChangedList?.Invoke(_previewSkin);
//			Debug.Log("Allowed" + allowed);

			if(allowed){
				Context.PlayerData.SelectedSkin = _previewSkin;
				_skinRarity.text = "Skin selected!";
				_skinRarity.color = Color.green;
			}else{
				if(_previewSkin!="default"){
					_skinRarity.text = "You need a " + _previewSkin +  " BFF to use this skin ";
					_skinRarity.color = Color.yellow;
				}else{
					_skinRarity.text = "";
					_skinRarity.color = Color.white;
					Context.PlayerData.SelectedSkin = Context.Settings.Skins.Skins[0].ID;
				}
				
			}
			}


			
			
			
			
		}

		private void OnSelectButton()
		{
				_selectedEffect.SetActive(false);
				_selectedEffect.SetActive(true);

				PlaySound(_selectedSound);

				UpdateSkin();
				selectionChangedList?.Invoke(Context.PlayerData.SelectedSkin);
				//Invoke("CloseWithBack", _closeDelayAfterSelection);
			/*bool isSame = Context.PlayerData.SelectedSkin == _previewSkin;

			if (isSame == false)
			{
				Context.PlayerData.SelectedSkin = _previewSkin;

				_selectedEffect.SetActive(false);
				_selectedEffect.SetActive(true);

				PlaySound(_selectedSound);

				UpdateSkin();
				Invoke("CloseWithBack", _closeDelayAfterSelection);
			}
			else
			{
				CloseWithBack();
			}*/
		}

		private void UpdateSkin()
		{
			try{
				var skinSetups = Context.Settings.Skins.Skins;

			

			_skinList.Selection = Array.FindIndex(skinSetups, t => t.ID == _previewSkin);

			/*
			if(skinSetups[_skinList.Selection].Available){
				_comming.SetActive(false);
				_selectButton.SetActive(true);
			}else{
				_comming.SetActive(true);
				_selectButton.SetActive(false);
			} */



			if (_skinList.Selection < 0)
			{
				_skinList.Selection = 0;
				_previewSkin = skinSetups[_skinList.Selection].ID;
			}

			if (_previewSkin.HasValue() == false)
			{
				//Context.PlayerPreview.HideAgent();
				_skinName.text = string.Empty;
				//_skinRarity.text = string.Empty;
			}
			else
			{
				var setup = Context.Settings.Skins.GetSkinSetup(_previewSkin);

				//Context.PlayerPreview.ShowAgent(_previewAgent);
				_skinName.text = string.Format(_skinNameFormat, setup.DisplayName)  + " holder";
				//_skinRarity.text = setup.Description;
			}

			_selectedSkinGroup.SetActive(_previewSkin == Context.PlayerData.SelectedSkin);

			if(_previewSkin!="defaultSkin" &&  allowedTypes.Contains(_previewSkin.ToLower()) ){
				Context.PlayerData.SelectedSkin = _previewSkin;
			}else{
				Context.PlayerData.SelectedSkin = "defaultSkin";
			}
			Debug.Log("Skin Name: "+ Context.PlayerData.SelectedSkin);
			
			PersistentStorage.SetString("Skin", Context.PlayerData.SelectedSkin);
			}catch(System.Exception e){

			}
			
			
		}
	}
}
