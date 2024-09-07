using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TPSBR.UI
{
    public class UIlevelInfo : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI XPdesc;

        [SerializeField]
        private TextMeshProUGUI XPtext;

        [SerializeField]
        private TextMeshProUGUI LVLdesc;

        [SerializeField]
        private Image levelProgress;

        [SerializeField]
        private GameObject LevelParent;

        void Start()
        {
            CryptoManager.OnUserData += setLevel;
        }
        public void OnDestroy()
        {
            CryptoManager.OnUserData -= setLevel;
        }

        void setLevel(UserData userData)
        {
            if (userData.status != -1)
            {
                LevelParent.SetActive(true);
                var exp = Global.PlayerService.PlayerData.userData.userExp;
                int level = levelByExp((float)exp);
                var levelExpNext = requeiredExpforNext(level + 1) - requeiredExpforNext(level);
                var remaining = requeiredExpforNext(level + 1) - (int)exp;
                var fillAmount = 100 - (remaining * 100 / levelExpNext);
                XPdesc.SetText(exp.ToString("#,##0") + " / " + requeiredExpforNext(level + 1).ToString("#,##0"));
                LVLdesc.SetText(level.ToString());
                XPtext.SetText((exp.ToString("#,##0")));
                levelProgress.fillAmount = (float)fillAmount / 100;
            }
        }

        private int levelByExp(float exp)
        {
            //return (int)Mathf.Round(0.3f * (exp*exp)) ;
            return (int)Mathf.Floor((0.03f * Mathf.Sqrt(exp)));
        }

        private float requeiredExpforNext(int level)
        {
            return (float)Mathf.Pow(level / 0.03f, 2);
        }
    }
}
