using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TPSBR.UI
{
	public class UIPlayer : UIBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private TextMeshProUGUI _playerName;
		[SerializeField]
		 private TextMeshProUGUI _playerNum;
		  [SerializeField]
        private TextMeshProUGUI _playerStatus;

		[SerializeField]
		private Image _playerIcon;

		// PUBLIC MEMBERS

		public void SetData(SceneContext context, IPlayer player, int num = 0)
		{
			_playerName.text = player.Nickname;

			if (_playerIcon != null)
			{
				var agentSetup = context.Settings.Agent.GetAgentSetup(player.AgentPrefab);
				Sprite sprite = agentSetup != null ? agentSetup.Icon : null;

				_playerIcon.sprite = sprite;
				_playerIcon.SetActive(sprite != null);
			}
			if (_playerNum)
			{
                _playerNum.text = num.ToString();
            }

			if (_playerStatus)
			{
				_playerStatus.text = player.lobbyStatus;

            }
		}
	}
}
