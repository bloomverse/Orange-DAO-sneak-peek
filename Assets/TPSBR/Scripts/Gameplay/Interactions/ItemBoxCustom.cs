using System;
using Fusion;
using UnityEngine;

namespace TPSBR
{
	public class ItemBoxCustom : NetworkBehaviour, IInteraction
	{
		// HELPERS

		public enum EState
		{
			None,
			Closed,
			Open,
			Locked,
		}

		// PUBLIC MEMBERS

		[Networked]
		public EState State { get; set; }

		// PRIVATE MEMBERS

		[Header("Item Box")]
		[SerializeField]
		private float _autoCloseTime;
		[SerializeField]
		private float _unlockTime;
		[SerializeField]
		private EState _startState;

		[SerializeField]
		private Transform _lockedState;
		[SerializeField]
		private Transform _unlockedState;

		[SerializeField]
		private AnimationClip _openAnimation;
		[SerializeField]
		private AnimationClip _closeAnimation;

		[Header("Interaction")]
		[SerializeField]
		private string _interactionName;
		[SerializeField]
		private string _interactionDescription;
		[SerializeField]
		private string _interactionLongDescription;
		[SerializeField]
		private int _hudID;


		[SerializeField]
		private Transform _hudPivot;
		[SerializeField]
		private Collider _interactionCollider;

		[Header("Pickups")]
		[SerializeField]
		private PickupPoint[] _pickupsSetup;

		//[SerializeField]
		public string _pickupinternalId;
		public string _pickupitemType;
		public string _pickupitem3D;
		public string _pickupitemMaterial;
		public string _pickupDesc;

		[SerializeField]
		private CustomPickup _pickupCustom;
		[SerializeField]
		private Transform _pickupTransform;

		[Header("Audio")]
		[SerializeField]
		private AudioEffect _audioEffect;
		[SerializeField]
		private AudioSetup _openSound;
		[SerializeField]
		private AudioSetup _closeSound;

		[Networked]
		private TickTimer StateTimer { get; set; }

		private Animation      _animation;
		private StaticPickup[] _nestedPickups;

	

		[SerializeField] private Renderer materialTop;
		[SerializeField] private Renderer materialBottom;

		[SerializeField] private Material materialCommon;
		[SerializeField] private Material materialEpic;
		[SerializeField] private Material materialLegendary;
		[SerializeField] private Material materialMhytic;
		[SerializeField] private Material materialTransparent;


		[SerializeField] private Material[] materials;

		public Transform opener;
		// PUBLIC METHODS
	
		public void SetMaterialChest(string chestMaterial){
			
			Material mat;
			bool myRandomBool = UnityEngine.Random.Range(0, 0) == 0; 

			if(myRandomBool){
				switch(chestMaterial){
				case "common"  :	mat= materialCommon;break;
				case "epic"  : 		mat= materialEpic;break;
				case "legendary"  : mat= materialLegendary;break;
				case "mythical"  : 	mat= materialMhytic;break;

				default : mat=materialCommon;break;
				}
			}else{
				mat= materialTransparent;
			}

			
			//Debug.Log(chestMaterial + " Chestmaterial");
			
			

			materialTop.material = mat;
			materialBottom.material = mat;
		}

	


		public void Open(Agent player)
		{
			Debug.Log("Open player " + player + " player Ref" + player.Context.LocalPlayerRef);
			if (Object.HasStateAuthority == false)
				return;
			if (State != EState.Closed)
				return;

			StateTimer = TickTimer.CreateFromSeconds(Runner, _autoCloseTime);
			State      = EState.Open;
			CustomPickup pickup = Runner.Spawn(_pickupCustom,
			 _pickupTransform.position,
			 _pickupTransform.rotation,
			 player.Object.InputAuthority,
			(runner, networkObject) => 
             networkObject.GetComponent<CustomPickup>().StartObj(_pickupinternalId,
			_pickupitemType,
			_pickupitem3D,
			_pickupDesc,
			_pickupitemMaterial)
        	);

			Debug.Log(_pickupitem3D + " -YY- " + _pickupitemType);
			pickup.SetBehaviour(StaticPickup.EBehaviour.Interaction, -1f);
			pickup.PickupConsumed += OnPickupConsumed;

		}

