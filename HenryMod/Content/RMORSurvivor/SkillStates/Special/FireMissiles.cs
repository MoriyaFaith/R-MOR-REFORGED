using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using EntityStates;
using EntityStates.Engi.EngiMissilePainter;

namespace EntityStates.RMOR.Special
{
	public class FireMissiles : BaseLockOnMissileState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.durationPerMissile = Fire.baseDurationPerMissile / this.attackSpeedStat;
			this.PlayAnimation("Gesture, Additive", "IdleHarpoons");
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = false;
			if (base.isAuthority)
			{
				this.stopwatch += Time.fixedDeltaTime;
				if (this.stopwatch >= this.durationPerMissile)
				{
					this.stopwatch -= this.durationPerMissile;
					while (this.fireIndex < this.targetsList.Count)
					{
						List<HurtBox> list = this.targetsList;
						int num = this.fireIndex;
						this.fireIndex = num + 1;
						HurtBox hurtBox = list[num];
						if (hurtBox.healthComponent && hurtBox.healthComponent.alive)
						{
							string text = (this.fireIndex % 2 == 0) ? "MuzzleLeft" : "MuzzleRight";
							Vector3 position = base.inputBank.aimOrigin;
							Transform transform = base.FindModelChild(text);
							if (transform != null)
							{
								position = transform.position;
							}
							EffectManager.SimpleMuzzleFlash(Fire.muzzleflashEffectPrefab, base.gameObject, text, true);
							this.FireMissile(hurtBox, position);
							flag = true;
							break;
						}
						base.activatorSkillSlot.AddOneStock();
					}
					if (this.fireIndex >= this.targetsList.Count)
					{
						this.outer.SetNextState(new Finish());
					}
				}
			}
			if (flag)
			{
				this.PlayAnimation((this.fireIndex % 2 == 0) ? "Gesture Left Cannon, Additive" : "Gesture Right Cannon, Additive", "FireHarpoon");
			}
		}

		private void FireMissile(HurtBox target, Vector3 position)
		{
			MissileUtils.FireMissile(base.inputBank.aimOrigin, base.characterBody, default(ProcChainMask), target.gameObject, this.damageStat * damageCoefficient, base.RollCrit(), projectilePrefab, DamageColorIndex.Default, Vector3.up, 0f, false);
		}

		public override void OnExit()
		{
			base.OnExit();
			base.PlayCrossfade("Gesture, Additive", "ExitHarpoons", 0.1f);
		}

		public static float baseDurationPerMissile;
		public static float damageCoefficient = 6.0f;
		public static GameObject projectilePrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");
		public static GameObject muzzleflashEffectPrefab;
		public List<HurtBox> targetsList;
		private int fireIndex;
		private float durationPerMissile;
		private float stopwatch;
	}
}
