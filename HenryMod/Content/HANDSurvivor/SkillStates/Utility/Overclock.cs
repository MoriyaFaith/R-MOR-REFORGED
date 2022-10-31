using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.HANDSurvivor.Components.Body;
using RoR2;
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
				if (this.overclockController)
				{
					this.overclockController.BeginOverclock();
				}
			}

			this.skillSlot = (base.skillLocator ? base.skillLocator.utility : null);
			if (this.skillSlot)
			{
				startStocks = this.skillSlot.stock;
				this.skillSlot.SetSkillOverride(this, SkillDefs.UtilityOverclockCancel, GenericSkill.SkillOverridePriority.Contextual);
				this.skillSlot.stock = Mathf.Min(skillSlot.maxStock, startStocks + 1);
			}
		}

		public override void OnExit()
		{
			if (this.skillSlot)
			{
				this.skillSlot.UnsetSkillOverride(this, SkillDefs.UtilityOverclockCancel, GenericSkill.SkillOverridePriority.Contextual);
				this.skillSlot.stock = startStocks;
			}
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if ((!this.skillSlot || this.skillSlot.stock == 0) || !(overclockController && overclockController.ovcActive))
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

		private float timerSinceComplete = 0f;
		private bool beginExit;
		private int startStocks = 0;

		public static float baseExitDuration = 0.3f;
		public static float shortHopVelocity = 0f;
		private OverclockController overclockController;
		private GenericSkill skillSlot;
	}

	public class CancelOverclock : BaseState
	{
		public static float shortHopVelocity = 30f;
		public static GameObject jetEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoDashJets.prefab").WaitForCompletion();

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
				if (overclockController)
				{
					overclockController.EndOverclock();
				}
				this.outer.SetNextStateToMain();
			}

		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		private OverclockController overclockController;
	}
}
