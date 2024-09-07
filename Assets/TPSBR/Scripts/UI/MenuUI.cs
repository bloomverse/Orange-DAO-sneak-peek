using UnityEngine;

namespace TPSBR.UI
{
	public class MenuUI : SceneUI
	{
		// SceneUI INTERFACE

		//[SerializeField]
		private CryptoManager cryptoManager;
		

		protected override void OnInitializeInternal()
		{
			base.OnInitializeInternal();
			//PlayerPrefs.DeleteAll();
			Context.Input.RequestCursorVisibility(true, ECursorStateSource.Menu);
			//Debug.Log(PersistentStorage.GetString("token") + " otken");

			if (Context.PlayerData.Nickname.HasValue() == false && PersistentStorage.GetString("token")== "")
			{
				var changeNicknameView = Open<UIChangeNicknameView>();
				changeNicknameView.SetData("NICKNAME", true);
			}else if(PersistentStorage.GetString("token")!= ""){
				//cryptoManager.getUserData();
			}
		}

		protected override void OnDeinitializeInternal()
		{
			Context.Input.RequestCursorVisibility(false, ECursorStateSource.Menu);

			base.OnDeinitializeInternal();
		}

		protected override void OnActivate()
		{
			base.OnActivate();

			if (Global.Networking.ErrorStatus.HasValue() == true)
			{
				Open<UIMultiplayerView>();
				var errorDialog = Open<UIErrorDialogView>();

				errorDialog.Title.text = "Connection Issue";

				if (Global.Networking.ErrorStatus == Networking.STATUS_SERVER_CLOSED)
				{
					errorDialog.Description.text = $"Server was closed.";
				}
				else
				{
					errorDialog.Description.text = $"Failed to start network game\n\nReason:\n{Global.Networking.ErrorStatus}";
				}

				Global.Networking.ClearErrorStatus();
			}
		}
	}
}
