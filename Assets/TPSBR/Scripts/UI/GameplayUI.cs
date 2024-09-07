using System.Collections;
using System;
using UnityEngine;

namespace TPSBR.UI
{
	public class GameplayUI : SceneUI
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float _gameOverScreenDelay = 2f;

		private UIDeathView _deathView;

		private bool _gameOverShown;
		private Coroutine _gameOverCoroutine;


		[SerializeField]
		private Canvas VFXcanvas;


		// PUBLIC METHODS

		public void RefreshCursorVisibility()


		{
			bool showCursor = false;

			for (int i = 0; i < _views.Length; i++)
			{
				var view = _views[i];

				if (view.IsOpen == true && view.NeedsCursor == true)
				{
					showCursor = true;
					break;
				}
			}

			Context.Input.RequestCursorVisibility(showCursor, ECursorStateSource.UI);
		}

		// SceneUI INTERFACE

		protected override void OnInitializeInternal()
		{
			base.OnInitializeInternal();





			_deathView = Get<UIDeathView>();
		}

		protected override void OnActivate()
		{
			base.OnActivate();



			if (Context.Runner.Mode == Fusion.SimulationModes.Server)
			{
				Open<UIDedicatedServerView>();
			}
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();

			if (_gameOverCoroutine != null)
			{
				StopCoroutine(_gameOverCoroutine);
				_gameOverCoroutine = null;
			}

			_gameOverShown = false;
		}

		protected override void OnTickInternal()
		{
			base.OnTickInternal();

			if (_gameOverShown == true)
				return;
			if (Context.Runner == null || Context.Runner.Exists(Context.GameplayMode.Object) == false)
				return;

			var player = Context.NetworkGame.GetPlayer(Context.LocalPlayerRef);
			if (player == null || player.Statistics.IsAlive == true)
			{
				_deathView.Close();
			}
			else
			{
				_deathView.Open();
			}

			if (Context.GameplayMode.State == GameplayMode.EState.Finished && _gameOverCoroutine == null)
			{

				_gameOverCoroutine = StartCoroutine(ShowGameOver_Coroutine(_gameOverScreenDelay));
			}
		}

		protected override void OnViewOpened(UIView view)
		{
			RefreshCursorVisibility();


		}

		protected override void OnViewClosed(UIView view)
		{
			RefreshCursorVisibility();
		}

		// PRIVATE METHODS


		public void ChangeEmail()
		{
			Debug.Log("change email");

			Open<UIChangeEmailView>();

		}

		public void OpenUserSignIn(){
			Open<UIWalletView>();
		}

		public void CloseUserSignIn(){
			Close<UIWalletView>();
		}

		public void ReedemUI()
		{
			Debug.Log("launch reedem");

			Open<UIReedemCode>();

		}

		public void infoGameShift(){
			  var infoDialog = Open<UIInfoDialogView>();
			infoDialog.Title.text = "Info";
			infoDialog.Description.text = "Powered by Gameshift and Solana Labs, your Bloom account allows top security protocols to protect your account while making it retrievable if you ever lose access to it. It will also allow you to use 2FA in the near future to prevent unauthorized transactions from happening. Use a secure email that you trust. ";
		}

		private void restartGame(UIView view){
			Close<UIView>();
		}

		private IEnumerator ShowGameOver_Coroutine(float delay)
		{
			yield return new WaitForSeconds(delay);

			_gameOverShown = true;

			_deathView.Close();
			Close<UIGameplayView>();
			Close<UIScoreboardView>();
			Close<UIGameplayMenu>();
			Close<UIAnnouncementsView>();

			Debug.Log("gamemode " + Context.GameplayMode.GameplayName);

			switch(Context.GameplayMode.GameplayName){

				
				case "Tutorial" : Open<UITutorialGameOverView>();break;
				case "Hopscotch" : Open<UIHopscotchGameOverView>();break;
				default : Open<UIGameOverView>();break;
			}






			_gameOverCoroutine = null;
		}
	}
}
