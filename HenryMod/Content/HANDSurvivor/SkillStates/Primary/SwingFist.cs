using HANDMod.Content.HANDSurvivor;
using HANDMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace EntityStates.HAND_Overclocked.Primary
{
    public class SwingFist : BaseMeleeAttack
    {
        public static GameObject swingEffect;
        public override void OnEnter()
        {
            //this.pushForce = 300f;
            //this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;

            //this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = null;
            this.hitEffectPrefab = null;


            this.damageType = DamageType.Generic;
            this.hitHopVelocity = 10f;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "Play_MULT_shift_hit";
            this.swingSoundString = "Play_HOC_Punch";
            this.hitboxName = "FistHitbox";
            this.damageCoefficient = 3.9f;
            this.procCoefficient = 1f;
            this.baseDuration = 1f;
            this.baseEarlyExitTime = 0f;
            this.attackStartTime = 0.5f;
            this.attackEndTime = 0.7f;

            Util.PlaySound("Play_HOC_StartPunch", base.gameObject);

            if (base.characterBody && base.characterBody.HasBuff(Buffs.Overclock) && this.swingIndex == 1)
            {
                this.damageType |= DamageType.Stun1s;
            }

            base.OnEnter();

            if (this.swingIndex != 0)
            {
                base.characterBody.OnSkillActivated(base.skillLocator.primary);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }


        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("Gesture, Override", "PunchR", "Slash.playbackRate", this.duration, 0.2f);
            switch (this.swingIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }


        protected override void SetNextState()
        {
            int index = this.swingIndex;
            switch (index)
            {
                case 0:
                    index = 1;
                    break;
                case 1:
                    index = 2;
                    break;
                case 2:
                    index = 1;
                    break;
            }
            //0 - PunchR
            //1 - PunchLR
            //2 - PunchRL

            this.outer.SetNextState(new SwingFist
            {
                swingIndex = index
            });
        }
    }
}
