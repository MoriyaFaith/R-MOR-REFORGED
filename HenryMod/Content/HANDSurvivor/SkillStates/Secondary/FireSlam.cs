using UnityEngine;
using RoR2;
using HANDMod.SkillStates.BaseStates;
using HANDMod.Content.HANDSurvivor.Components.Body;
using R2API;
using HANDMod.Content.HANDSurvivor;
using HANDMod.Content;

namespace EntityStates.HAND_Overclocked.Secondary
{
    public class FireSlam : BaseMeleeAttack
    {
        public static GameObject swingEffect;

        public float chargePercent = 0f;

        public static float minDamageCoefficient = 5f;
        public static float maxDamageCoefficient = 15f;

        public static float minDownForce = 2400f;
        public static float maxDownForce = 3200f;

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
            this.hitHopVelocity = 24f;
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
            this.bonusForce = Vector3.down * Mathf.Lerp(FireSlam.minDownForce, FireSlam.maxDownForce, chargePercent);

            Util.PlaySound("Play_HOC_StartPunch", base.gameObject);

            hammerController = base.GetComponent<HammerVisibilityController>();
            if (hammerController)
            {
                hammerController.SetHammerEnabled(true);
            }

            base.OnEnter();

            if (this.attack != null)
            {
                this.attack.AddModdedDamageType(DamageTypes.SquashOnKill);
                this.attack.AddModdedDamageType(DamageTypes.ResetVictimForce);
            }
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
                if (base.characterBody && base.characterBody.HasBuff(Buffs.Overclock))
                {
                    OverclockController hc = base.gameObject.GetComponent<OverclockController>();
                    if (hc)
                    {
                        hc.MeleeHit();
                        hc.ExtendOverclock(Mathf.Lerp(0.8f, 1.6f, chargePercent);
                    }
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
