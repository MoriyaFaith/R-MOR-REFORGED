using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using UnityEngine;
using EntityStates;
using EntityStates.Engi.EngiMissilePainter;
using RMORMod.Modules;

namespace EntityStates.RMOR.Special
{
	public class LockOn : BaseLockOnMissileState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				this.targetsList = new List<HurtBox>();
				this.targetIndicators = new Dictionary<HurtBox, IndicatorInfo>();
				this.stickyTargetIndicator = new Indicator(base.gameObject, Paint.stickyTargetIndicatorPrefab);
				this.search = new BullseyeSearch();
			}
			base.PlayCrossfade("Gesture, Additive", "PrepHarpoons", 0.1f);
			Util.PlaySound(Paint.enterSoundString, base.gameObject);
			this.loopSoundID = Util.PlaySound(Paint.loopSoundString, base.gameObject);
			{
				this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, Assets.LoadCrosshair("Railgunner"), CrosshairUtils.OverridePriority.Skill);
			}
			this.engiConfirmTargetDummySkillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("EngiConfirmTargetDummy"));
			this.engiCancelTargetingDummySkillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("EngiCancelTargetingDummy"));
			base.skillLocator.primary.SetSkillOverride(this, this.engiConfirmTargetDummySkillDef, GenericSkill.SkillOverridePriority.Contextual);
			base.skillLocator.secondary.SetSkillOverride(this, this.engiCancelTargetingDummySkillDef, GenericSkill.SkillOverridePriority.Contextual);
		}

		public override void OnExit()
		{
			if (base.isAuthority && !this.outer.destroying && !this.queuedFiringState)
			{
				for (int i = 0; i < this.targetsList.Count; i++)
				{
					base.activatorSkillSlot.AddOneStock();
				}
			}
			base.skillLocator.secondary.UnsetSkillOverride(this, this.engiCancelTargetingDummySkillDef, GenericSkill.SkillOverridePriority.Contextual);
			base.skillLocator.primary.UnsetSkillOverride(this, this.engiConfirmTargetDummySkillDef, GenericSkill.SkillOverridePriority.Contextual);
			if (this.targetIndicators != null)
			{
				foreach (KeyValuePair<HurtBox, IndicatorInfo> keyValuePair in this.targetIndicators)
				{
					keyValuePair.Value.indicator.active = false;
				}
			}
			if (this.stickyTargetIndicator != null)
			{
				this.stickyTargetIndicator.active = false;
			}
			CrosshairUtils.OverrideRequest overrideRequest = this.crosshairOverrideRequest;
			if (overrideRequest != null)
			{
				overrideRequest.Dispose();
			}
			base.PlayCrossfade("Gesture, Additive", "ExitHarpoons", 0.1f);
			Util.PlaySound(Paint.exitSoundString, base.gameObject);
			Util.PlaySound(Paint.stopLoopSoundString, base.gameObject);
			base.OnExit();
		}

		private void AddTargetAuthority(HurtBox hurtBox)
		{
            if (base.activatorSkillSlot.stock == 0)
            {
                return;
            }
            Util.PlaySound(Paint.lockOnSoundString, base.gameObject);
			this.targetsList.Add(hurtBox);
			IndicatorInfo indicatorInfo;
			if (!this.targetIndicators.TryGetValue(hurtBox, out indicatorInfo))
			{
				indicatorInfo = new IndicatorInfo
				{
					refCount = 0,
					indicator = new LockOnIndicator(base.gameObject, RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/EngiMissileTrackingIndicator"))
				};
				indicatorInfo.indicator.targetTransform = hurtBox.transform;
				indicatorInfo.indicator.active = true;
			}
			indicatorInfo.refCount++;
			indicatorInfo.indicator.missileCount = indicatorInfo.refCount;
			this.targetIndicators[hurtBox] = indicatorInfo;
			base.activatorSkillSlot.DeductStock(1);
		}

		private void RemoveTargetAtAuthority(int i)
		{
			HurtBox key = this.targetsList[i];
			this.targetsList.RemoveAt(i);
			IndicatorInfo indicatorInfo;
			if (this.targetIndicators.TryGetValue(key, out indicatorInfo))
			{
				indicatorInfo.refCount--;
				indicatorInfo.indicator.missileCount = indicatorInfo.refCount;
				this.targetIndicators[key] = indicatorInfo;
				if (indicatorInfo.refCount == 0)
				{
					indicatorInfo.indicator.active = false;
					this.targetIndicators.Remove(key);
				}
			}
		}

		private void CleanTargetsList()
		{
			for (int i = this.targetsList.Count - 1; i >= 0; i--)
			{
				HurtBox hurtBox = this.targetsList[i];
				if (!hurtBox.healthComponent || !hurtBox.healthComponent.alive)
				{
					this.RemoveTargetAtAuthority(i);
					base.activatorSkillSlot.AddOneStock();
				}
			}
            for (int j = this.targetsList.Count - 1; j >= base.activatorSkillSlot.maxStock; j--)
            {
                this.RemoveTargetAtAuthority(j);
            }
        }

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.characterBody.SetAimTimer(3f);
			if (base.isAuthority)
			{
				this.AuthorityFixedUpdate();
			}
		}

		private void GetCurrentTargetInfo(out HurtBox currentTargetHurtBox, out HealthComponent currentTargetHealthComponent)
		{
			Ray aimRay = base.GetAimRay();
			this.search.filterByDistinctEntity = true;
			this.search.filterByLoS = true;
			this.search.minDistanceFilter = 0f;
			this.search.maxDistanceFilter = 100f;
			this.search.minAngleFilter = 0f;
			this.search.maxAngleFilter = Paint.maxAngle;
			this.search.viewer = base.characterBody;
			this.search.searchOrigin = aimRay.origin;
			this.search.searchDirection = aimRay.direction;
			this.search.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(base.GetTeam());
			this.search.RefreshCandidates();
			this.search.FilterOutGameObject(base.gameObject);
			foreach (HurtBox hurtBox in this.search.GetResults())
			{
				if (hurtBox.healthComponent && hurtBox.healthComponent.alive)
				{
					currentTargetHurtBox = hurtBox;
					currentTargetHealthComponent = hurtBox.healthComponent;
					return;
				}
			}
			currentTargetHurtBox = null;
			currentTargetHealthComponent = null;
		}

		private void AuthorityFixedUpdate()
		{
			this.CleanTargetsList();
			bool flag = false;
			HurtBox hurtBox;
			HealthComponent y;
			this.GetCurrentTargetInfo(out hurtBox, out y);
			if (hurtBox)
			{
				this.stackStopwatch += Time.fixedDeltaTime;
				if (base.inputBank.skill1.down && (this.previousHighlightTargetHealthComponent != y || this.stackStopwatch > Paint.stackInterval / this.attackSpeedStat || base.inputBank.skill1.justPressed))
				{
					this.stackStopwatch = 0f;
					this.AddTargetAuthority(hurtBox);
				}
			}
			if (base.inputBank.skill1.justReleased)
			{
				flag = true;
			}
			if (base.inputBank.skill2.justReleased)
			{
				this.outer.SetNextStateToMain();
				return;
			}
			if (base.inputBank.skill3.justReleased)
			{
				if (this.releasedKeyOnce)
				{
					flag = true;
				}
				this.releasedKeyOnce = true;
			}
			if (hurtBox != this.previousHighlightTargetHurtBox)
			{
				this.previousHighlightTargetHurtBox = hurtBox;
				this.previousHighlightTargetHealthComponent = y;
                this.stickyTargetIndicator.targetTransform = ((hurtBox && base.activatorSkillSlot.stock != 0) ? hurtBox.transform : null);
                this.stackStopwatch = 0f;
            }
			this.stickyTargetIndicator.active = this.stickyTargetIndicator.targetTransform;
			if (flag)
			{
				this.queuedFiringState = true;
				this.outer.SetNextState(new FireMissiles
				{
					targetsList = this.targetsList,
					activatorSkillSlot = base.activatorSkillSlot
				});
			}
		}

		public static GameObject crosshairOverridePrefab;
		public static GameObject stickyTargetIndicatorPrefab;
		public static float stackInterval;
		public static string enterSoundString;
		public static string exitSoundString;
		public static string loopSoundString;
		public static string lockOnSoundString;
		public static string stopLoopSoundString;
		public static float maxAngle;
		public static float maxDistance;
		private List<HurtBox> targetsList;
		private Dictionary<HurtBox, IndicatorInfo> targetIndicators;
		private Indicator stickyTargetIndicator;
		private SkillDef engiConfirmTargetDummySkillDef;
		private SkillDef engiCancelTargetingDummySkillDef;
		private bool releasedKeyOnce;
		private float stackStopwatch;
		private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
		private BullseyeSearch search;
		private bool queuedFiringState;
		private uint loopSoundID;
		private HealthComponent previousHighlightTargetHealthComponent;
		private HurtBox previousHighlightTargetHurtBox;
		private struct IndicatorInfo
		{
			public int refCount;
			public LockOnIndicator indicator;
		}

		private class LockOnIndicator : Indicator
		{
			public int missileCount;
			public override void UpdateVisualizer()
			{
				base.UpdateVisualizer();
				Transform transform = base.visualizerTransform.Find("DotOrigin");
				for (int i = transform.childCount - 1; i >= this.missileCount; i--)
				{
					EntityState.Destroy(transform.GetChild(i));
				}
				for (int j = transform.childCount; j < this.missileCount; j++)
				{
					UnityEngine.Object.Instantiate<GameObject>(base.visualizerPrefab.transform.Find("DotOrigin/DotTemplate").gameObject, transform);
				}
				if (transform.childCount > 0)
				{
					float num = 360f / (float)transform.childCount;
					float num2 = (float)(transform.childCount - 1) * 90f;
					for (int k = 0; k < transform.childCount; k++)
					{
						Transform child = transform.GetChild(k);
						child.gameObject.SetActive(true);
						child.localRotation = Quaternion.Euler(0f, 0f, num2 + (float)k * num);
					}
				}
			}

			public LockOnIndicator(GameObject owner, GameObject visualizerPrefab) : base(owner, visualizerPrefab)
			{
			}
		}
	}
}
