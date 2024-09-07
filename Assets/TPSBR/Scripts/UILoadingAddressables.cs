using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TPSBR.UI
{
    public class UILoadingAddressables : UIWidget
    {
        [SerializeField]
        private Transform _container;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
		private UIValue _remainingProgressValue;

        private int  progValue = 0;
        private bool start;
        private Agent _player;

        void Start(){
            AddressablesManager.LoadingAssets += UpdateLoadingBar;
           //  Agent _player = Object.FindObjectOfType<Agent>();
          // _player.AgentInput.SetActionStates(EGameplayInputActionStates.Look);
        }

        //void OnVisible()
        //{
           
           //Context.AgentInput.SetActionStates(EGameplayInputActionStates.None);
          
        //}
        void UpdateLoadingBar(int progress)
		{
            start = true;
            Debug.Log("Loading addressables UI" + progress);
            progValue = progress;
            
        }

       protected override void OnTick()
		{
            if (!start){
                return;
            }
            if(progValue<100){
                _container.SetActive(true);
                _remainingProgressValue.SetValue(progValue/100);
                _text.text = "Loading Assets..." + progValue + "%";
            }else{
                _text.text = "Loading Assets...100%";
                _container.SetActive(false);
                AddressablesManager.LoadingAssets -= UpdateLoadingBar;
              //  _player.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust ^ EGameplayInputActionStates.Aim ^ EGameplayInputActionStates.Spray ^ EGameplayInputActionStates.ToggleSpeed ^ EGameplayInputActionStates.Interact ^ EGameplayInputActionStates.Attack ^ EGameplayInputActionStates.Reload);

            }
        }


        


        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
