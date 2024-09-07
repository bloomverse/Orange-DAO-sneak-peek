using Fusion;

namespace TPSBR
{
	using UnityEngine;

	public enum EGameplayInputAction
	{
		Jump          = 1,
		Aim           = 2,
		Attack        = 3,
		ToggleSpeed   = 4,
		Reload        = 6,
		Interact      = 7,
		ToggleSide    = 8,
		ToggleJetpack = 9,
		Thrust        = 10,
		Spray		  = 11,
		Crouch        = 12,
		Emote         = 13,
		CanInteractic = 14,
		CanInteracticChest = 15

	}

	 public enum EGameplayInputActionStates
    {
		None = 0,
        Jump = 1,
        Aim = 2,
        Attack = 4,
        ToggleSpeed = 8,
        Reload = 16,
        Interact = 32,
        ToggleSide = 64,
        ToggleJetpack = 128,
        Thrust = 256,
        Spray = 512,
        Crouch = 1024,
        Emote = 2048,
		Look = 4096,
		Move = 8192,
		Weapon = 16384,
    }


	public struct GameplayInput : INetworkInput
	{
		// PUBLIC MEMBERS

		public Vector2        MoveDirection;
		public Vector2        LookRotationDelta;
		public NetworkButtons Actions;
		public byte           Weapon;
		public float          LocalAlpha;            // This value is used for render-accurate actions. Valid range is 0.0 - 1.0 and represents position of the time between current and last fixed tick.
		public float          InterpolationAlpha;    // This value is used for render-accurate lag compensated casts.
		public int            InterpolationFromTick; // This value is used for render-accurate lag compensated casts.
		public int            InterpolationToTick;   // This value is used for render-accurate lag compensated casts.

		public bool 
		CanInteractWithItem { get { return Actions.IsSet(EGameplayInputAction.CanInteractic);          } set { Actions.Set(EGameplayInputAction.CanInteractic, value); } }
		public bool 
		CanInteractWithChest { get { return Actions.IsSet(EGameplayInputAction.CanInteracticChest);          } set { Actions.Set(EGameplayInputAction.CanInteracticChest, value); } }

		public bool Jump          { get { return Actions.IsSet(EGameplayInputAction.Jump);          } set { Actions.Set(EGameplayInputAction.Jump,          value); } }
		public bool Aim           { get { return Actions.IsSet(EGameplayInputAction.Aim);           } set { Actions.Set(EGameplayInputAction.Aim,           value); } }
		public bool Attack        { get { return Actions.IsSet(EGameplayInputAction.Attack);        } set { Actions.Set(EGameplayInputAction.Attack,        value); } }
		public bool ToggleSpeed   { get { return Actions.IsSet(EGameplayInputAction.ToggleSpeed);   } set { Actions.Set(EGameplayInputAction.ToggleSpeed,   value); } }
		public bool Reload        { get { return Actions.IsSet(EGameplayInputAction.Reload);        } set { Actions.Set(EGameplayInputAction.Reload,        value); } }
		public bool Interact      { get { return Actions.IsSet(EGameplayInputAction.Interact);      } set { Actions.Set(EGameplayInputAction.Interact,      value); } }
		public bool ToggleSide    { get { return Actions.IsSet(EGameplayInputAction.ToggleSide);    } set { Actions.Set(EGameplayInputAction.ToggleSide,    value); } }
		public bool ToggleJetpack { get { return Actions.IsSet(EGameplayInputAction.ToggleJetpack); } set { Actions.Set(EGameplayInputAction.ToggleJetpack, value); } }
		public bool Thrust        { get { return Actions.IsSet(EGameplayInputAction.Thrust);        } set { Actions.Set(EGameplayInputAction.Thrust,        value); } }
		public bool Spray         { get { return Actions.IsSet(EGameplayInputAction.Spray);         } set { Actions.Set(EGameplayInputAction.Spray,         value); } }
		public bool Crouch		  { get { return Actions.IsSet(EGameplayInputAction.Crouch);		} set { Actions.Set(EGameplayInputAction.Crouch,	    value); } }
		public bool Emote		  { get { return Actions.IsSet(EGameplayInputAction.Emote);		    } set { Actions.Set(EGameplayInputAction.Emote,	   		value); } }

	}

	public static class GameplayInputActionExtensions
	{
		// PUBLIC METHODS

		public static bool IsActive(this EGameplayInputAction action, GameplayInput input)
		{
			return input.Actions.IsSet(action) == true;
		}

		public static bool WasActivated(this EGameplayInputAction action, GameplayInput currentInput, GameplayInput previousInput)
		{
			return currentInput.Actions.IsSet(action) == true && previousInput.Actions.IsSet(action) == false;
		}

		public static bool WasDeactivated(this EGameplayInputAction action, GameplayInput currentInput, GameplayInput previousInput)
		{
			return currentInput.Actions.IsSet(action) == false && previousInput.Actions.IsSet(action) == true;
		}
	}
}
