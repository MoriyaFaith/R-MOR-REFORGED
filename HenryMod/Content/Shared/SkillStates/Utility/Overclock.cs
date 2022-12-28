using RMORMod.Content.HANDSurvivor;
using RMORMod.Content.Shared.Components.Body;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.RMOR.Utility
{
	public class BeginOverclock : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();

			LoadStats();

            modelAnimator = base.GetModelAnimator();
            if (modelAnimator)
            {
                this.PlayAnimation("Overclock, Override", "SprintFWD");
            }

			this.overclockController = base.gameObject.GetComponent<OverclockController>();
			if (base.isAuthority)
			{
				if (base.characterMotor && !base.characterMotor.isGrounded && BeginOverclock.shortHopVelocity > 0f)
				{
					base.SmallHop(base.characterMotor, BeginOverclock.shortHopVelocity);
				}
				if(overclockController)
				{
					overclockController.StartOverclock(gaugeInternal, gaugeArrowInternal);
				}
			}

			if (NetworkServer.active)
			{
				BuffDef selectedBuff = buffDef;
				if (base.characterBody && !base.characterBody.HasBuff(selectedBuff))
				{
					base.characterBody.AddBuff(selectedBuff);
				}
			}

			Util.PlaySound(startSoundString, base.gameObject);

			this.skillSlot = (base.skillLocator ? base.skillLocator.utility : null);
			if (this.skillSlot)
			{
				startStocks = this.skillSlot.stock;
				this.skillSlot.SetSkillOverride(this, cancelDef, GenericSkill.SkillOverridePriority.Contextual);
				this.skillSlot.stock = Mathf.Min(skillSlot.maxStock, startStocks + 1);
			}

			jetFireTime = 1f / BeginOverclock.jetFireFrequency;
			jetStopwatch = 0f;
			ChildLocator cl = base.GetModelChildLocator();
			if (cl)
			{
				leftJet = cl.FindChild("Jetpack.L");
                rightJet = cl.FindChild("Jetpack.R");

                GameObject leftEffect = UnityEngine.Object.Instantiate<GameObject>(BeginOverclock.jetEffectPrefab, leftJet);
				leftEffect.transform.localRotation *= Quaternion.Euler(0f, -60f, 0f);
				leftEffect.transform.localPosition += new Vector3(0f, 0.6f, 0f);    //Adding to this shifts it downwards.

				GameObject rightEffect = UnityEngine.Object.Instantiate<GameObject>(BeginOverclock.jetEffectPrefab, rightJet);
				rightEffect.transform.localRotation *= Quaternion.Euler(0f, 60f, 0f);
				rightEffect.transform.localPosition += new Vector3(0f, 0.6f, 0f);
			}

			if (internalOverlayMaterial)
			{
				if (base.modelLocator && base.modelLocator.modelTransform && base.modelLocator.modelTransform.gameObject)
				{
					characterModel = base.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
					if (characterModel)
					{
						tempOverlay = characterModel.gameObject.AddComponent<TemporaryOverlay>();
						tempOverlay.duration = Mathf.Infinity;
						tempOverlay.animateShaderAlpha = true;
						tempOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
						tempOverlay.destroyComponentOnEnd = true;
						tempOverlay.originalMaterial = internalOverlayMaterial;
						tempOverlay.AddToCharacerModel(characterModel);
					}
				}
			}
		}

		public virtual void LoadStats()
        {
			cancelDef = RMORMod.Content.Shared.SkillDefs.UtilityOverclockCancel;
			buffDef = RMORMod.Content.Shared.Buffs.Overclock;
			gaugeInternal = BeginOverclock.texGauge;
			gaugeArrowInternal = BeginOverclock.texGaugeArrow;
			internalOverlayMaterial = BeginOverclock.overlayMaterial;
        }

		public virtual float ExtendBuff(float stopwatch, float extensionTime)
        {
			return Mathf.Max(0f, stopwatch - extensionTime);
        }

		public override void OnExit()
		{
			if (this.skillSlot)
			{
				this.skillSlot.UnsetSkillOverride(this, cancelDef, GenericSkill.SkillOverridePriority.Contextual);
				this.skillSlot.stock = startStocks;
			}

			if (NetworkServer.active)
            {
				if (base.characterBody && base.characterBody.HasBuff(buffDef))
                {
					base.characterBody.RemoveBuff(buffDef);
                }
            }

			if (tempOverlay)
            {
				tempOverlay.RemoveFromCharacterModel();
				UnityEngine.Object.Destroy(tempOverlay);
				tempOverlay = null;
            }

			if (base.isAuthority)
            {
				if (overclockController) overclockController.EndOverclock();
            }

			Util.PlaySound(endSoundString, base.gameObject);
            if (modelAnimator)
            {
                this.PlayAnimation("Overclock, Override", "BufferEmpty");
            }
            base.OnExit();
		}

        public override void FixedUpdate()
		{
			base.FixedUpdate();

			jetStopwatch += Time.fixedDeltaTime;
			if (jetStopwatch >= jetFireTime)
			{
				jetStopwatch -= jetFireTime;

				GameObject leftEffect = UnityEngine.Object.Instantiate<GameObject>(BeginOverclock.jetEffectPrefab, leftJet);
				leftEffect.transform.localRotation *= Quaternion.Euler(0f, -60f, 0f);
				leftEffect.transform.localPosition += new Vector3(0f, 0.6f, 0f);    //Adding to this shifts it downwards.

				GameObject rightEffect = UnityEngine.Object.Instantiate<GameObject>(BeginOverclock.jetEffectPrefab, rightJet);
				rightEffect.transform.localRotation *= Quaternion.Euler(0f, 60f, 0f);
				rightEffect.transform.localPosition += new Vector3(0f, 0.6f, 0f);
			}

			if (base.isAuthority)
			{
				stopwatch += Time.fixedDeltaTime;
				if (overclockController)
				{
					stopwatch = ExtendBuff(stopwatch, overclockController.ConsumeExtensionTime());
					overclockController.buffPercent = Mathf.Max(0f, (buffDuration - stopwatch)) / buffDuration;
				}

				if (onAuthorityFixedUpdateGlobal != null) onAuthorityFixedUpdateGlobal.Invoke(this);
                
				if (!this.skillSlot || this.skillSlot.stock == 0 || stopwatch >= buffDuration)
				{
					this.beginExit = true;
				}
				if (this.beginExit)
				{
					this.timerSinceComplete += Time.fixedDeltaTime;
					if (this.timerSinceComplete > BeginOverclock.baseExitDuration)
					{
						this.outer.SetNextStateToMain();
					}
				}
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
        }

        //Used for achievement.
        public static event Action<BeginOverclock> onAuthorityFixedUpdateGlobal;

        public float buffDuration = 4f;
		public BuffDef buffDef;
		public string startSoundString = "Play_MULT_shift_start";
		public string endSoundString = "Play_MULT_shift_end";
		public SkillDef cancelDef;

        private Animator modelAnimator;
		private float stopwatch = 0f;
		private float jetFireTime;
		private float jetStopwatch;
		private float timerSinceComplete = 0f;
		private bool beginExit;
		private int startStocks = 0;
		private Transform leftJet;
		private Transform rightJet;
		private TemporaryOverlay tempOverlay;
		private CharacterModel characterModel;

		public static GameObject jetEffectPrefab;
		public static float baseExitDuration = 0.3f;
		public static float shortHopVelocity = 12f;
		public static float jetFireFrequency = 6f;

		public Material internalOverlayMaterial;
		public static Material overlayMaterial;

		public OverclockController overclockController;
		private GenericSkill skillSlot;

		public Texture2D gaugeInternal, gaugeArrowInternal;

		public static Texture2D texGauge, texGaugeArrow;
	}

	public class CancelOverclock : BaseState
	{
		public static float shortHopVelocity = 24f;
		public static GameObject jetEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoDashJets.prefab").WaitForCompletion();
		protected OverclockController overclockController;

		public override void OnEnter()
		{
			base.OnEnter();

			Util.PlaySound("Play_commando_shift", base.gameObject);

			ChildLocator cl = base.GetModelChildLocator();
			if (cl)
			{
				Transform leftJet = cl.FindChild("Jetpack.L");
				Transform rightJet = cl.FindChild("Jetpack.R");

				GameObject leftEffect = UnityEngine.Object.Instantiate<GameObject>(CancelOverclock.jetEffectPrefab, leftJet);
				leftEffect.transform.localRotation *= Quaternion.Euler(-60f, -90f, -60f);
				leftEffect.transform.localPosition += new Vector3(0f, 0.6f, 0f);    //Adding to this shifts it downwards.

				GameObject rightEffect = UnityEngine.Object.Instantiate<GameObject>(CancelOverclock.jetEffectPrefab, rightJet);
				rightEffect.transform.localRotation *= Quaternion.Euler(-60f, 90f, -60f);
				rightEffect.transform.localPosition += new Vector3(0f, 0.6f, 0f);
			}

			if (base.isAuthority)
			{
				if (base.characterMotor != null)    //Manually exiting will always trigger the shorthop regardless of grounded status.
				{
					base.SmallHop(base.characterMotor, CancelOverclock.shortHopVelocity);
				}
				this.outer.SetNextStateToMain();
			}

		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