		public void Open()
		{
			Debug.Log("Open normal");
			if (Object.HasStateAuthority == false)
				return;
			if (State != EState.Closed)
				return;

			StateTimer = TickTimer.CreateFromSeconds(Runner, _autoCloseTime);
			State      = EState.Open;
			CustomPickup pickup = Runner.Spawn(_pickupCustom,
			 _pickupTransform.position,
			 _pickupTransform.rotation,
			 null,
			(runner, networkObject) => 
             networkObject.GetComponent<CustomPickup>().StartObj(_pickupinternalId,
			_pickupitemType,
			_pickupitem3D,
			_pickupDesc,
			_pickupitemMaterial)
        	);

			Debug.Log(_pickupitem3D + " -YY- " + _pickupitemType);
			pickup.SetBehaviour(StaticPickup.EBehaviour.Interaction, -1f);
			pickup.PickupConsumed += OnPickupConsumed;

		}

		// IInteraction INTERFACE

		string  IInteraction.Name        => _interactionName;
		string  IInteraction.Description => _interactionDescription;
		string  IInteraction.LongDescription => _interactionLongDescription;
		Vector3 IInteraction.HUDPosition => _hudPivot != null ? _hudPivot.position : transform.position;
		bool    IInteraction.IsActive    => State == EState.Closed;
		int 	IInteraction.HUDID       => _hudID;

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_animation = GetComponent<Animation>();
		}

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
			if (Object.HasStateAuthority == false)
			{
				OnStateChangedCustom(State);

				if (ApplicationSettings.IsStrippedBatch == true)
				{
					gameObject.SetActive(false);
				}

				return;
			}

			//_nestedPickups = new StaticPickup[_pickupsSetup.Length];

			switch (_startState)
			{
				case EState.None:
				case EState.Closed:
					Unlock();
					break;
				case EState.Open:
					Open();
					break;
				case EState.Locked:
					Lock();
					break;
				default:
					break;
			}
		}

		public override void FixedUpdateNetwork()
		{
			if (Object.HasStateAuthority == false)
				return;

			switch (State)
			{
				case EState.Open:   Update_Open();   break;
				case EState.Locked: Update_Locked(); break;
			}
		}

		// PRIVATE METHODS

		private void Update_Open()
		{
			if (StateTimer.Expired(Runner) == false)
				return;

			Lock();
		}

		private void Update_Locked()
		{
			if (StateTimer.Expired(Runner) == false)
				return;

			Unlock();
		}

		private void Lock()
		{
			if (State == EState.Locked)
				return;

			StateTimer = TickTimer.CreateFromSeconds(Runner, _unlockTime);
			State      = EState.Locked;
		}

		private void Unlock()
		{
			State = EState.Closed;
		}

		private void OnStateChangedCustom(EState state)
		{
			if (ApplicationSettings.IsStrippedBatch == true)
				return;

			_lockedState.SetActive(state == EState.Locked);
			_unlockedState.SetActive(state != EState.Locked);
			_interactionCollider.enabled = state == EState.Closed;

			switch (state)
			{
				case EState.Open:
					if (_animation.clip != _openAnimation)
					{
						_animation.clip = _openAnimation;
						_animation.Play();
						var player = FindObjectOfType<Player>();
                        var owner = player != null ? player.ActiveAgent : null;
                        owner.GetComponent<Agent>().mecan.Animator.CrossFadeInFixedTime("OpenChest",.5f);
						//owner.transform.localPosition = opener.localPosition;
						//owner.transform.localRotation = opener.localRotation;
                        _audioEffect.Play(_openSound, EForceBehaviour.ForceAny);
					}
					break;
				case EState.Closed:
				case EState.Locked:
					if (_animation.clip != _closeAnimation)
					{
						_animation.clip = _closeAnimation;
						_animation.Play();

						_audioEffect.Play(_closeSound, EForceBehaviour.ForceAny);
					}
					break;
				default:
					break;
			}
		}

		private void OnPickupConsumed(StaticPickup pickup)
		{
			if (pickup.AutoDespawn == false)
				return;

			int index = Array.IndexOf(_nestedPickups, pickup);
			if (index >= 0)
			{
				// Clear reference, pickup will be auto despawned
				_nestedPickups[index] = null;
			}
		}

		private static void StateChanged(ItemBoxCustom changed)
		{
//			Debug.Log("@state custom changed");
			changed.OnStateChangedCustom(changed.State);
		}

		

		[Serializable]
		private class PickupPoint
		{
			public Transform      Transform;
			public PickupData[]   Pickups;
		}

		[Serializable]
		private class PickupData
		{
			public StaticPickup Pickup;
			public int          Probability = 1;
		}
	}
}
