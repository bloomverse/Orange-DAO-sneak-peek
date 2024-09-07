using System;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using ReadyPlayerMe;
using System.Runtime.InteropServices;


namespace TPSBR.UI
{
	public class UIRPMLoad : UICloseView
	{

		[SerializeField]
		private UIButton _selectButton;

		[SerializeField]
		private UIButton _createAvatar;
		
		[SerializeField]		
		private TextMeshProUGUI _statusText;

		[SerializeField]		
		private TMP_InputField _inputText;

		[SerializeField]
		private UIBehaviour _selectedSkinGroup;
		[SerializeField]
		private UIBehaviour _selectedEffect;
		[SerializeField]
		private AudioSetup _selectedSound;
		[SerializeField]
		private float _closeDelayAfterSelection = 0.5f;

		 [DllImport("__Internal")]
    	private static extern void PasteHereWindow(string gettext);

		


		public GameObject loader;

		protected override void OnInitialize()
		{
			base.OnInitialize();
			_selectButton.onClick.AddListener(OnSelectButton);
			_createAvatar.onClick.AddListener(OnCreateAvatar);
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			_selectedEffect.SetActive(false);

			#if UNITY_WEBGL && !UNITY_EDITOR
				_inputText.onSelect.AddListener( delegate { OnSelectInput();});
			#endif

			Debug.Log(PersistentStorage.GetString("RPMURL") + " remote URL prev");

			if(PersistentStorage.GetString("RPMURL")!=""){
				_inputText.text = PersistentStorage.GetString("RPMURL");
				Context.PlayerData.RPMSKIN= PersistentStorage.GetString("RPMURL");
			}else{
				
			}

			//cryptoManager.startManager();
			
		}

		private void OnSelectInput(){
			PasteHereWindow("READY PLAYER ME URL");
		}
		public void GetPastedText(string newpastedtext)
		{
			Debug.Log(newpastedtext + " new paste text");
			_inputText.text = newpastedtext;
		}

		protected override void OnClose()
		{
			base.OnClose();
		}
		protected override void OnDeinitialize()
		{
			_selectButton.onClick.RemoveListener(OnSelectButton);
			base.OnDeinitialize();
		}

		private void OnSelectButton()
		{
			loadAv(_inputText.text);
				
		}

		public void loadAv(string url){
			PersistentStorage.SetString("RPMURL",url);
				
				//ontext.Settings.Agent.Agents[3].
				//RPMloaderMenu.loadExt(_inputText.text);
				Context.PlayerData.RPMSKIN= url.Trim();
				Context.PlayerPreview._agentInstance.GetComponent<RPMloaderMenu>().LoadAvatar(url.Trim());
				WebInterface.SetIFrameVisibility(false);
				//Debug.Log(_inputText.text);	
		}

		private void OnCreateAvatar()
		{
			  	 PartnerSO partner = Resources.Load<PartnerSO>("Partner");
				 WebInterface.SetIFrameVisibility(true);
				 WebInterface.SetupRpmFrame(partner.Subdomain);
		}

		}
}
