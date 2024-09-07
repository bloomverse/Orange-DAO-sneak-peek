using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


namespace TPSBR.UI
{
    public class UIBloomies : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI bloomText ;

        // Start is called before the first frame update
        void Start()
        {
            CryptoManager.OnBloomies += setBloomies;
        }
        void onDisable(){
          //  CryptoManager.OnBloomies -= setBloomies;
        }

        void setBloomies(string bloomies){

              int bloomiesFloat = (int)Math.Round(float.Parse(bloomies),0);
                bloomText.text = bloomiesFloat.ToString("#,##0");
                //FormatNumberWithDecimalsAndCommas(bloomiesFloat);
        }

      

         public static string FormatNumberWithDecimalsAndCommas(double number)
    {
        // Split the number into integer and decimal parts
        double integerPart = Mathf.Floor((float)number);
        double decimalPart = number - integerPart;

        // Format the integer part with commas
        string formattedInteger = string.Format("{0:N0}", integerPart);

        // Format the decimal part with exactly 2 decimal places
        string formattedDecimal = string.Format("{0:F2}", decimalPart);

        // Combine the formatted parts
        return string.Format("{0}.{1}", formattedInteger, formattedDecimal.Substring(2));
    }
    }
}
