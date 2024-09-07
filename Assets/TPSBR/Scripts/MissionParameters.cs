using System.Collections;
using TPSBR.UI;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPSBR
{
    public abstract class MissionObjectiveParameters
    {
        public int m_MissionId;
        public int m_ObjectiveId;

        public virtual IEnumerator ObjectiveStart() { yield return null; }
        public virtual IEnumerator ObjectiveUpdated() { yield return null; }
        public virtual IEnumerator ObjectiveCompleted() { yield return null; }

        public MissionObjectiveParameters(int missionId, int objectiveId) { m_MissionId = missionId; m_ObjectiveId = objectiveId; }
    }

    public class Tutorial_1_Look : MissionObjectiveParameters
    {
        public Tutorial_1_Look(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }

        public override IEnumerator ObjectiveStart()
        {
            /*GameObject _Canvas = GameObject.Find("GameplayUI");

            _Canvas.transform.GetChild(1).GetChild(2).GetChild(1).SetActive(false);
            _Canvas.transform.GetChild(1).GetChild(2).GetChild(3).SetActive(false);
            _Canvas.transform.GetChild(1).GetChild(7).SetActive(false);
            _Canvas.transform.GetChild(1).GetChild(8).SetActive(false);
            _Canvas.transform.GetChild(1).GetChild(17).GetChild(1).SetActive(false);*/

            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();

            _TutorialPlayer.Weapons.SwitchWeapon(0);
            _TutorialPlayer.AgentInput.SetRenderInput(new GameplayInput
            {
                Weapon = 1
            }, false);
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look);

            TutorialManager.Instance.SetTutorialKeys("look", true);
            TutorialManager.Instance.AnnouncerStep(5f);

            yield return new WaitForSeconds(5);

            TutorialManager.Instance.SetTutorialKeys("look", false);
            MissionSystem.Instance.Progress(1, 1);
        }

        public override IEnumerator ObjectiveCompleted()
        {
            TutorialManager.Instance.AnnouncerStep(3.5f);

            yield return new WaitForSeconds(2f);

            MissionSystem.Instance.BeginMission(1);
        }
    }

    public class Tutorial_2_Move : MissionObjectiveParameters
    {
        public Tutorial_2_Move(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }

        public override IEnumerator ObjectiveStart()
        {
            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.Crouch);

            TutorialManager.Instance.SetTutorialKeys("move", true);

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SetTutorialKeys("move", false);

            yield return new WaitForSeconds(2f);

            MissionSystem.Instance.BeginMission(1);
        }
    }

    public class Tutorial_3_Jump : MissionObjectiveParameters
    {
        public Tutorial_3_Jump(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }

        public override IEnumerator ObjectiveStart()
        {
            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.Crouch);

            TutorialManager.Instance.SetTutorialKeys("jump", true);
            TutorialManager.Instance.CheckpointIndex++;

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SetTutorialKeys("jump", false);
            TutorialManager.Instance.StartDroneSection();

            yield return null;
        }
    }
    public class Tutorial_4_Weapon : MissionObjectiveParameters
    {
        public Tutorial_4_Weapon(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }

        public override IEnumerator ObjectiveStart()
        {
            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump);

            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.CheckpointIndex++;

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            Agent _TutorialPlayer = GameObject.FindObjectOfType<Agent>();

            _TutorialPlayer.Weapons.SwitchWeapon(1);
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Attack);

            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SpawnDroneWave(0);

            yield return new WaitForSeconds(1);

            MissionSystem.Instance.BeginMission(1);
        }
    }
    public class Tutorial_5_Shooting : MissionObjectiveParameters
    {
        public Tutorial_5_Shooting(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }

        private void OnWeaponFired_Event()
        {
            MissionSystem.Instance.Progress(1, 5);
        }

        public override IEnumerator ObjectiveStart()
        {
            Agent _TutorialPlayer = GameObject.FindObjectOfType<Agent>();

            _TutorialPlayer.OnWeaponFired += OnWeaponFired_Event;

            TutorialManager.Instance.CheckpointIndex++;
            TutorialManager.Instance.SetTutorialKeys("shoot", true);

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            Agent _TutorialPlayer = GameObject.FindObjectOfType<Agent>();

            _TutorialPlayer.OnWeaponFired -= OnWeaponFired_Event;

            TutorialManager.Instance.SetTutorialKeys("shoot", false);

            yield return null;
        }
    }
    public class Tutorial_6_Reloading : MissionObjectiveParameters
    {
        public Tutorial_6_Reloading(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }

        private void OnWeaponReloaded_Event()
        {
            MissionSystem.Instance.Progress(1, 6);
        }

        public override IEnumerator ObjectiveStart()
        {
            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SetTutorialKeys("reload", true);

            Agent _TutorialPlayer = GameObject.FindObjectOfType<Agent>();

            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Attack ^ EGameplayInputActionStates.Weapon ^ EGameplayInputActionStates.Reload);
            _TutorialPlayer.OnWeaponManualReload += OnWeaponReloaded_Event;

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            Agent _TutorialPlayer = GameObject.FindObjectOfType<Agent>();

            _TutorialPlayer.OnWeaponManualReload -= OnWeaponReloaded_Event;

            TutorialManager.Instance.SpawnDroneWave(1);
            TutorialManager.Instance.SetTutorialKeys("reload", false);

            yield return null;
        }
    }
    public class Tutorial_7_Aiming : MissionObjectiveParameters
    {
        public Tutorial_7_Aiming(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        private void OnAim_Event()
        {
            MissionSystem.Instance.Progress(1, 7);
        }

        public override IEnumerator ObjectiveStart()
        {
            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust ^ EGameplayInputActionStates.Aim ^ EGameplayInputActionStates.Spray ^ EGameplayInputActionStates.ToggleSpeed ^ EGameplayInputActionStates.Attack);
            _TutorialPlayer.OnAim += OnAim_Event;

            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SetTutorialKeys("aim", true);

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            TutorialManager.Instance.SetTutorialKeys("aim", false);

            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();

            _TutorialPlayer.OnAim -= OnAim_Event;

            yield return null;
        }
    }
    public class Tutorial_8_Jetpack : MissionObjectiveParameters
    {
        public Tutorial_8_Jetpack(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        public override IEnumerator ObjectiveStart()
        {
            /*GameObject _Canvas = GameObject.Find("GameplayUI");

            _Canvas.transform.GetChild(1).GetChild(2).GetChild(3).SetActive(true);*/

            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();

            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust);

            TutorialManager.Instance.AnnouncerStep(7f);
            TutorialManager.Instance.SetTutorialKeys("jetpack", true);

            yield return new WaitForSeconds(1f);
        }
        public override IEnumerator ObjectiveCompleted()
        {
            yield return null;

            TutorialManager.Instance.SetTutorialKeys("jetpack", false);
            MissionSystem.Instance.BeginMission(1);
        }
    }
    public class Tutorial_9_Spray : MissionObjectiveParameters
    {
        public Tutorial_9_Spray(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }

        private void OnSpray_Event()
        {
            MissionSystem.Instance.Progress(1, 9);
        }

        public override IEnumerator ObjectiveStart()
        {
            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust ^ EGameplayInputActionStates.Aim ^ EGameplayInputActionStates.Spray);
            _TutorialPlayer.OnSpray += OnSpray_Event;

            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SetTutorialKeys("spray", true);

            yield return null;
        }

        public override IEnumerator ObjectiveCompleted()
        {
            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust ^ EGameplayInputActionStates.Aim ^ EGameplayInputActionStates.Spray);
            _TutorialPlayer.OnSpray -= OnSpray_Event;

            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SetTutorialKeys("spray", false);
            TutorialManager.Instance.EnableBoostDoor();

            yield return null;
        }
    }
    public class Tutorial_10_Boost : MissionObjectiveParameters
    {
        public Tutorial_10_Boost(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        public override IEnumerator ObjectiveStart()
        {
            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();

            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust ^ EGameplayInputActionStates.Spray ^ EGameplayInputActionStates.ToggleSpeed);

            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SetTutorialKeys("boost", true);

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            TutorialManager.Instance.SetTutorialKeys("boost", false);

            yield return new WaitForSeconds(1f);

            MissionSystem.Instance.BeginMission(1);
            TutorialManager.Instance.ShowAnnouncerText(3.5f);
        }
    }
    public class Tutorial_11_Chest : MissionObjectiveParameters
    {
        public Tutorial_11_Chest(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        private void OnItemBoxOpened_Event()
        {
            MissionSystem.Instance.Progress(1, 11);
        }

        public override IEnumerator ObjectiveStart()
        {
            TutorialManager.Instance.AnnouncerStep(3.5f);
            TutorialManager.Instance.SetTutorialKeys("interact", true);
            TutorialManager.Instance.ActivateChest();

            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            _TutorialPlayer.Weapons.SwitchWeapon(2);
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust ^ EGameplayInputActionStates.Aim ^ EGameplayInputActionStates.Spray ^ EGameplayInputActionStates.ToggleSpeed ^ EGameplayInputActionStates.Interact);
            _TutorialPlayer.OnItemBoxOpened += OnItemBoxOpened_Event;
            yield return null;
        }

        public override IEnumerator ObjectiveCompleted()
        {
            TutorialManager.Instance.EnableBossDoor();
            TutorialManager.Instance.SetTutorialKeys("interact", false);

            yield return null;
        }
    }
    public class Tutorial_12_Boss : MissionObjectiveParameters
    {
        public Tutorial_12_Boss(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        public override IEnumerator ObjectiveStart()
        {
            TutorialManager.Instance.AnnouncerStep(10f);

            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look);

            yield return new WaitForSeconds(10f);

            TutorialManager.Instance.BeginFinalBoss();
            TutorialManager.Instance.CheckpointIndex++;

            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleJetpack ^ EGameplayInputActionStates.Thrust ^ EGameplayInputActionStates.Aim ^ EGameplayInputActionStates.Spray ^ EGameplayInputActionStates.ToggleSpeed ^ EGameplayInputActionStates.Interact ^ EGameplayInputActionStates.Attack ^ EGameplayInputActionStates.Reload);

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            TutorialManager.Instance.AnnouncerStep(6f);

            yield return null;
        }
    }
    public class Tutorial_13_Lobby : MissionObjectiveParameters
    {
        public Tutorial_13_Lobby(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        public override IEnumerator ObjectiveStart()
        {
            yield return new WaitForSeconds(7f);

            LobbyTutorialManager.Instance.AnnouncerStep(7f);


            MissionSystem.Instance.Progress(1, 13);
        }

        public override IEnumerator ObjectiveCompleted()
        {
            yield return new WaitForSeconds(7f);

            LobbyTutorialManager.Instance.AnnouncerStep(7.5f);

            yield return new WaitForSeconds(7.5f);

            LobbyTutorialManager.Instance.AnnouncerStep(4f);

            yield return new WaitForSeconds(4f);

            MissionSystem.Instance.BeginMission(1);

            yield return null;
        }
    }
    public class Tutorial_14_HV_Enter : MissionObjectiveParameters
    {
        public Tutorial_14_HV_Enter(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        public override IEnumerator ObjectiveStart()
        {
            Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
            Debug.Log("Spawing objects");
            _TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.ToggleSpeed ^ EGameplayInputActionStates.Thrust);
            LobbyTutorialManager.Instance.SpawnLobbyObjects(_TutorialPlayer != null ? _TutorialPlayer.transform : null);

            LobbyTutorialManager.Instance.ToggleHVArrow(true);
            LobbyTutorialManager.Instance.SpawnLobbyTrigger();

            yield return null;
        }

        public override IEnumerator ObjectiveCompleted()
        {
            LobbyTutorialManager.Instance.ToggleHVArrow(false);
            LobbyTutorialManager.Instance.AnnouncerStep(5.5f);

            yield return new WaitForSeconds(5.5f);

            LobbyTutorialManager.Instance.AnnouncerStep(5.5f);

            yield return new WaitForSeconds(5.5f);

            LobbyTutorialManager.Instance.AnnouncerStep(7f);

            yield return new WaitForSeconds(7f);

            MissionSystem.Instance.BeginMission(1);
        }
    }
    public class Tutorial_15_HV_Interact : MissionObjectiveParameters
    {
        public Tutorial_15_HV_Interact(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        private void Tutorial_15_HV_Interact_OnStaticInteraction()
        {
            //MissionSystem.Instance.Progress(1, 15);

            /* Start Happy Vibes Minigame */
            LobbyTutorialManager.Instance.LoadHVMinigame();
        }
        public override IEnumerator ObjectiveStart()
        {
            Agent[] _Agents = Object.FindObjectsOfType<Agent>();

            foreach (Agent _Agent in _Agents)
            {
                if (_Agent.IsLocal)
                {
                    _Agent.AgentInput.SetActionStates(EGameplayInputActionStates.Look ^ EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.Interact ^ EGameplayInputActionStates.ToggleSpeed ^ EGameplayInputActionStates.Thrust);
                    _Agent.OnStaticInteraction += Tutorial_15_HV_Interact_OnStaticInteraction;
                    break;
                }
            }

            LobbyTutorialManager.Instance.ToggleHVConsoleArrow(true);

            yield return null;
        }

        public override IEnumerator ObjectiveCompleted()
        {
            LobbyTutorialManager.Instance.ToggleHVConsoleArrow(false);

            Agent[] _Agents = Object.FindObjectsOfType<Agent>();

            foreach (Agent _Agent in _Agents)
            {
                if (_Agent.IsLocal)
                {
                    _Agent.OnStaticInteraction -= Tutorial_15_HV_Interact_OnStaticInteraction;
                    break;
                }
            }

            yield return new WaitForSeconds(3f);

            MissionSystem.Instance.BeginMission(1);
        }
    }
    public class Tutorial_16_HV_Game : MissionObjectiveParameters
    {
        public Tutorial_16_HV_Game(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        private void Tutorial_16_HV_Game_OnStaticInteraction()
        {
            MissionSystem.Instance.Progress(1, 16);
        }

        public override IEnumerator ObjectiveStart()
        {
            Agent[] _Agents = Object.FindObjectsOfType<Agent>();

            foreach (Agent _Agent in _Agents)
            {
                if (_Agent.IsLocal)
                {
                    _Agent.OnStaticInteraction += Tutorial_16_HV_Game_OnStaticInteraction;
                    break;
                }
            }

            LobbyTutorialManager.Instance.AnnouncerStep(4f);
            LobbyTutorialManager.Instance.ToggleHVGameArrow(true);
            LobbyTutorialManager.Instance.ToggleArcadeButton(true);

            yield return null;

        }



        public override IEnumerator ObjectiveCompleted()
        {
            LobbyTutorialManager.Instance.ToggleHVGameArrow(false);

            Agent[] _Agents = Object.FindObjectsOfType<Agent>();

            foreach (Agent _Agent in _Agents)
            {
                if (_Agent.IsLocal)
                {
                    _Agent.OnStaticInteraction -= Tutorial_16_HV_Game_OnStaticInteraction;

                    break;
                }
            }

          yield return null;

            //GameplayUI gameplayUI = GameObject.Find("GameplayUI").GetComponent<GameplayUI>();
           // gameplayUI.ChangeEmail();
             Debug.Log("Llegeo al final acabo de empezar el correo antes");

            MissionSystem.Instance.BeginMission(1);
        }
    }
    public class Tutorial_17_HV_Email : MissionObjectiveParameters
    {
        public Tutorial_17_HV_Email(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        public override IEnumerator ObjectiveStart()
        {
            // Email Logic
            
           // LobbyTutorialManager.Instance.SetAnnouncerStep(7);
            LobbyTutorialManager.Instance.AnnouncerStep(10f);

            yield return new WaitForSeconds(10f);

            //LobbyTutorialManager.Instance.SetAnnouncerStep(8);
            LobbyTutorialManager.Instance.AnnouncerStep(5f);
            yield return new WaitForSeconds(4f);

            GameplayUI gameplayUI = GameObject.Find("GameplayUI").GetComponent<GameplayUI>();
            gameplayUI.ChangeEmail();
            // UIChangeEmailView.emailDone +=  ;
            //yield return new WaitForSeconds(5f);

            // End Email Logic
            yield return null;
            //

        }
        public override IEnumerator ObjectiveCompleted()
        {
            LobbyTutorialManager.Instance.AnnouncerStep(12f);

            yield return new WaitForSeconds(14f);

            LobbyTutorialManager.Instance.AnnouncerStep(4f);
            yield return new WaitForSeconds(4f);

            LobbyTutorialManager.Instance.AnnouncerStep(6f);

            yield return new WaitForSeconds(6f);

            // Load Main Menu Logic
            Global.Networking.StopGame();

        }
    }
    public class Tutorial_Drones : MissionObjectiveParameters
    {
        public Tutorial_Drones(int missionId, int objectiveId) : base(missionId, objectiveId)
        {
        }
        public override IEnumerator ObjectiveStart()
        {
            /* Agent _TutorialPlayer = Object.FindObjectOfType<Agent>();
             string _AddAmmoResult;
             _TutorialPlayer.Weapons.AddAmmo(1, 60, out _AddAmmoResult);

             Debug.LogError(_AddAmmoResult);*/

            yield return null;
        }
        public override IEnumerator ObjectiveCompleted()
        {
            yield return new WaitForSeconds(1f);

            if (MissionSystem.Instance.AllLastRankMissionObjectivesComplete(1))
            {
                MissionSystem.Instance.BeginMission(1);
            }
        }
    }
}
