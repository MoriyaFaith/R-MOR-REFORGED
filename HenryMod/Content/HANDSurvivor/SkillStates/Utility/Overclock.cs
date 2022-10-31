using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.HANDSurvivor.Components.Body;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.HAND_Overclocked.Utility
{
    public class BeginOverclock : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.overclockController = base.gameObject.GetComponent<OverclockController>();
			if (base.isAuthority)
			{
				if (base.characterMotor && !base.characterMotor.isGrounded && BeginOverclock.shortHopVelocity > 0f)
				{
					base.SmallHop(base.characterMotor, BeginOverclock.shortHopVelocity);
				}
				StartOverclock();
			}

			this.skillSlot = (base.skillLocator ? base.skillLocator.utility : null);
			if (this.skillSlot)
			{
				startStocks = this.skillSlot.stock;
				this.skillSlot.SetSkillOverride(this, GetCancelDef(), GenericSkill.SkillOverridePriority.Contextual);
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
		}
		public virtual SkillDef GetCancelDef()
        {
			return SkillDefs.UtilityOverclockCancel;
		}
		public virtual void StartOverclock()
		{
			if (this.overclockController)
			{
				this.overclockController.BeginOverclock();
			}
		}

		public override void OnExit()
		{
			if (this.skillSlot)
			{
				this.skillSlot.UnsetSkillOverride(this, GetCancelDef(), GenericSkill.SkillOverridePriority.Contextual);
				this.skillSlot.stock = startStocks;
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

			if ((!this.skillSlot || this.skillSlot.stock == 0) || !(overclockController && overclockController.buffActive))
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

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		private float jetFireTime;
		private float jetStopwatch;
		private float timerSinceComplete = 0f;
		private bool beginExit;
		private int startStocks = 0;
		private Transform leftJet;
		private Transform rightJet;

		public static GameObject jetEffectPrefab;
		public static float baseExitDuration = 0.3f;
		public static float shortHopVelocity = 12f;
		public static float jetFireFrequency = 6f;

		protected OverclockController overclockController;
		private GenericSkill skillSlot;

		public static Texture2D texGauge;
		public static Texture2D texGaugeArrow;
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
				leftEffect.transform.localPosition += new Vector3(0f, 0.6f, 0f);	//Adding to this shifts it downwards.

				GameObject rightEffect = UnityEngine.Object.Instantiate<GameObject>(CancelOverclock.jetEffectPrefab, rightJet);
				rightEffect.transform.localRotation *= Quaternion.Euler(-60f, 90f, -60f);
				rightEffect.transform.localPosition += new Vector3(0f, 0.6f, 0f);
			}

			overclockController = base.gameObject.GetComponent<OverclockController>();
			if (base.isAuthority)
			{
				if (base.characterMotor != null)    //Manually exiting will always trigger the shorthop regardless of grounded status.
				{
					base.SmallHop(base.characterMotor, CancelOverclock.shortHopVelocity);
				}
				EndOverclock();
				this.outer.SetNextStateToMain();
			}

		}

		public virtual void EndOverclock()
		{
			if (overclockController)
			{
				overclockController.EndOverclock();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
