using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using TPSBR.UI;
using ReadyPlayerMe;


namespace TPSBR
{
    public class bloomieBundle :MonoBehaviour
    {
        [SerializeField] BloomeBundle_SO bundleInfo;
        [SerializeField] TextMeshProUGUI bundle_name;
        [SerializeField] Image bundle_image;
        [SerializeField] TextMeshProUGUI bundle_description;
        [SerializeField] TextMeshProUGUI bundle_price;
        [SerializeField] TextMeshProUGUI bundle_bonus;

        [SerializeField] UIButton bundleBuy;

        [SerializeField] AutoFitHeight autoFitHelper;

        private float solanPriceUSD = UIBloomieBundlesView.solanaPrice ;
        private float bloomieUSD = .001f;

        private float bonusTotal;
        private float totalBloomies;
        private float finalPrice;
        private float usdPrice;
        private float bundlePrice;

         [SerializeField] private UIBloomieBundlesView bundleController;

        void Start()
        {
            //setInfo();
            bundle_image.SetActive(false);
            DataManager.OnSolanaPriceResponse += setInfo;
        }

        void setInfo(solanaData extPrice){
            Debug.Log(extPrice.solana.usd + " Precio solana");

            DataManager.OnSolanaPriceResponse -= setInfo;
            //solanPriceUSD = extPrice.solana.usd;
            bonusTotal = bundleInfo.Amount*(bundleInfo.BonusPerc/100);
            finalPrice = (bundleInfo.Amount * bloomieUSD)  / extPrice.solana.usd;
            usdPrice = finalPrice * extPrice.solana.usd;
            string bonusText= "";
            if(bundleInfo.BonusPerc>0){
                bonusText = " + " + bonusTotal + " Bonus";
            }
            totalBloomies = bonusTotal + bundleInfo.Amount;
            bundle_name.SetText(bundleInfo.Name);
            bundle_bonus.SetText(bonusText);
            bundle_image.SetActive(true);
            bundle_image.sprite = bundleInfo.ImageUI;

           autoFitHelper.AdjustSize();
//            bundle_description.SetText(bundleInfo.Description);
            bundlePrice = finalPrice;
            bundle_price.SetText(Math.Round(finalPrice,2).ToString() + " SOL / " + Math.Round(usdPrice,2) + " USD");

            // TODO API to get SOLANA ACTUAL PRICE 

            bundleBuy.onClick.AddListener(onBuyBundle);

        }

        private void onBuyBundle()
        {
           bundleController.startTransaction(bundlePrice,totalBloomies);
        }

        

      
    }
}
