using UnityEngine;
using TMPro;
using System;

namespace TPSBR.UI
{
	public class UIChangeEmailView : UICloseView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private TextMeshProUGUI _caption;
		[SerializeField]
		private TMP_InputField _name;
		[SerializeField]
		private UIButton _confirmButton;
		[SerializeField]
		private int _minCharacters = 1;

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
			Debug.Log("openning change email");
			base.OnOpen();

			string currentEmail = Context.PlayerData.Email;
			if (currentEmail.HasValue() == false)
			{
				//_name.text = "B_" + Random.Range(10000, 100000);
			}
			else
			{
				_name.text = Context.PlayerData.Email;
			}
		}

		protected override void OnTick()
		{
			base.OnTick();

			_confirmButton.interactable = _name.text.Length >= _minCharacters && _name.text != Context.PlayerData.Email;
		}



		private void OnConfirmButton()
		{
			cryptoManager = GameObject.Find("CryptoManager").GetComponent<CryptoManager>();
			Context.PlayerData.Email = _name.text;
			//Debug.Log("updatign nicjanmae" + PersistentStorage.GetString("token"));
			if (PersistentStorage.GetString("token") != "")
			{
				cryptoManager.updateEmail();
				MissionSystem.Instance.Progress(1, 17);
			}

			Close();
		}
	}
}
