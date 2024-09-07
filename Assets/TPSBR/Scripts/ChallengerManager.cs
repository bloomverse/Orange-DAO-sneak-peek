using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    public class ChallengerManager : MonoBehaviour
    {
        public Text tutorialText;
        public float completionMessageDuration = 1f;

        private int currentStep;
        private bool isStepCompleted;
        private float completionMessageTimer;

        private void Start()
        {
            currentStep = 0;
            PromptAction();
        }

        private void Update()
        {
            if (!isStepCompleted)
            {
                if (IsStepCompleted())
                {
                    isStepCompleted = true;
                    completionMessageTimer = 0f;
                    CompleteStep();
                }
            }
            else
            {
                completionMessageTimer += Time.deltaTime;
                if (completionMessageTimer >= completionMessageDuration)
                {
                    isStepCompleted = false;
                    currentStep++;
                    PromptAction();
                }
            }
        }

        private bool IsStepCompleted()
        {
           switch (currentStep)
            {
                case 0: // Movement
                    return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
                case 1: // Jump
                    return Input.GetKeyDown(KeyCode.Space);
                case 2: // Shoot
                    return Input.GetMouseButtonDown(0);
                case 3: // Zoom
                    return Input.GetMouseButtonDown(1);
                case 4: // Sprint
                    return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                case 5: // Fly
                    return Input.GetKey(KeyCode.Space);
                case 6:
                    return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
                default:
                    return true;
            }
            //return false;
        }

        private void PromptAction()
        {
            switch (currentStep)
            {
                case 0:
                    SetTutorialText("Press W, A, S, or D to move!");
                    break;
                case 1:
                    SetTutorialText("Press Space to jump!");
                    break;
                case 2:
                    SetTutorialText("Press Mouse 1 to shoot!");
                    break;
                case 3:
                    SetTutorialText("hold Mouse 2 to Zoom!");
                    break;
                case 4:
                    SetTutorialText("Press Shift to sprint!");
                    break;
                case 5: // Sprint
                    SetTutorialText("Double Press and hold Space to Fly!");
                    break;
                case 6: // Shoot
                    SetTutorialText("Go to shooting range to test your shooting skill");
                    break;
                default:
                    SetTutorialText("");
                    break;
            }
        }

        private void CompleteStep()
        {
            switch (currentStep)
            {
                case 0:
                    SetTutorialText("Nice, Movement Task completed!");
                    break;
                case 1:
                    SetTutorialText("Welldone, Jump Task completed!");
                    break;
                case 2:
                    SetTutorialText("Great, Shoot Task completed!");
                    break;
                case 3:
                    SetTutorialText("Nicely Done, Zoom Task completed!");
                    break;
                case 4:
                    SetTutorialText("Good Job, Sprint Task completed!");
                    break;
                case 5:
                    SetTutorialText("Doing Great, Flying Tasks Complete!");
                    break;
                case 6:
                    SetTutorialText("");
                    break;
            }
        }

        private void SetTutorialText(string text)
        {
            tutorialText.text = text;
        }
    }
}
