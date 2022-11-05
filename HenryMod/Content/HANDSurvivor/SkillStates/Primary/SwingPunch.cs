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
        public static AnimationCurve punchVelocityCurve = new AnimationCurve(new Keyframe[]
        {
            new Keyframe(0f, 0f, 0.25312966108322146f, Mathf.Infinity, 0f, 0.3333333432674408f),    //Move starting time to when HAN-D's punch starts
            new Keyframe(0.24929532408714295f, 0.20000000298023225f, -1.3447399139404297f, -1.3447399139404297f, 0.3333333432674408f, 0.09076657891273499f),
            new Keyframe(0.6705322265625f, 0f, -0.1023506224155426f, -0.1023506224155426f, 0.7332440614700317f, 0f),
        });

        //Loader's VelocityCurve
        /*{"preWrapMode":8,"postWrapMode":8
         * "keys":[{"time":0.0,"value":0.0,"inTangent":0.25312966108322146,"outTangent":Infinity,"inWeight":0.0,"outWeight":0.3333333432674408,"weightedMode":0,"tangentMode":97},
         * {"time":0.24929532408714295,"value":0.20000000298023225,"inTangent":-1.3447399139404297,"outTangent":-1.3447399139404297,"inWeight":0.3333333432674408,"outWeight":0.09076657891273499,"weightedMode":0,"tangentMode":0},
         * { "time":0.6705322265625,"value":0.0,"inTangent":-0.1023506224155426,"outTangent":-0.1023506224155426,"inWeight":0.7332440614700317,"outWeight":0.0,"weightedMode":0,"tangentMode":0}]}*/

        private bool setNextState = false;
        private string animationLayer;
        public static float force = 1600f;

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
            this.bonusForce = SwingPunch.force * base.transform.forward;
            this.forceForwardVelocity = true;
            this.forwardVelocityCurve = punchVelocityCurve;

            this.animationLayer = "FullBody, Override";

            Util.PlaySound("Play_HOC_StartPunch", base.gameObject);

            OverclockController ovc = base.GetComponent<OverclockController>();
            bool hasOVC = ovc && ovc.BuffActive();

            Animator an = base.GetModelAnimator();
            if (an) an.SetFloat("hammerIdle", 0f);

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

        public override void FixedUpdate()
        {
            this.bonusForce = SwingPunch.force * base.transform.forward;
            base.FixedUpdate();
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
            switch (this.swingIndex)
            {
                case 0:
                    base.PlayCrossfade(animationLayer, "PunchL", "Punch.playbackRate", this.duration, 0.2f);
                    break;
                case 1:
                    base.PlayCrossfade(animationLayer, "PunchLR", "Punch.playbackRate", this.duration * 0.6f, 0.2f);
                    break;
                case 2:
                    base.PlayCrossfade(animationLayer, "PunchRL", "Punch.playbackRate", this.duration * 0.6f, 0.2f);
                    break;
            }

            /*if (this.swingIndex == 1)
            {
                base.PlayCrossfade(animationLayer, "PunchR", "Punch.playbackRate", this.duration, 0.2f);
            }
            else
            {
                base.PlayCrossfade(animationLayer, "PunchL", "Punch.playbackRate", this.duration, 0.2f);
            }*/
        }

        public override void OnExit()
        {
            if (!this.outer.destroying && !setNextState)
            {
                this.PlayCrossfade(animationLayer, "BufferEmpty", "Punch.playbackRate", 0.2f, 0.2f);
            }
            base.OnExit();
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
                        hc.ExtendOverclock(0.8f);
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
            setNextState = true;
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
