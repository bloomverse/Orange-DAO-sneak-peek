using UnityEngine;

namespace TPSBR.UI
{
	public class UILobbyCustom : UIWidget
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private GameObject _jumpGroup;
		[SerializeField]
		private UIValue _jumpTime;
		[SerializeField]
		private GameObject _waitingForPlayersGroup;
		[SerializeField]
		private UIValue _waitingForPlayersTime;
		[SerializeField]
		private GameObject _waitingForPlayersServerGroup;
		[SerializeField]
		private UIButton _startDropButton;
		[SerializeField]
		private UIButton _addTimeButton;

		private EliminationGameplayModeCustom _eliminationMode;

		// UIWidget INTERFACE

		protected override void OnInitialize()
		{
			_jumpGroup.SetActive(false);
			_waitingForPlayersGroup.SetActive(false);
			_waitingForPlayersServerGroup.SetActive(false);

			_startDropButton.onClick.AddListener(OnStartDropButton);
			_addTimeButton.onClick.AddListener(OnAddTimeButton);
		}

		protected override void OnDeinitialize()
		{
			_startDropButton.onClick.RemoveListener(OnStartDropButton);
			_addTimeButton.onClick.RemoveListener(OnAddTimeButton);
		}

		protected override void OnVisible()
		{
			_eliminationMode = Context.GameplayMode as EliminationGameplayModeCustom;
		}

		protected override void OnHidden()
		{
			Context.Input.RequestCursorVisibility(false, ECursorStateSource.Menu);
		}
		protected override void OnTick()
		{
//			Debug.Log(_eliminationMode.Object + " Elimination object");
//			Debug.Log(Context.Runner + "  Reunner context");

			if (  Context.Runner.Exists(_eliminationMode.Object) == false )
			return;

		//	Debug.Log(_eliminationMode.HasStarted + " has started");

	//	_waitingForPlayersGroup.SetActive(_eliminationMode.HasStarted == false);

		//	bool showServerGroup = _eliminationMode.HasStarted == false; //&& (_eliminationMode.Object.HasStateAuthority == true || ApplicationSettings.IsModerator == true);
	//		_waitingForPlayersServerGroup.SetActive(showServerGroup);

	//		Context.Input.RequestCursorVisibility(showServerGroup, ECursorStateSource.Menu, false);

		//	if (_eliminationMode.HasStarted == false)
		//	{
		//		_waitingForPlayersTime.SetValue(_eliminationMode.WaitingCooldown);
		//	}

			//bool canJump = _eliminationMode.HasStarted == true && _eliminationMode.AirplaneActive == true && Context.ObservedAgent == null;
			//_jumpGroup.SetActive(canJump);

			//if (canJump == true)
			//{
				//_jumpTime.SetValue(_eliminationMode.DropCooldown);
			//}

		}

		// PRIVATE METHODS

		private void OnStartDropButton()
		{
			Debug.Log("Start game");
			//_eliminationMode.StartImmediately();
		}

		private void OnAddTimeButton()
		{
			_eliminationMode.TryAddWaitTime(30f);
		}
	}
}
