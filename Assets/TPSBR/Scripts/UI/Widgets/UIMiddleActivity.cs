using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace TPSBR.UI
{
	using Fusion;

    public class UIMiddleActivity : UIWidget
    {
		// PRIVATE MEMBERS

	

		[SerializeField]
		private UIValue _remainingProgressValue;
		[SerializeField]
		private UIValue _remainingProgressTimeValue;
		[SerializeField]
		private UIValue _remainingTimeValue;

		[SerializeField]
		private TextMeshProUGUI _progText;


		[SerializeField]
		private Color colorCooling = Color.yellow;

		[SerializeField]
		private Color colorHarvesting = Color.blue;

		[SerializeField]
		private AudioEffect _shrinkingSound;

		private EliminationGameplayModeCustom _eliminationMode;

		// PUBLIC METHODS

		protected override void OnVisible()
		{
			_eliminationMode = Context.GameplayMode as EliminationGameplayModeCustom;
			 var scene = SceneManager.GetActiveScene();
			
//			Debug.Log(scene.name + " scene name");
			//Debug.Log("map name" + Context.NetworkGame.);
			transform.SetActive(scene.name=="generator" && Context.GameplayMode as EliminationGameplayModeCustom);
			
		}

		protected override void OnTick()
		{
			//var remainingTimeValue = area.NextShrinking.RemainingTime(runner);
			//float remainingTime = remainingTimeValue.HasValue == true ? remainingTimeValue.Value : 0f;

				if(_eliminationMode==null){
					return;
				}

				var timer = Mathf.CeilToInt(_eliminationMode.BloomieCooldownTime);
				var eventon = Mathf.CeilToInt(_eliminationMode.BloomieTime);
				//Debug.Log(timer + " Timer cooldown");
				//var remainingTime = Mathf.CeilToInt(_eliminationMode.BloomieTime.re);

				//_remainingProgressValue.SetValue(area.ShrinkDelay - remainingTime, area.ShrinkDelay);
				if(timer>0  && _eliminationMode._bloomieCooldownTimer.IsRunning){
					_progText.text = timer  + " Cooling down reactor...";
					//Debug.Log(10 - timer * 100 / 20  + "colling");
					_remainingProgressValue.SetValue((100 - (timer * 100 / 20)),100);
					_remainingProgressValue.SetFillColor(colorCooling);
				}
				if(_eliminationMode._bloomieCooldownTimer.Expired(Context.Runner) && !_eliminationMode._bloomieTimer.IsRunning){
					_progText.text =  "Reactor ready...";
					_remainingProgressValue.SetValue(100,100);
					_remainingProgressValue.SetFillColor(Color.green);
				}

				if(eventon>0  && _eliminationMode._bloomieTimer.IsRunning){
					_progText.text = eventon +  " harvesting...";
					//Debug.Log(10- (eventon * 100 / 1000)  + "harbesting");
					_remainingProgressValue.SetValue(eventon * 100 / 10,100);
					_remainingProgressValue.SetFillColor(colorHarvesting);
				}
				
				//_remainingTimeValue.SetValue(remainingTime);
			
			
		}
	}
}
