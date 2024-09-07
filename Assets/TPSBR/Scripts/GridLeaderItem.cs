using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TPSBR
{
    public class GridLeaderItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI positionField;
        [SerializeField] TextMeshProUGUI nicknameField;
        [SerializeField] TextMeshProUGUI scoreField;

        public void setValues(string position, string nickname, string score){
            positionField.text = "#" + position;
            nicknameField.text = nickname;
            scoreField.text = score;
        }   

        
    }
}
