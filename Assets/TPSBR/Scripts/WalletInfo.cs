using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TPSBR.UI
{
    public class WalletInfo : MonoBehaviour
    {
        // Start is called before the first frame update

        public Button wallet_button;
        public CryptoManager cryptoManager;    
        public GameObject loader;

        void Start()
        {
            wallet_button.onClick.AddListener(WalletClick);
            //loader.SetActive(true);
            //cryptoManager.startManager();
        }

          public void WalletClick(){
            cryptoManager.WalletClick();
            //cryptoManager.LoginParticleClick();
        }

      
    }
}
