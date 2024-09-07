using UnityEngine;
using UnityEngine.Profiling;
using Fusion;
using Fusion.Addons.KCC;
using TMPro;
using DG.Tweening;
using System;
using TPSBR.UI;
using System.Collections;
using Random=UnityEngine.Random;


namespace TPSBR
{
	[DefaultExecutionOrder(-5)]
	public sealed class Agent : ContextBehaviour, ISortedUpdate
	{
		// PUBLIC METHODS

        public bool IsLocal => Object != null && Object.HasInputAuthority == true;
		public bool IsObserved => Context != null && Context.ObservedAgent == this;

		public AgentInput        AgentInput   => _agentInput;
		public Interactions      Interactions => _interactions;
		public Character         Character    => _character;
		public Weapons           Weapons      => _weapons;
		public Health            Health       => _health;
		public AgentSenses       Senses       => _senses;
		public Jetpack           Jetpack      => _jetpack;
		public AgentVFX          Effects      => _agentVFX;
		public AgentInterestView InterestView => _interestView;

		[Networked]
		public NetworkBool LeftSide { get; private set; }

        public event System.Action OnWeaponFired, OnWeaponManualReload, OnSpray, OnAim, OnShoot, OnItemBoxOpened, OnStaticInteraction;

		// PRIVATE MEMBERS

		[SerializeField]
		private float _jumpPower;
		[SerializeField]
		private float _topCameraAngleLimit;
		[SerializeField]
		private float _bottomCameraAngleLimit;
		[SerializeField]
		private GameObject _visualRoot;

		



		[Header("Fall Damage")]
		[SerializeField]
		private float _minFallDamage = 5f;
		[SerializeField]
		private float _maxFallDamage = 200f;
		[SerializeField]
		private float _maxFallDamageVelocity = 20f;
		[SerializeField]
		private float _minFallDamageVelocity = 5f;

		private AgentInput          _agentInput;
		private Interactions        _interactions;
		private AgentFootsteps      _footsteps;
		private Character           _character;
		private Weapons             _weapons;
		private Jetpack             _jetpack;
		private AgentSenses         _senses;
		private Health              _health;
		private AgentVFX            _agentVFX;
		private AgentInterestView   _interestView;
		private SortedUpdateInvoker _sortedUpdateInvoker;
		private Quaternion          _cachedLookRotation;
		private Quaternion          _cachedPitchRotation;

        [SerializeField]
        private TextMeshProUGUI nicknameInput;
        [Networked]
        public int sprayCount { get; set; } = 0;
        [Networked]
        public int sprayMax { get; set; } = 8;
        private bool pendingPush;
        private HitData pushAwayData;
        [Header("Effects")]
        [SerializeField] public GameObject DashParticles;
        [Networked]
        public string dBID { get; set; } = "";

         [Networked]
        private byte _state { get; set; } = 0;

        [Networked, Capacity(20)]
        public string AgentID { get; set; } = "";

        [Networked ]
        public string skinName { get; set; } = "";


        public bool weaponsAllowed { get; set; } = true;

        [Networked, Capacity(200)]
        public string nickname { get; set; } = "";

        public GameObject[] tags;
        public GameObject sprayPrefab;
        public event Action<bool> SprintAction;
        public NetworkMecanimAnimator mecan;
        public int jumpCount;

		public float strafeMagnitude;
		public Vector2 inputCharacter;
		public bool pistolEquip, gunEquip;
        public bool shoot;
        GameObject obj;
		public float shootDelay;
        private float onlyArmsLayerWeight;
		 public GameObject leftHand, rightHand;

		[SerializeField]
        private KCCProcessor _fastMovementProcessor;


		public CharacterAnimationController cac;
	

		public bool IsActive { get { return _state.IsBitSet(0); } set { byte state = _state; state.SetBit(0, value); _state = state; } }

		// NetworkBehaviour INTERFACE
         public void setOptions(bool weapons, string nicknameE, string skinNameE, string AgentID)
        {
            weaponsAllowed = weapons;
            nickname = nicknameE;
            skinName = skinNameE;
        }
		public void setRandomAnim(int num)
        {
            GameplayInput input = default;
            input.Weapon = (byte)num;

        }

		 public void setSkin(string selectedSkin)
        {
            if (GetComponent<SkinLiveAdaptorTextures>() != null)
            {
                GetComponent<SkinLiveAdaptorTextures>().setRandom(selectedSkin);
            }

        }

