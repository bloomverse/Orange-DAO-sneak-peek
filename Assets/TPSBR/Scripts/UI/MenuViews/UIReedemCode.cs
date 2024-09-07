using UnityEngine;
using TMPro;
using System;

namespace TPSBR.UI
{
	public class UIReedemCode : UICloseView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private TextMeshProUGUI _caption;
		[SerializeField]
		private TMP_InputField _name;
		[SerializeField]
		private UIButton _confirmButton;
		[SerializeField]
		private int _minCharacters = 5;

		[SerializeField]
		private CryptoManager cryptoManager;

		// PUBLIC METHODS

		public void SetData(string caption, bool nameRequired)
		{
			_caption.text = caption;
			CloseButton.SetActive(nameRequired == false);
		}

		// UIView INTERFACE

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_confirmButton.onClick.AddListener(OnConfirmButton);
		}

		protected override void OnDeinitialize()
		{
			_confirmButton.onClick.RemoveListener(OnConfirmButton);

			base.OnDeinitialize();
		}

		protected override void OnOpen()
		{
			Debug.Log("openning reedem code");
			base.OnOpen();
		}

		protected override void OnTick()
		{
			base.OnTick();

			_confirmButton.interactable = _name.text.Length >= _minCharacters && _name.text != Context.PlayerData.Email;
		}



		private void OnConfirmButton()
		{
			cryptoManager = GameObject.Find("CryptoManager").GetComponent<CryptoManager>();
			var Code = _name.text;
			
				CryptoManager.OnReedemCode += ReedemResponse;
				cryptoManager.ReedemCode(Code,Context.PlayerData.userData._id);
				//MissionSystem.Instance.Progress(1, 17);
			

			
		}
		public static Action reedemCompleted;
		private void ReedemResponse(string res){

			if(res=="Error"){
				var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Code Reedem";
				infoDialog.Description.text = "You code is not valid or it was already redeemed.";
				return;
			}

			if(res=="Completed"){
				reedemCompleted?.Invoke();
				Close();
			}
			Debug.Log("res"  + res);
			
		}
	}
}
