using UnityEngine;

namespace TPSBR.UI
{
	public class UIWalletView : UICloseView
	{

		[SerializeField]
		private UIButton _disconnectButton;

		[SerializeField] LoginController loginController;
		
		protected override void OnInitialize()
		{
			base.OnInitialize();

			_disconnectButton.onClick.AddListener(OndisconnectButton);
		}
		protected override void OnDeinitialize()
		{
			_disconnectButton.onClick.RemoveListener(OndisconnectButton);

			base.OnDeinitialize();
		}

		public void closeWallet(){
			Close();
		}

        protected override void OnOpen()
        {
            base.OnOpen();
			loginController.OnOpen();
        }


        private void OndisconnectButton(){
			//PlayerPrefs.DeleteAll();
			//Close();
		}

	}

	


}