		public override void Spawned()
		{
			name = Object.InputAuthority.ToString();

			_sortedUpdateInvoker = Runner.GetSingleton<SortedUpdateInvoker>();

			_visualRoot.SetActive(true);

			_character.OnSpawned(this);
			_jetpack.OnSpawned(this);
            _weapons.OnSpawned(weaponsAllowed);
			_health.OnSpawned(this);
			_agentVFX.OnSpawned(this);

             SprintAction?.Invoke(false);

			 // Set Skin
            if (AgentID == "Lilly" && skinName != null)
            {
                //						//Debug.Log("Setting bloomverse skin");
                setSkin(skinName);

            }
            // Set Name
            if (!IsLocal)
            {
                nicknameInput.text = nickname;
            }
            else
            {
                // nicknameInput.text = nickname;
                nicknameInput.text = "";
            }

			DashParticles.GetComponentInChildren<TrailRenderer>().emitting = false;

			if (ApplicationSettings.IsStrippedBatch == true)
			{
				gameObject.SetActive(false);

				if (ApplicationSettings.GenerateInput == true)
				{
					NetworkEvents networkEvents = Runner.GetComponent<NetworkEvents>();
					networkEvents.OnInput.RemoveListener(GenerateRandomInput);
					networkEvents.OnInput.AddListener(GenerateRandomInput);
				}
			}

            if(Context.PlayerData.userData!=null){
                InventoryManager.GetInventory();
            }

			 CryptoManager.OnUserData += changeSignState;

            

			return;

			void GenerateRandomInput(NetworkRunner runner, NetworkInput networkInput)
			{
				// Used for batch testing

				GameplayInput gameplayInput = new GameplayInput();
				gameplayInput.MoveDirection     = new Vector2(UnityEngine.Random.value * 2.0f - 1.0f, UnityEngine.Random.value > 0.25f ? 1.0f : -1.0f).normalized;
				gameplayInput.LookRotationDelta = new Vector2(UnityEngine.Random.value * 2.0f - 1.0f, UnityEngine.Random.value * 2.0f - 1.0f);
				gameplayInput.Jump              = UnityEngine.Random.value > 0.99f;
				gameplayInput.Attack            = UnityEngine.Random.value > 0.99f;
				gameplayInput.Reload            = UnityEngine.Random.value > 0.99f;
				gameplayInput.Interact          = UnityEngine.Random.value > 0.99f;
				gameplayInput.Weapon            = (byte)(UnityEngine.Random.value > 0.99f ? (UnityEngine.Random.value > 0.25f ? 2 : 1) : 0);

				networkInput.Set(gameplayInput);
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			if (_weapons  != null) { _weapons.OnDespawned();  }
			if (_jetpack  != null) { _jetpack.OnDespawned();  }
			if (_health   != null) { _health.OnDespawned();   }
			if (_agentVFX != null) { _agentVFX.OnDespawned(); }
		}

		public void EarlyFixedUpdateNetwork()
		{
			Profiler.BeginSample($"{nameof(Agent)}(Early)");

			ProcessFixedInput();

			_weapons.OnFixedUpdate();
			_jetpack.OnFixedUpdate();
			_character.OnFixedUpdate();

			Profiler.EndSample();
		}

		public override void FixedUpdateNetwork()
		{
			Profiler.BeginSample($"{nameof(Agent)}(Regular)");

			// Performance optimization, unnecessary euler call
			Quaternion currentLookRotation = _character.CharacterController.FixedData.LookRotation;
			if (_cachedLookRotation.ComponentEquals(currentLookRotation) == false)
			{
				_cachedLookRotation  = currentLookRotation;
				_cachedPitchRotation = Quaternion.Euler(_character.CharacterController.FixedData.LookPitch, 0.0f, 0.0f);
			}

			_character.GetCameraHandle().transform.localRotation = _cachedPitchRotation;

			CheckFallDamage();

			if (_health.IsAlive == true)
			{
				float sortOrder = _agentInput.FixedInput.LocalAlpha;
				if (sortOrder <= 0.0f)
				{
					// Default LocalAlpha value results in update callback being executed last.
					sortOrder = 1.0f;
				}

				// Schedule update to process render-accurate shooting.
				_sortedUpdateInvoker.ScheduleSortedUpdate(this, sortOrder);

				if (Runner.IsServer == true)
				{
					_interestView.SetPlayerInfo(_character.CharacterController.Transform, _character.GetCameraHandle());
				}
			}

			_health.OnFixedUpdate();

			Profiler.EndSample();
		}

		public void EarlyRender()
		{
			if (HasInputAuthority == true)
			{
				ProcessRenderInput();
			}

			_character.OnRender();
		}

		public override void Render()
		{
			if (HasInputAuthority == true || IsObserved == true)
			{
				// Performance optimization, unnecessary euler call
				Quaternion currentLookRotation = _character.CharacterController.RenderData.LookRotation;
				if (_cachedLookRotation.ComponentEquals(currentLookRotation) == false)
				{
					_cachedLookRotation  = currentLookRotation;
					_cachedPitchRotation = Quaternion.Euler(_character.CharacterController.RenderData.LookPitch, 0.0f, 0.0f);
				}

				_character.GetCameraHandle().transform.localRotation = _cachedPitchRotation;
			}

			_character.OnAgentRender();
			_footsteps.OnAgentRender();
		}

		// ISortedUpdate INTERFACE

		void ISortedUpdate.SortedUpdate()
		{
			// This method execution is sorted by LocalAlpha property passed in input and preserves realtime order of input actions.

			bool attackWasActivated   = _agentInput.WasActivated(EGameplayInputAction.Attack);
			bool reloadWasActivated   = _agentInput.WasActivated(EGameplayInputAction.Reload);
			bool interactWasActivated = _agentInput.WasActivated(EGameplayInputAction.Interact);

			TryFire(attackWasActivated, _agentInput.FixedInput.Attack);
			TryReload(reloadWasActivated == false);

			_interactions.TryInteract(interactWasActivated, _agentInput.FixedInput.Interact);
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_agentInput   = GetComponent<AgentInput>();
			_interactions = GetComponent<Interactions>();
			_footsteps    = GetComponent<AgentFootsteps>();
			_character    = GetComponent<Character>();
			_weapons      = GetComponent<Weapons>();
			_health       = GetComponent<Health>();
			_agentVFX     = GetComponent<AgentVFX>();
			_senses       = GetComponent<AgentSenses>();
			_jetpack      = GetComponent<Jetpack>();
			_interestView = GetComponent<AgentInterestView>();
		}

		// PRIVATE METHODS

		private void ProcessFixedInput()
		{
			KCC     kcc          = _character.CharacterController;
			KCCData kccFixedData = kcc.FixedData;

			GameplayInput input = default;

			if (_health.IsAlive == true)
			{
				input = _agentInput.FixedInput;
			}

			if (input.Aim == true)
			{
				input.Aim &= CanAim(kccFixedData);
			}

			if (input.Aim == true)
			{
				if (_weapons.CurrentWeapon != null && _weapons.CurrentWeapon.HitType == EHitType.Sniper)
				{
					input.LookRotationDelta *= 0.3f;
				}
			}

			kcc.SetAim(input.Aim);

            if (!this._health.IsAlive)
            {
                if (mecan != null)
                {
                    float FullBodyLayerWeight = Mathf.Lerp(0, 1f, 6f * 2f);
                    mecan.Animator.SetLayerWeight(mecan.Animator.GetLayerIndex("FullBody"), FullBodyLayerWeight);
                    mecan.Animator.SetBool("isDead", true);
                }
            }

			if (_agentInput.WasActivated(EGameplayInputAction.Jump, input) == true)
            {
                if (_character.AnimationController.CanJump() == true && jumpCount == 0)
                {
                    jumpCount++;

                    if (mecan != null)
                    {
                        cac.onGround = false;

                        //  mecan.Animator.SetBool("IsGrounded", false);
                        //  mecan.Animator.CrossFadeInFixedTime("Jump", 0.1f);
                        //mecan.Animator.SetBool("Jump", true);
                        //  if (input.MoveDirection.magnitude < 0.1f )
                        //	mecan.Animator.CrossFadeInFixedTime("Jump", 0.1f);
                        //else
                        //	mecan.Animator.CrossFadeInFixedTime("JumpMove", .2f);

                        //mecan.Animator.SetBool("IsGrounded", this.gameObject.GetComponentInParent<CharacterAnimationController>().onGround);

                    }

                    kcc.Jump(Vector3.up * _jumpPower);
                }
                else
                {
                    if (_agentInput.HasActive(EGameplayInputAction.Thrust) == true)
                    {
                        if (mecan != null)
                        {
                            mecan.Animator.SetBool("Fly", true);
                        }
                        ////Debug.Log("====Flying====");
                        _jetpack.Activate();
                        //RocketParticles.GetComponent<ParticleSystem>().enableEmission=true;
                    }
                    else
                    {
                        ////Debug.Log("====Deactive====");
                        _jetpack.Deactivate();
                        if (mecan != null)
                        {
                            mecan.Animator.SetBool("Fly", false);
                        }
                        //RocketParticles.GetComponent<ParticleSystem>().enableEmission=false;
                    }

                    Vector2 normalizedDirection = kccFixedData.JumpImpulse;

                    if (mecan != null)
                    {
                        if (mecan.Animator.GetBool("Jump") == true)
                        {
                            mecan.Animator.SetBool("Jump", false);
                        }
                    }

                }
            }
            else
            {
                if (_agentInput.HasActive(EGameplayInputAction.Thrust) == false && jumpCount > 0 && Runner.IsForward)
                {
                    ////Debug.Log("====Deactive====");
                    if (mecan != null)
                    {
                        mecan.Animator.SetBool("Fly", false);
                    }

                    _jetpack.Deactivate();
                }

                Vector2 normalizedDirection = kccFixedData.JumpImpulse;
                if (mecan != null && Runner.IsForward)
                {
                    if (mecan.Animator.GetBool("Jump") == true)
                    {
                        mecan.Animator.SetBool("Jump", false);
                    }
                }

                //RocketParticles.GetComponent<ParticleSystem>().enableEmission=false;
            }
            if (_agentInput.HasActive(EGameplayInputAction.Crouch) == true)
            {
                if (mecan != null)
                {

                    mecan.Animator.SetBool("IsCrouching", true);
                }
                ////Debug.Log("Crouching" + kccFixedData.HasCrouch);
            }
            else
            {
                if (mecan != null)
                {
                    mecan.Animator.SetBool("IsCrouching", false);
                }
            }

			/*if (_agentInput.WasActivated(EGameplayInputAction.Jump, input) == true && _character.AnimationController.CanJump() == true)
			{
				kcc.Jump(Vector3.up * _jumpPower);
			}*/

			SetLookRotation(kccFixedData, input.LookRotationDelta, _weapons.GetRecoil(), out Vector2 newRecoil);
			_weapons.SetRecoil(newRecoil);
			

			kcc.SetInputDirection(input.MoveDirection.IsZero() == true ? Vector3.zero : kcc.FixedData.TransformRotation * input.MoveDirection.X0Y());


			 StrafeLimitSpeed(1, input.MoveDirection);
            if (Runner.IsForward && mecan != null)
            {
                if (input.MoveDirection.magnitude > 0.1f)
                {
                    Vector2 normalizedDirection = input.MoveDirection.normalized;

                    mecan.Animator.SetFloat("InputMagnitude", strafeMagnitude);
                    mecan.Animator.SetFloat("InputVertical", inputCharacter.y);
                    mecan.Animator.SetFloat("InputHorizontal", inputCharacter.x);

                    bool isMoving = false;

                    if (Vector2.Dot(normalizedDirection, Vector2.up) > 0.5f)
                    {
                        isMoving = true;
                    }
                    else if (Vector2.Dot(normalizedDirection, Vector2.down) > 0.5f)
                    {
                        isMoving = true;
                    }
                    else if (Vector2.Dot(normalizedDirection, Vector2.right) > 0.5f)
                    {
                        isMoving = true;
                    }
                    else if (Vector2.Dot(normalizedDirection, Vector2.left) > 0.5f)
                    {
                        isMoving = true;
                    }

                    if (_agentInput.HasActive(EGameplayInputAction.ToggleSpeed) && isMoving && _jetpack._fuel > 0)
                    {
						Debug.Log("ACELERANDO");
                        IsActive = true;
                       // kcc.AddModifier(_fastMovementProcessor);
						_character.CharacterController.SetSpeedMultiplier(2f);
                        mecan.Animator.SetFloat("InputMagnitude", strafeMagnitude * 2);
                        SprintAction?.Invoke(true);
                        //_jetpack.externalConsumption();
                    }
                    else
                    {
                        
                            IsActive = false;
							
                            _character.CharacterController.SetSpeedMultiplier(1f);
                            mecan.Animator.SetFloat("InputMagnitude", strafeMagnitude * 1);
                            SprintAction?.Invoke(false);
                      
                        
                    }
                }
                else
                {
                    IsActive = false;
                    mecan.Animator.SetFloat("InputMagnitude", strafeMagnitude);
                    mecan.Animator.SetFloat("InputVertical", inputCharacter.y);
                    mecan.Animator.SetFloat("InputHorizontal", inputCharacter.x);
                    _character.CharacterController.SetSpeedMultiplier(1f);
                }

             

                if (this.GetComponent<Weapons>().CurrentWeaponSlot == 1)
                {
                    if (!pistolEquip)
                    {
                        gunEquip = false;
                        pistolEquip = true;
                        //mecan.Animator.CrossFadeInFixedTime("LowBack", 0.1f);

                    }
                    //StartCoroutine(Equip(input));
                    mecan.Animator.SetBool("CanAim", true);
                    mecan.Animator.SetFloat("UpperBody_ID", 1f, .2f, Time.deltaTime);
                    mecan.Animator.SetFloat("Shot_ID", 1);
                    //mecan.Animator.SetBool("IsAiming", true);
                    onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 1f, 6f * Time.deltaTime);
                    mecan.Animator.SetLayerWeight(mecan.Animator.GetLayerIndex("OnlyArms"), onlyArmsLayerWeight);

                    //					this.gameObject.GetComponent<CharacterAnimationController>().angle = 30f;

                }
                else if (this.GetComponent<Weapons>().CurrentWeaponSlot == 2)
                {
                    mecan.Animator.SetBool("CanAim", true);
                    if (!gunEquip)
                    {
                        gunEquip = true;
                        pistolEquip = false;
                        //mecan.Animator.CrossFadeInFixedTime("HighBack", 0.1f);

                    }
                    mecan.Animator.SetFloat("UpperBody_ID", 2f, .2f, Time.deltaTime);
                    mecan.Animator.SetFloat("Shot_ID", 2);
                    //mecan.Animator.SetBool("IsAiming", true);
                    // turn on the onlyarms layer to aim 
                    onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 1f, 6f * Time.deltaTime);
                    mecan.Animator.SetLayerWeight(mecan.Animator.GetLayerIndex("OnlyArms"), onlyArmsLayerWeight);
                    //				this.gameObject.GetComponent<CharacterAnimationController>().angle = 20f;

                }
                else
                {
                    pistolEquip = false;
                    gunEquip = false;
                    mecan.Animator.SetBool("CanAim", false);
                    //StartCoroutine(Equip(input));
                    mecan.Animator.SetFloat("UpperBody_ID", 0f, .2f, Time.deltaTime);
                    mecan.Animator.SetFloat("Shot_ID", 0);
                    //mecan.Animator.SetBool("IsAiming", true);
                    onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 0f, 6f * Time.deltaTime);
                    mecan.Animator.SetLayerWeight(mecan.Animator.GetLayerIndex("OnlyArms"), onlyArmsLayerWeight);
                    mecan.Animator.SetFloat("MoveSet_ID", 0f, .1f, Time.deltaTime);
                }
                if (this.GetComponent<Weapons>().CurrentWeaponSlot == 3)
                {
                    //StartCoroutine(Equip(input));
                    mecan.Animator.CrossFadeInFixedTime("HoldingObject", 0.2f);

                    //mecan.Animator.SetBool("throw", true);
                }
                if (this.GetComponent<Weapons>().CurrentWeaponSlot == 0)
                {
                    mecan.Animator.SetBool("CanAim", false);
                    mecan.Animator.SetFloat("UpperBody_ID", 0f, .2f, Time.deltaTime);
                    mecan.Animator.SetFloat("Shot_ID", 0);
                    mecan.Animator.SetBool("IsAiming", false);
                    onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 0f, 6f * Time.deltaTime);
                    mecan.Animator.SetLayerWeight(mecan.Animator.GetLayerIndex("OnlyArms"), onlyArmsLayerWeight);
                    mecan.Animator.SetFloat("MoveSet_ID", 0f, .1f, Time.deltaTime);
                    cac.angle = 0f;
                    //if (_agentInput.HasActive(EGameplayInputAction.Attack)) 
                    //{
                    //	mecan.Animator.SetBool("punch", true);
                    //	leftHand.GetComponent<SphereCollider>().enabled = true;
                    //	rightHand.GetComponent<SphereCollider>().enabled = true;
                    //}
                    if (_agentInput.WasDeactivated(EGameplayInputAction.Attack, input))
                    {
                        mecan.Animator.SetBool("punch", true);
                        leftHand.GetComponent<SphereCollider>().enabled = true;
                        rightHand.GetComponent<SphereCollider>().enabled = true;

                        leftHand.GetComponent<DisableDelay>().DisableCollider();
                        rightHand.GetComponent<DisableDelay>().DisableCollider();
                    }

