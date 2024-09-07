using UnityEngine;
using TMPro;

namespace TPSBR.UI
{
	public class UIGamePlayLobby : UIWidget
	{
		// PRIVATE MEMBERS

		

		private LobbyGameplayMode _LobbyMode;

		[SerializeField] TextMeshProUGUI  PrizesText;

		// UIWidget INTERFACE

		protected override void OnInitialize()
		{
			
		}

		protected override void OnDeinitialize()
		{
			
		}

		public void setRemaining(string text){
			PrizesText.SetText(text);
		}

		protected override void OnVisible()
		{
			_LobbyMode = Context.GameplayMode as LobbyGameplayMode;
		}

		protected override void OnHidden()
		{
			Context.Input.RequestCursorVisibility(false, ECursorStateSource.Menu);
		}
		protected override void OnTick()
		{
			if (Context.Runner.Exists(_LobbyMode.Object) == false)
				return;

			//double remaining = InventoryManager.getRemaining();

			//	string msg = InventoryManager.GetFormattedRemainingTime();
				//PrizesText.SetText(msg);

		}

	
	
	}
}
