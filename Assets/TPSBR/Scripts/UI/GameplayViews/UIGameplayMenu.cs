using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace TPSBR.UI
{
	public class UIGameplayMenu : UICloseView
	{
		// PUBLIC MEMBERS

		 [DllImport("__Internal")]
    	private static extern void CopyToClipboard(string text);

		public override bool NeedsCursor => _menuVisible;

		// PRIVATE MEMBERS

		[SerializeField]
		private UIButton _leaveButton;
		[SerializeField]
		private UIButton _settingsButton;
		[SerializeField]
		private UIButton _cancelButton;

		[SerializeField]
		private TextMeshProUGUI _leaveText;

		private bool _menuVisible;

		// PUBLIC METHODS

		public void Show(bool value, bool force = false)
		{
			if (_menuVisible == value && force == false)
				return;

			_menuVisible = value;

			(SceneUI as GameplayUI).RefreshCursorVisibility();

			if (value == true)
			{
				Animation.PlayForward();
			}
			else
			{
				Animation.PlayBackward();
			}
		}

		// UIView INTERFACE

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_leaveText.SetText("LEAVE MATCH");

			if(Context.GameplayMode is TutorialGameplayMode){
				_leaveText.SetText("EXIT TUTORIAL");
				
			}

			if(Context.GameplayMode is LobbyGameplayMode){
				_leaveText.SetText("EXIT LOBBY");
			}

			if(Context.GameplayMode is ActivityMode){
				_leaveText.SetText("BACK TO MAIN SCREEN");
			}

			_leaveButton.onClick.AddListener(OnLeaveButton);
			_settingsButton.onClick.AddListener(OnSettingsButton);
			_cancelButton.onClick.AddListener(OnCancelButton);
		}

		protected override void OnDeinitialize()
		{
			_leaveButton.onClick.RemoveListener(OnLeaveButton);
			_settingsButton.onClick.RemoveListener(OnSettingsButton);
			_cancelButton.onClick.RemoveListener(OnCancelButton);

			base.OnDeinitialize();
		}

		protected override void OnOpen()
		{
			base.OnOpen();

			Animation.SampleStart();
			_menuVisible = false;
		}

		protected override void OnCloseButton()
		{
			Debug.Log("UIGameplayMenu.OnCloseButton()");
			Show(false);
		}

		protected override bool OnBackAction()
		{
			if (_menuVisible == true)
				return base.OnBackAction();

			Show(true);
			return true;
		}

		// PRIVATE MEMBERS

		public void Notification(string desctext,string prizeId){
			
			var dialog = Open<UIClaimDialogView>();
			dialog.Title.text = "You've won";
			dialog.NoButtonText.SetText("Flex my Win");
			dialog.YesButtonText.SetText("Copy code and close");
			dialog.descriptionId.SetText(prizeId);
			dialog.Description.text = desctext;
			dialog.HasClosed += (result) =>
			{
				if (result == true){

					GUIUtility.systemCopyBuffer = desctext + " / " + prizeId;
					
					#if UNITY_WEBGL && !UNITY_EDITOR
					CopyToClipboard(desctext + " / " + prizeId);
					#endif
					//string clipBoard = GUIUtility.systemCopyBuffer;
				}
				else{
					var texturl = "https://twitter.com/intent/tweet?text=Just%20me%20casually%20claiming%20some%20nice%20loot%20from%20%40play_Bloomverse%20%0AI%20won%20" + desctext +"!%0AWhat%20did%20you%20get%20today?  PID: " + prizeId + "-" + UnityEngine.Random.Range(10,99) ;
						Application.OpenURL(texturl);
				}
			};
		}

		public void NotificationPrizeRestriction(string remaining){
			
			var infoDialog = Open<UIInfoDialogView>();
			infoDialog.Title.text = "Chest Prize!";
			infoDialog.Description.text = "Chest limit reached, come back later. \n " + remaining;
		}

		private void OnLeaveButton()
		{
			var dialog = Open<UIYesNoDialogView>();

			dialog.Title.text = "LEAVE MATCH";
			dialog.Description.text = "Are you sure you want to leave current match?";

				Debug.Log("Context" + Context.GameplayMode);

			if(Context.GameplayMode is TutorialGameplayMode){
				dialog.Title.text = "EXIT TUTORIAL";
				dialog.Description.text = "Are you sure your want to exit tutorial?.";
			}

			if(Context.GameplayMode is LobbyGameplayMode){
				dialog.Title.text = "EXIT LOBBY";
				dialog.Description.text = "Are you sure your want to exit lobby?.";
			}

			if(Context.GameplayMode is ActivityMode){
				dialog.Title.text = "EXIT LEVEL";
				dialog.Description.text = "Are you sure your want to exit level?.";
				
			}

			
			 
			

			dialog.HasClosed += (result) =>
			{
				if (result == true)
				{
					
					if(Context.GameplayMode is TutorialGameplayMode){
						PlayerPrefs.SetString("TutorialComplete","true") ;
					}

					if(Context.GameplayMode is ActivityMode){
						GlobalFunctions.ChangeURL("/");
					}

					

					if (Context != null && Context.GameplayMode != null)
					{
						Context.GameplayMode.StopGame();
					}
					else
					{
						Global.Networking.StopGame();
					}
				}
			};
		}

		private void OnSettingsButton()
		{
			var settings = Open<UISettingsView>();
			settings.HasClosed += () => { Show(false); };
		}

		private void OnCancelButton()
		{
			OnCloseButton();
		}
	}
}