                    if (_agentInput.HasActive(EGameplayInputAction.Emote) == true)
                    {
                        int i = 0;
                        i = Random.Range(0, 2);
                        if (i == 0)
                        {
                            if (mecan != null)
                            {
                                mecan.Animator.SetBool("emote1", true);
                            }
                        }
                        if (i == 1)
                        {
                            if (mecan != null)
                            {
                                mecan.Animator.SetBool("emote2", true);
                            }

                        }
                        ////Debug.Log("Crouching" + kccFixedData.HasCrouch);
                    }
                    else
                    {
                        if (mecan != null)
                        {
                            mecan.Animator.SetBool("emote1", false);
                            mecan.Animator.SetBool("emote2", false);
                        }

                    }
                    //}
                }
                if (_agentInput.HasActive(EGameplayInputAction.Attack))
                {
                    //if()
                    ////StopCoroutine(Shoot());
                    //StartCoroutine(Shoot());
                }
                //else
                //{

                //}
                //            else if(_agentInput.WasDeactivated(EGameplayInputAction.Attack, input))S
                //            {
                //	mecan.Animator.SetBool("IsAiming", false);
                //	mecan.Animator.SetFloat("MoveSet_ID", 0, .1f, Time.deltaTime);S
                //}
                //if (mecan.Animator.GetBool("gun") == false && mecan.Animator.GetBool("pistol") == false && this.GetComponent<Weapons>().CurrentWeaponSlot != 0)
                //{

