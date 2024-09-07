using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    public class RandomTargets : MonoBehaviour
    {
        public List<GameObject> targets;

        public int totalPoint;
        public int currentPoints;
        public int challengeCounter;

        public string gun;
        public string difficulty;

        private bool movementCompleted;
        private bool jumpCompleted;
        private bool sprintCompleted;


        private bool shootCompleted;
        public Text totalPointsText;
        public Text currentPointsTexts;
        public Text challengeText;

        public Animator[] animator;
        public GameObject challenge1, challenge2, challenge3;

        void Start()
        {
            animator = GetComponentsInChildren<Animator>();
            StartCoroutine(ChallengeStarted());
            
        }

        public void SetAnimatorSpeed(int speed)
        {
            foreach(Animator anim in animator)
            {
                anim.speed = speed;
            }
        }
        public IEnumerator ChallengeStarted()
        {
            StartCoroutine(round(0, "Pistol", callback =>
            {
                Challenge1("Round 1: shoot targets with pistol \n Difficulty easy");
                StartCoroutine(DisplayRandomTargets());
                StartCoroutine(round(1, "Pistol", callback =>
                 {
                     SetAnimatorSpeed(2);
                     Challenge1("Round 2: shoot targets with pistol \n Difficulty medium");
                     StopCoroutine(DisplayRandomTargets());
                     StartCoroutine(DisplayRandomTargets());
                     StartCoroutine(round(2, "Pistol", callback =>
                     {
                         SetAnimatorSpeed(3);
                         Challenge1("Round 3: shoot targets with pistol \n Difficulty hard");
                         StopCoroutine(DisplayRandomTargets());
                         StartCoroutine(DisplayRandomTargets());
                         StartCoroutine(round(3, "Pistol", callback =>
                         {
                             SetAnimatorSpeed(1);
                             Challenge2("Round 1: shoot targets with Plasma Rifle \n Difficulty easy");
                             StopCoroutine(DisplayRandomTargets());
                             StartCoroutine(DisplayRandomTargets());
                             StartCoroutine(round(1, "Plasma Rifle", callback =>
                             {
                                 SetAnimatorSpeed(2);
                                 Challenge2("Round 2: shoot targets with Plasma Rifle \n Difficulty medium");
                                 StopCoroutine(DisplayRandomTargets());
                                 StartCoroutine(DisplayRandomTargets());
                                 StartCoroutine(round(2, "Plasma Rifle", callback =>
                                 {
                                     SetAnimatorSpeed(3);
                                     Challenge2("Round 3: shoot targets with Plasma Rifle \n Difficulty hard");
                                     StartCoroutine(DisplayRandomTargets());
                                     StartCoroutine(round(3, "Plasma Rifle", callback =>
                                     {
                                         SetAnimatorSpeed(1);
                                         Challenge3("Round 1: shoot targets with Skull Breaker \n Difficulty easy");
                                         StartCoroutine(DisplayRandomTargets());
                                         StartCoroutine(round(1, "Skull Breaker", callback =>
                                         {
                                             SetAnimatorSpeed(2);
                                             Challenge3("Round 2: shoot targets with Skull Breaker \n Difficulty medium");
                                             StartCoroutine(DisplayRandomTargets());
                                             StartCoroutine(round(2, "Skull Breaker", callback =>
                                             {
                                                 SetAnimatorSpeed(3);
                                                 Challenge3("Round 3: shoot targets with Skull Breaker \n Difficulty hard");
                                                 StartCoroutine(DisplayRandomTargets());
                                                 StartCoroutine(round(3, "Skull Breaker", callback =>
                                                 {
                                                     challengeText.text = "Challenge Completed";                                                                                                         
                                                 }));
                                             }));
                                         }));
                                     }));
                                 }));
                             }));
                         }));
                     }));
                 }));
            }));
            yield return null;
        }

        IEnumerator round(int round, string gun, Action<bool> callback)
        {
            if (round != 0)
            {
                yield return new WaitUntil(() => currentPoints >= totalPoint);
                
                yield return new WaitForSeconds(2f);
                challengeText.text = gun + " round " + round + " completed ";
                yield return new WaitForSeconds(2f);
                if(round == 3)
                {
                    if (gun.Contains("Pistol"))
                    {
                        challengeText.text = "Get ready for Plasma Rifle round 1";
                    }
                    if (gun.Contains("Plasma Rifle"))
                    {
                        challengeText.text = "Get ready for Skull Breaker round 1";
                    }
                }
                else
                {
                    challengeText.text = "Get ready for " + gun.ToLower() + " round " + (round + 1);
                }
            }
            else
            {
                challengeText.text = "Get ready for " + gun.ToLower() + " round " + (round+1);
            }            
            yield return new WaitForSeconds(1f);
            challengeText.text = "3";
            yield return new WaitForSeconds(1f);
            challengeText.text = "2";
            yield return new WaitForSeconds(1f);
            challengeText.text = "1";
            yield return new WaitForSeconds(1f);
            challengeText.text = "GO!";
            yield return new WaitForSeconds(1f);
            challengeText.text = "";
            callback(true);
        }
        public void Challenge1(string message)
        {
            totalPoint = 100;
            currentPoints = 0;
            totalPointsText.text = "Target : " + totalPoint.ToString();
            currentPointsTexts.text = "Points : " + currentPoints.ToString();
            challengeText.text = message;
            gun = "Pistol";

            if (message.Contains("easy"))
            {
                difficulty = "easy";
            }
            if (message.Contains("medium"))
            {
                difficulty = "medium";
            }
            if (message.Contains("hard"))
            {
                difficulty = "hard";
            }
        }

        public void Challenge2(string message)
        {
            totalPoint = 100;
            currentPoints = 0;
            totalPointsText.text = "Target : " + totalPoint.ToString();
            currentPointsTexts.text = "Points : " + currentPoints.ToString();
            challengeText.text = message;
            gun = "Plasma Rifle";

            if (message.Contains("easy"))
            {
                difficulty = "easy";
            }
            if (message.Contains("medium"))
            {
                difficulty = "medium";
            }
            if (message.Contains("hard"))
            {
                difficulty = "hard";
            }
        }

        public void Challenge3(string message)
        {
            totalPoint = 100;
            currentPoints = 0;
            totalPointsText.text = "Target : " + totalPoint.ToString();
            currentPointsTexts.text = "Points : " + currentPoints.ToString();
            challengeText.text = message;
            gun = "Skull Breaker";

            if (message.Contains("easy"))
            {
                difficulty = "easy";
            }
            if (message.Contains("medium"))
            {
                difficulty = "medium";
            }
            if (message.Contains("hard"))
            {
                difficulty = "hard";
            }
        }

        public void UpdatePoints(int points, string gun)
        {
            if (!this.gun.Contains(gun))
            {
                return;
            }
            currentPoints += points;
            if (currentPoints >= totalPoint)
            {
                currentPoints = totalPoint;
            }
            currentPointsTexts.text = "Points : " + currentPoints.ToString();
        }
    
        IEnumerator DisplayRandomTargets()
        {
            yield return new WaitForSeconds(2f);
            challengeText.text = "";
            yield return new WaitForSeconds(2f);
            int i = UnityEngine.Random.Range(0,targets.Count);

            List<int> temp = new List<int>();
            for(int j = 0; j < i; j++)
            {
                int k = UnityEngine.Random.Range(0, targets.Count);
                while (temp.Contains(k))
                {
                    k = UnityEngine.Random.Range(0, targets.Count);
                }
                targets[k].GetComponent<Animator>().Rebind();
                temp.Add(k);
            }
            StartCoroutine(DisplayRandomTargets());
        }
    }

}   
