using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


namespace TPSBR.UI
{
	public class UIBloomieBundlesView : UICloseView
	{

		[SerializeField]
		private UIButton _disconnectButton;

		[SerializeField] GameObject loaderCont;
		[SerializeField] TextMeshProUGUI loaderText;

		[SerializeField] CanvasGroup bloomieGroup;

		public static float solanaPrice= 100.0f;
		
		protected override void OnInitialize()
		{
			base.OnInitialize();
			loaderCont.SetActive(false);

			DataManager.OnSolanaPriceResponse += solanaResponse;
		
		}

        protected override void OnOpen()
        {
            base.OnOpen();
			DataManager.instance.solanaPrice();
        }

        

		private void solanaResponse(solanaData price){
			solanaPrice = price.solana.usd;
		}


		protected override void OnDeinitialize()
		{
			base.OnDeinitialize();
			CryptoManager.OnSolanaResponse -= _tokenResponse;	
		}

		private float _bundle_price;
		private float _totalBloomies;

		public void startTransaction(float bundlePrice,float totalBloomies){

			Debug.Log("Checking particle walletconnection");
			_bundle_price = bundlePrice;
			_totalBloomies = totalBloomies;	

			CryptoManager.OnIsParticleResponse += CheckParticleConnection;
		    CryptoManager.instance.CheckIfParticleIs();

		
		}


		public void CheckParticleConnection(string value){
			Debug.Log("return value bloomie bundle");
			CryptoManager.OnIsParticleResponse -= CheckParticleConnection;
			if(value=="Connected"){
				bloomieGroup.alpha = 0.3f;
			bloomieGroup.interactable = false;

			loaderText.SetText("Waiting for solana payment confirmation...");
			loaderCont.SetActive(true);

			PlayerData playerData = Global.PlayerService.PlayerData;
			CryptoManager.OnSolanaResponse += _tokenResponse;	
			CryptoManager.AskSolanaTransaction(_bundle_price,_totalBloomies,playerData.userData.solanaWallet);

		    }else{
			         var dialog = Open<UIYesNoDialogView>();
                    dialog.Title.text = "Connect Inventory";
                    dialog.YesButtonText.SetText("Connect");
                    dialog.Description.text = "Please connect your inventory and try again.";
                    dialog.HasClosed += (result) =>
                    {
                        if (result == true){
                            CryptoManager.instance.LoginParticleClick();
                        }
                        else{
                            Close();
                        }
                    };

		    }
		}

		

		private void _tokenResponse(string res){
            
			if(res=="PhantomError"){
				loaderCont.SetActive(false);
				var infoDialog = Open<UIInfoDialogView>();

					infoDialog.Title.text = "Bloomverse Market ";
					infoDialog.Description.text = "There was an error in your transaction check that your inventory is connected";

					bloomieGroup.alpha = 1f;
					bloomieGroup.interactable = true;
			}else{
					loaderCont.SetActive(false);


					Close();
					var infoDialog = Open<UIInfoDialogView>();

					infoDialog.Title.text = "Bloomverse Market ";
					infoDialog.Description.text = "Your transaction is complete, your bloomies are being generated . They will show in your inventory in a minute";

					bloomieGroup.alpha = 1f;
					bloomieGroup.interactable = true;
			}

		
        }


		private void OndisconnectButton(){
			//PlayerPrefs.DeleteAll();
			//Close();
		}

	}

	


}
