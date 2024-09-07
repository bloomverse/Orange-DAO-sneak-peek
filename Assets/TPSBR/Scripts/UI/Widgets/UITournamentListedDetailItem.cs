using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TPSBR.UI
{
    public class UITournamentListedDetailItem : UIBehaviour
	{
      
        
        [SerializeField]
        private TextMeshProUGUI _nickname;
        [SerializeField]
        private TextMeshProUGUI _kills;
        [SerializeField]
        private TextMeshProUGUI _deaths;
        [SerializeField]
        private TextMeshProUGUI _damage;
        [SerializeField]
        private TextMeshProUGUI _score;
       

        // PUBLIC METHODS

        public void SetData(TournamentPlayer playerInfo)
        {
            if (playerInfo == null)
                return;

            _nickname.text = playerInfo.nickname;
            _kills.text = "" + playerInfo.kills.ToString("#,##0");
            _deaths.text = "" + playerInfo.deaths.ToString("#,##0");
            _damage.text = "" + playerInfo.damage.ToString("#,##0");
            _score.text = "" + playerInfo.score.ToString("#,##0");

            
        }
    }
}