                //if (input.MoveDirection.magnitude <= 0.1f)S
                //if (_agentInput.HasActive(EGameplayInputAction.Attack))
                //	mecan.Animator.SetBool("throwarm", true);
                //}

            }



            ////Debug.Log("Pressed Spry" + _character.CharacterController.name);


            //if (_agentInput.WasActivated(EGameplayInputAction.ToggleSpeed, input) == true)


            // SPRAY  -----------------------------------------------------------
            if (_agentInput.WasActivated(EGameplayInputAction.Spray, input) == true)
            {
                PlayerRef playerRef = Context.ObservedPlayerRef;
                TransformData trans = GameObject.Find(playerRef.ToString()).GetComponent<Agent>()._character.GetCameraTransform(false);
                ////Debug.Log("Spray Player Position: " + trans.Position);
                LayerMask layerMask = LayerMask.GetMask("Default");
                if (Runner.GetPhysicsScene().Raycast(trans.Position, trans.Rotation * Vector3.forward * 30, out RaycastHit hitInfo, 30f, layerMask) == true)
                {



                    Debug.Log("Spray Hit Info: " + hitInfo.collider.name);
                    if (sprayCount < sprayMax)
                    {
                        OnSpray?.Invoke();
                        RPC_Spray(hitInfo.point, hitInfo.normal);

                    }
                    else
                    {
                        UIGameplayEvents eventUI = GameObject.Find("GameplayEvents").GetComponent<UIGameplayEvents>();
                        eventUI.ShowEvent(new GameplayEventData
                        {
                            Name = "Tagging...",
                            Description = "Tag limit reached, refilling in 30 seconds",
                            Color = Color.red,
                            Sound = null,
                        }, false, true);
                        StartCoroutine(replenishTags());
                    }

                    //NetworkObject spray = Context.Runner.Spawn(sprayPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal * -1), Object.InputAuthority);
                }
            }

			if (_agentInput.WasActivated(EGameplayInputAction.ToggleSide, input) == true)
			{
				LeftSide = !LeftSide;
			}

			if (input.Weapon > 0 && _character.AnimationController.CanSwitchWeapons(true) == true && _weapons.SwitchWeapon(input.Weapon - 1) == true)
			{
				_character.AnimationController.SwitchWeapons();
			}
			else if (input.Weapon <= 0 && _weapons.PendingWeaponSlot != _weapons.CurrentWeaponSlot && _character.AnimationController.CanSwitchWeapons(false) == true)
			{
				_character.AnimationController.SwitchWeapons();
			}

			if (_agentInput.WasActivated(EGameplayInputAction.ToggleJetpack, input) == true)
			{
				if (_jetpack.IsActive == true)
				{
					_jetpack.Deactivate();
				}
				else if (_character.AnimationController.CanSwitchWeapons(true) == true)
				{
					_jetpack.Activate();
				}
			}

			if (_jetpack.IsActive == true)
			{
				_jetpack.FullThrust = input.Thrust;
			}

			_agentInput.SetFixedInput(input, false);
		}

		public void StrafeLimitSpeed(float value, Vector2 inputChar)
        {
            inputCharacter.x = inputChar.x; //Input.GetAxis("Horizontal");
            inputCharacter.y = inputChar.y; //Input.GetAxis("Vertical");
                                            //Debug.Log("_agentInput.HasActive(EGameplayInputAction.ToggleSpeed) : " + _agentInput.HasActive(EGameplayInputAction.ToggleSpeed));
            var limitInput = (_agentInput.HasActive(EGameplayInputAction.ToggleSpeed) && inputCharacter.y > 0 && inputCharacter.x == 0) ? value + 0.5f : value;
            //Debug.Log("limitInput : " + limitInput);
            var _input = inputCharacter * limitInput;
            var _speed = Mathf.Clamp(_input.y, -limitInput, limitInput);
            var _direction = Mathf.Clamp(_input.x, -limitInput, limitInput);
            //speed = _speed;
            //direction = _direction;
            var newInput = new Vector2(_speed, _direction);
            strafeMagnitude = Mathf.Clamp(newInput.magnitude, 0, limitInput);
            if (strafeMagnitude < -1)
            {
                strafeMagnitude = 1;
            }
            //Debug.Log("strafeMagnitude : " + strafeMagnitude);
        }
        private IEnumerator replenishTags()
        {

            yield return new WaitForSeconds(30);
            sprayCount = 0;

        }

		private void ProcessRenderInput()
		{
			KCC     kcc           = _character.CharacterController;
			KCCData kccFixedData  = kcc.FixedData;

			GameplayInput input = default;

			if (_health.IsAlive == true)
			{
				input = _agentInput.RenderInput;

				var accumulatedInput = _agentInput.AccumulatedInput;

				input.LookRotationDelta = accumulatedInput.LookRotationDelta;
				input.Aim               = accumulatedInput.Aim;
				input.Thrust            = accumulatedInput.Thrust;
			}

			if (input.Aim == true)
			{
				input.Aim &= CanAim(kccFixedData);
			}

			if (input.Aim == true)
			{
				if (_weapons.CurrentWeapon != null && _weapons.CurrentWeapon.HitType == EHitType.Sniper)
				{
					input.LookRotationDelta *= 0.3f;
				}
			}

			SetLookRotation(kccFixedData, input.LookRotationDelta, _weapons.GetRecoil(), out Vector2 newRecoil);

			kcc.SetInputDirection(input.MoveDirection.IsZero() == true ? Vector3.zero : kcc.RenderData.TransformRotation * input.MoveDirection.X0Y());

			kcc.SetAim(input.Aim);

			if (_agentInput.WasActivated(EGameplayInputAction.Jump, input) == true && _character.AnimationController.CanJump() == true)
			{
				kcc.Jump(Vector3.up * _jumpPower);
			}
		}

		private void TryFire(bool attack, bool hold)
		{
			var currentWeapon = _weapons.CurrentWeapon;
			if (currentWeapon is ThrowableWeapon && currentWeapon.WeaponSlot == _weapons.PendingWeaponSlot)
			{
				// Fire is handled form the grenade animation state itself
				_character.AnimationController.ProcessThrow(attack, hold);
				return;
			}

			if (hold == false)
				return;
			if (_weapons.CanFireWeapon(attack) == false)
				return;

			if (_character.AnimationController.StartFire() == true)
			{
				if (_weapons.Fire() == true)
				{
					_health.ResetRegenDelay();

					if (Runner.IsServer == true)
					{
						PlayerRef inputAuthority = Object.InputAuthority;
						if (inputAuthority.IsRealPlayer == true)
						{
							_interestView.UpdateShootInterestTargets();
						}
					}
				}
			}
		}

		private void TryReload(bool autoReload)
		{
			if (_weapons.CanReloadWeapon(autoReload) == false)
				return;

			if (_character.AnimationController.StartReload() == true)
			{
				_weapons.Reload();
			}
		}

		[Rpc]
        private void RPC_Spray(Vector3 point, Vector3 normal)
        {
            //Debug.Log("Spray Position: " + point);
            sprayCount++;
            NetworkBehaviour networkBehaviour;
            if (tags.Length > 0)
            {
                GameObject Prefab = tags[Random.Range(0, tags.Length)];
                networkBehaviour = Prefab.GetComponent<NetworkBehaviour>();

            }
            else
            {
                networkBehaviour = sprayPrefab.GetComponent<NetworkBehaviour>();
            }
            NetworkBehaviour spray = Runner.Spawn(networkBehaviour, point, Quaternion.LookRotation(normal * -1), Object.StateAuthority);
        }

          [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
        private void RPC_PushAway(Vector3 direction, float amount)
        {
            KCC kcc = _character.CharacterController;
            kcc.SetExternalImpulse(direction * (amount / 2));
            Context.Camera.ShakeEffect.Play(EShakeForce.ReplaceSame);
        }

		private bool CanAim(KCCData kccData)
		{
			if (kccData.IsGrounded == false)
				return false;

			return _weapons.CanAim();
		}

		private void SetLookRotation(KCCData kccData, Vector2 lookRotationDelta, Vector2 recoil, out Vector2 newRecoil)
		{
			if (lookRotationDelta.IsZero() == true && recoil.IsZero() == true && _character.CharacterController.Data.Recoil == Vector2.zero)
			{
				newRecoil = recoil;
				return;
			}

			Vector2 baseLookRotation = kccData.GetLookRotation(true, true) - kccData.Recoil;
			Vector2 recoilReduction  = Vector2.zero;

			if (recoil.x > 0f && lookRotationDelta.x < 0)
			{
				recoilReduction.x = Mathf.Clamp(lookRotationDelta.x, -recoil.x, 0f);
			}

			if (recoil.x < 0f && lookRotationDelta.x > 0f)
			{
				recoilReduction.x = Mathf.Clamp(lookRotationDelta.x, 0, -recoil.x);
			}

			if (recoil.y > 0f && lookRotationDelta.y < 0)
			{
				recoilReduction.y = Mathf.Clamp(lookRotationDelta.y, -recoil.y, 0f);
			}

			if (recoil.y < 0f && lookRotationDelta.y > 0f)
			{
				recoilReduction.y = Mathf.Clamp(lookRotationDelta.y, 0, -recoil.y);
			}

			lookRotationDelta -= recoilReduction;
			recoil            += recoilReduction;

			lookRotationDelta.x = Mathf.Clamp(baseLookRotation.x + lookRotationDelta.x, -_topCameraAngleLimit, _bottomCameraAngleLimit) - baseLookRotation.x;

			_character.CharacterController.SetLookRotation(baseLookRotation + recoil + lookRotationDelta);
			_character.CharacterController.SetRecoil(recoil);

			_character.AnimationController.Turn(lookRotationDelta.y);

			newRecoil = recoil;
		}

		private void CheckFallDamage()
		{
			if (IsProxy == true)
				return;

			if (_health.IsAlive == false)
				return;

			var kccData = _character.CharacterController.Data;

			if (kccData.IsGrounded == false || kccData.WasGrounded == true)
				return;

			float fallVelocity = -kccData.DesiredVelocity.y;
			for (int i = 1; i < 3; ++i)
			{
				var historyData = _character.CharacterController.GetHistoryData(kccData.Tick - i);
				if (historyData != null)
				{
					fallVelocity = Mathf.Max(fallVelocity, -historyData.DesiredVelocity.y);
				}
			}

			if (fallVelocity < 0f)
				return;

			float damage = MathUtility.Map(_minFallDamageVelocity, _maxFallDamageVelocity, 0f, _maxFallDamage, fallVelocity);

			if (damage <= _minFallDamage)
				return;

			var hitData = new HitData
			{
				Action           = EHitAction.Damage,
				Amount           = damage,
				Position         = transform.position,
				Normal           = Vector3.up,
				Direction        = -Vector3.up,
				InstigatorRef    = Object.InputAuthority,
				Instigator       = _health,
				Target           = _health,
				HitType          = EHitType.Suicide,
			};

			(_health as IHitTarget).ProcessHit(ref hitData);
		}

		private void OnCullingUpdated(bool isCulled)
		{
			bool isActive = isCulled == false;

			// Show/hide the game object based on AoI (Area of Interest)

			_visualRoot.SetActive(isActive);

			if (_character.CharacterController.Collider != null)
			{
				_character.CharacterController.Collider.enabled = isActive;
			}
		}

		private void changeSignState(UserData userData){
            CryptoManager.OnUserData  -= changeSignState;
            InventoryManager.GetInventory();
        }
	}
}
