using System;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.Networking;
using System.Collections;

namespace TPSBR.UI
{
	public class UINFTSelectionView : UICloseView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private CinemachineVirtualCamera _camera;
		[SerializeField]
		private UIList _NFTList;
		[SerializeField]
		private UIButton _selectButton;
		[SerializeField]
		private TextMeshProUGUI _NFTName;
		[SerializeField]
		private TextMeshProUGUI _NFTDescription;
		[SerializeField]
		private TextMeshProUGUI _NFTRarity;
		//[SerializeField]
		//private string _NFTNameFormat = "{0}";
		[SerializeField]
		private UIBehaviour _selectedNFTGroup;
		[SerializeField]
		private UIBehaviour _selectedEffect;
		[SerializeField]
		private AudioSetup _selectedSound;
		[SerializeField]
		private float _closeDelayAfterSelection = 0.5f;

		private string _previewNFT;

		private NFT[]  nfts = new NFT[0] ;

		// UIView INTERFACE

		void SetList(string text){
			
			 nfts = Array.FindAll(JsonUtility.FromJson<NFTS>(text).nfts, c => (c.collectionAddress=="bffoiWfiVYfT7b4NpXexhLmDLC6B6CZNHPtW6R1R46y"));
//			 Debug.Log(nfts.Length + " Largo nfts");
				if(nfts.Length>0){
					var lng = nfts.Length>8 ? 8 : nfts.Length;
			_NFTList.Refresh(lng, false);
			UpdateAgent();
			}
		}			

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_NFTList.SelectionChanged += OnSelectionChanged;
			_NFTList.UpdateContent += OnListUpdateContent;


			


			CryptoManager.OnReceiveNFTS += SetList;


			_selectButton.onClick.AddListener(OnSelectButton);
		}

		private void OnListUpdateContent(int index, MonoBehaviour content)
		{
			var behaviour = content as UIBehaviour;

			if(nfts.Length>0){
				var setup = nfts[index];
//				Debug.Log(setup.imageUrl + " " + setup.name);
				Davinci.get().load(setup.imageUrl).setCached(true).into(behaviour.Image).start();
			}
			
			
			//behaviour.Image.sprite = setup.Icon;
		}

	

		protected override void OnOpen()
		{
			base.OnOpen();
			_selectedEffect.SetActive(false);

			//_previewNFT = Context.PlayerData.AgentID;
			
			
			
		}

		
    

		protected override void OnClose()
		{
			
			base.OnClose();
		}

		protected override void OnDeinitialize()
		{
			_NFTList.SelectionChanged -= OnSelectionChanged;
			_NFTList.UpdateContent -= OnListUpdateContent;
			CryptoManager.OnReceiveNFTS -= SetList;
			_selectButton.onClick.RemoveListener(OnSelectButton);

			base.OnDeinitialize();
		}

		// PRIVATE METHODS

		public static Action selectionChangedList ;

		private void OnSelectionChanged(int index)
		{
			if(nfts.Length>0){
		   		//_previewNFT = nfts[index].tokenAddress;
				//Debug.Log(_previewNFT);
				//UpdateAgent();
				
			}
			selectionChangedList?.Invoke();
		}

		private void OnSelectButton()
		{
			bool isSame = Context.PlayerData.SelectedNFT == _previewNFT;

			if (isSame == false)
			{
				Context.PlayerData.SelectedNFT = _previewNFT;

				_selectedEffect.SetActive(false);
				_selectedEffect.SetActive(true);

				PlaySound(_selectedSound);

				UpdateAgent();
				Invoke("CloseWithBack", _closeDelayAfterSelection);
			}
			else
			{
				CloseWithBack();
			}
		}

		private void UpdateAgent()
		{
			if(nfts.Length>0){
			var agentSetups = nfts;
			_NFTList.Selection = Array.FindIndex(agentSetups, t => t.tokenAddress == _previewNFT);
//			Debug.Log(_NFTList.Selection);

			if (_NFTList.Selection < 0)
			{
				_NFTList.Selection = 0;
				_previewNFT = agentSetups[_NFTList.Selection].tokenAddress;
			}

			if (_previewNFT.HasValue() == false)
			{
				
				_NFTName.text = string.Empty;
				_NFTDescription.text = string.Empty;
				_NFTDescription.text = string.Empty;
			}
			else
			{
			
			
				_NFTName.text = agentSetups[_NFTList.Selection].name;
				_NFTDescription.text = agentSetups[_NFTList.Selection].collectionName + "  -  " +  agentSetups[_NFTList.Selection].description;
				
			}

			_selectedNFTGroup.SetActive(_previewNFT == Context.PlayerData.SelectedNFT);
			}
			
		}
	}
}
