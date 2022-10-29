using UnityEngine;
using RoR2;
using HANDMod.SkillStates.BaseStates;
using HANDMod.Content.HANDSurvivor.Components.Body;
using R2API;
using HANDMod.Content.HANDSurvivor;

namespace EntityStates.HANDMod.Secondary
{
    public class FireSlam : BaseMeleeAttack
    {
        public static GameObject swingEffect;

        public float chargePercent = 0f;

        public static float minDamageCoefficient = 5f;
        public static float maxDamageCoefficient = 15f;

        private HammerVisibilityController hammerController;

        private bool hitEnemy = false;
        public override void OnEnter()
        {
            //this.pushForce = 300f;
            //this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;

            //this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = null;
            this.hitEffectPrefab = null;


            this.damageType = DamageType.Stun1s;
            this.hitHopVelocity = 10f;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "Play_MULT_shift_hit";
            this.swingSoundString = "Play_HOC_Punch";
            this.hitboxName = "ChargeHammerHitbox";
            this.damageCoefficient = Mathf.Lerp(FireSlam.minDamageCoefficient, FireSlam.maxDamageCoefficient, chargePercent);
            this.procCoefficient = 1f;
            this.baseDuration = 1f;
            this.baseEarlyExitTime = 0.5f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.5f;
            this.pushForce = 0f;

            Util.PlaySound("Play_HOC_StartPunch", base.gameObject);

            hammerController = base.GetComponent<HammerVisibilityController>();
            if (hammerController)
            {
                hammerController.SetHammerEnabled(true);
            }

            base.OnEnter();
        }


        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Gesture, Override", "FireHammer", "ChargeHammer.playbackRate", this.duration);
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (!hitEnemy)
            {
                hitEnemy = true;
                OverclockController hc = base.gameObject.GetComponent<OverclockController>();
                if (hc)
                {
                    hc.MeleeHit();
                    if (base.characterBody && base.characterBody.HasBuff(Buffs.Overclock)) hc.ExtendOverclock(0.8f);
                }
            }
        }

        public override void OnExit()
        {
            if (hammerController)
            {
                hammerController.SetHammerEnabled(false);
            }
            base.OnExit();
        }

        protected override void SetNextState()
        {
            this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.hasFired && base.fixedAge >= (this.duration - this.earlyExitTime))
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.PrioritySkill;
        }
    }
}
