using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.HANDSurvivor.Components.Body;
using HANDMod.SkillStates.BaseStates;
using RoR2;
using R2API;
using UnityEngine;
using HANDMod.Content;

namespace EntityStates.HAND_Overclocked.Primary
{
    public class SwingPunch : BaseMeleeAttack
    {
        public static NetworkSoundEventDef networkHitSound = null;
        public static GameObject swingEffect = null;
        public static GameObject hitEffect = null;

        private bool hitEnemy = false;
        public override void OnEnter()
        {
            this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;

            this.muzzleString = swingIndex == 1 ? "HandL" : "HandR";    //Anim names are reversed. This is correct.
            this.swingEffectPrefab = SwingPunch.swingEffect;
            this.hitEffectPrefab = SwingPunch.hitEffect;
            if(SwingPunch.networkHitSound) this.impactSound = networkHitSound.index;

            this.damageType = DamageType.Generic;
            this.hitHopVelocity = 8f;
            this.scaleHitHopWithAttackSpeed = true;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "";
            this.swingSoundString = "Play_HOC_Punch";
            this.hitboxName = "FistHitbox";
            this.damageCoefficient = 3.9f;
            this.procCoefficient = 1f;
            this.baseDuration = 1.25f;
            this.baseEarlyExitTime = 0.25f;
            this.attackStartTime = 0.4f;
            this.attackEndTime = 0.5f;
            this.pushForce = 0f;
            this.bonusForce = 1600f * base.GetAimRay().direction;

            Util.PlaySound("Play_HOC_StartPunch", base.gameObject);

            OverclockController ovc = base.GetComponent<OverclockController>();
            bool hasOVC = ovc && ovc.BuffActive();

            if (base.characterBody && hasOVC && this.swingIndex == 1)
            {
                this.damageType |= DamageType.Stun1s;
            }

            base.OnEnter();

            if (base.characterBody)
            {
                if (this.swingIndex != 0)
                {
                    base.characterBody.OnSkillActivated(base.skillLocator.primary);
                }

                //Attack is only agile while in OVC
                if (base.isAuthority && !hasOVC)
                {
                    base.characterBody.isSprinting = false;
                }

                base.characterBody.SetAimTimer(3f);
            }

            if (this.attack != null)
            {
                this.attack.AddModdedDamageType(DamageTypes.HANDPrimaryPunch);
                this.attack.AddModdedDamageType(DamageTypes.ResetVictimForce);

                if (base.characterBody && base.characterBody.HasBuff(Buffs.NemesisFocus))
                {
                    this.attack.damageColorIndex = DamageColorIndex.Sniper;
                }
            }
        }

        public override void OnFiredAttack()
        {
            if (base.isAuthority)
            {
                ShakeEmitter se = ShakeEmitter.CreateSimpleShakeEmitter(base.transform.position, new Wave() { amplitude = 3f, cycleOffset = 0f, frequency = 4f }, 0.25f, 20f, true);
                se.transform.parent = base.transform;
            }
        }

        protected override void PlayAttackAnimation()
        {
            //Uncomment when updated punch anims are in
            /*switch (this.swingIndex)
            {
                case 0:
                    base.PlayCrossfade("Gesture, Override", "PunchL", "Punch.playbackRate", this.duration, 0.2f);
                    break;
                case 1:
                    //base.PlayCrossfade("Gesture, Override", "PunchR", "Punch.playbackRate", this.duration, 0.2f);
                    base.PlayCrossfade("Gesture, Override", "PunchLR", "Punch.playbackRate", this.duration, 0.2f);
                    break;
                case 2:
                    //base.PlayCrossfade("Gesture, Override", "PunchL", "Punch.playbackRate", this.duration, 0.2f);
                    base.PlayCrossfade("Gesture, Override", "PunchRL", "Punch.playbackRate", this.duration, 0.2f);
                    break;
            }*/

            if (this.swingIndex == 1)
            {
                base.PlayCrossfade("Gesture, Override", "PunchR", "Punch.playbackRate", this.duration, 0.2f);
            }
            else
            {
                base.PlayCrossfade("Gesture, Override", "PunchL", "Punch.playbackRate", this.duration, 0.2f);
            }
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (!hitEnemy)
            {
                hitEnemy = true;
                if (base.characterBody)
                {
                    OverclockController hc = base.gameObject.GetComponent<OverclockController>();
                    if (hc)
                    {
                        hc.ExtendOverclock(this.baseDuration * 0.64f);
                    }

                    DroneStockController dsc = base.GetComponent<DroneStockController>();
                    if (dsc)
                    {
                        dsc.MeleeHit();
                    }
                }
            }
        }

        protected override void SetNextState()
        {
            int index = this.swingIndex;
            switch (index)
            {
                case 1:
                    index = 2;
                    break;
                case 0:
                case 2:
                    index = 1;
                    break;
            }
            //0 - PunchR
            //1 - PunchLR
            //2 - PunchRL

            this.outer.SetNextState(new SwingPunch
            {
                swingIndex = index
            });
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
