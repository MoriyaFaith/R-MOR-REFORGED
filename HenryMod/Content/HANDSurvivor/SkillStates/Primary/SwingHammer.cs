using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.HANDSurvivor.Components.Body;
using HANDMod.SkillStates.BaseStates;
using RoR2;
using R2API;
using UnityEngine;
using HANDMod.Content;
using HANDMod.Content.Shared.Components.Body;
using UnityEngine.Networking;
using HANDMod;

namespace EntityStates.HAND_Overclocked.Primary
{
    public class SwingHammer : BaseMeleeAttack
    {
        public static NetworkSoundEventDef networkHitSound = null;
        public static GameObject swingEffect = null;
        public static GameObject swingEffectFocus = null;
        public static GameObject hitEffect = null;
        public static float force = 3000f;
        public static float forwardSpeed = 30f;

        public static float recoilAmplitude = 1.3f;
        public static float momentumStartPercent = 0.5f;
        public static float momentumFadePercent = 0.6825f;
        public static float momentumEndPercent = 0.8f;

        private float accumulatedReductionPercent = 1f;
        private float inputReductionPercent = 0f;
        private bool hitEnemy = false;
        private bool setNextState = false;
        private string animationLayer;

        private bool removedBuff = false;

        public override void OnEnter()
        {
            this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;
            //this.hitEffectPrefab = SwingHammer.hitEffect;  //Why does this play the DRONE sound?
            if (SwingHammer.networkHitSound != null) this.impactSound = networkHitSound.index;

            this.damageType = DamageType.Generic;
            this.hitHopVelocity = 11f;
            this.scaleHitHopWithAttackSpeed = true;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "";
            this.swingSoundString = "Play_loader_m1_swing";//"Play_HOC_Punch";
            this.hitboxName = "HammerHitbox";
            this.damageCoefficient = 6f;
            this.procCoefficient = 1f;
            this.baseDuration = 1.625f;
            this.baseEarlyExitTime = 0.325f;
            this.attackStartTime = 0.56875f;
            this.attackEndTime = 0.6825f;
            this.pushForce = 0f;

            Vector3 aimFlat = base.GetAimRay().direction;
            aimFlat.y = 0;
            aimFlat.Normalize();
            this.bonusForce = SwingHammer.force * aimFlat;
            this.muzzleString = this.swingIndex == 1 ? "SwingCenterL": "SwingCenterR";

            this.animationLayer = "FullBody, Override";
            Util.PlaySound("Play_HOC_StartPunch", base.gameObject);

            OverclockController ovc = base.GetComponent<OverclockController>();
            bool hasOVC = ovc && ovc.BuffActive();

            Animator an = base.GetModelAnimator();
            if (an) an.SetFloat("hammerIdle", 1f);

            this.swingEffectPrefab = SwingHammer.swingEffect;
            if (base.characterBody)
            {
                if (SwingHammer.swingEffectFocus && base.characterBody.HasBuff(HANDMod.Content.Shared.Buffs.NemesisFocus))
                {
                    this.swingEffectPrefab = SwingHammer.swingEffectFocus;
                }
                if (!HandPlugin.ArenaModeActive && hasOVC && this.swingIndex == 1)
                {
                    this.damageType |= DamageType.Stun1s;
                }
            }

            base.OnEnter();

            if (base.characterBody)
            {
                if (this.swingIndex != 0)
                {
                    base.characterBody.OnSkillActivated(base.skillLocator.primary);
                }

                base.characterBody.SetAimTimer(3f);

                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
                }
            }

            if (this.attack != null)
            {
                this.attack.AddModdedDamageType(DamageTypes.HANDPrimaryHammer);
                this.attack.AddModdedDamageType(DamageTypes.ResetVictimForce);

                if (base.characterBody && base.characterBody.HasBuff(HANDMod.Content.Shared.Buffs.NemesisFocus))
                {
                    this.attack.damageColorIndex = DamageColorIndex.Sniper;
                }
            }

            HammerVisibilityController hvc = base.GetComponent<HammerVisibilityController>();
            if (hvc)
            {
                hvc.SetHammerEnabled(true);
            }

            //Attack is only agile while in OVC
            if (base.isAuthority && !hasOVC)
            {
                base.characterBody.isSprinting = false;
            }
        }

        public override void FixedUpdate()
        {
            Vector3 aimFlat = base.GetAimRay().direction;
            aimFlat.y = 0;
            aimFlat.Normalize();
            if (this.attack != null)
            {
                this.attack.forceVector = SwingHammer.force * aimFlat;
            }

            if (base.characterBody)
            {
                this.damageStat = base.characterBody.damage;

                if (base.characterBody.HasBuff(HANDMod.Content.Shared.Buffs.NemesisFocus))
                {
                    this.swingEffectPrefab = SwingHammer.swingEffectFocus;
                    this.attack.damageColorIndex = DamageColorIndex.Sniper;
                }
                else
                {
                    this.swingEffectPrefab = SwingHammer.swingEffect;
                    this.attack.damageColorIndex = DamageColorIndex.Default;
                }
            }

            base.FixedUpdate();

            if (NetworkServer.active && !removedBuff)
            {
                if (base.fixedAge > base.duration * attackEndTime)
                {
                    RemoveBuff();
                }
            }

            if (base.isAuthority)
            { 
                if (!this.hasFired)
                {
                    this.startedSkillStationary = base.characterMotor && base.characterMotor.moveDirection == Vector3.zero;
                    if (base.inputBank && !this.startedSkillStationary)
                    {
                        Ray aimRay = base.GetAimRay();
                        Vector2 moveDirectionFlat = new Vector2(base.inputBank.moveVector.x, base.inputBank.moveVector.z);
                        Vector2 forwardDirectionFlat = new Vector2(aimRay.direction.x, aimRay.direction.z);

                        float angle = Vector2.Angle(moveDirectionFlat, forwardDirectionFlat);

                        if (angle < 50f)
                        {
                            this.inputReductionPercent = 0f;
                        }
                        else
                        {
                            this.inputReductionPercent = 1f;
                            this.startedSkillStationary = true;
                        }
                    }
                }

                if (!this.inHitPause && base.characterDirection && base.characterMotor && !this.startedSkillStationary && this.stopwatch >= this.duration * SwingHammer.momentumStartPercent)
                {
                    float fadeTime = this.duration * SwingHammer.momentumFadePercent;
                    float momentumEndTime = this.duration * SwingHammer.momentumEndPercent;
                    if (this.stopwatch <= momentumEndTime)
                    {
                        float evaluatedForwardSpeed = SwingHammer.forwardSpeed * Time.fixedDeltaTime * (1f - inputReductionPercent) * accumulatedReductionPercent;
                        if (this.stopwatch > fadeTime)
                        {
                            evaluatedForwardSpeed = Mathf.Lerp(evaluatedForwardSpeed, 0f, (this.stopwatch - fadeTime) / (momentumEndTime - fadeTime));
                        }

                        if (evaluatedForwardSpeed > 0f)
                        {
                            Vector3 evaluatedForwardVector = base.characterDirection.forward * evaluatedForwardSpeed;
                            base.characterMotor.AddDisplacement(new Vector3(evaluatedForwardVector.x, 0f, evaluatedForwardVector.z));
                        }
                        else
                        {
                            //Use this to cancel lunge
                            startedSkillStationary = true;
                        }
                    }
                }
            }
        }

        public override void OnFiredAttack()
        {
            if (base.isAuthority)
            {
                base.AddRecoil(-0.1f * SwingHammer.recoilAmplitude, 0.1f * SwingHammer.recoilAmplitude, -1f * SwingHammer.recoilAmplitude, 1f * SwingHammer.recoilAmplitude);
            }

            if (this.swingEffectPrefab == SwingHammer.swingEffectFocus)
            {
                Util.PlaySound("Play_HOC_Focus", base.gameObject);
            }
        }

        protected override void PlayAttackAnimation()
        {
            //Uncomment when updated punch anims are in
            switch (this.swingIndex)
            {
                case 0:
                    base.PlayCrossfade(animationLayer, "HammerSwingR", "SwingHammer.playbackRate", this.duration * 1.4f, 0.2f);
                    break;
                case 1:
                    base.PlayCrossfade(animationLayer, "HammerSwingRL", "SwingHammer.playbackRate", this.duration * 0.8f, 0.2f);
                    break;
                case 2:
                    base.PlayCrossfade(animationLayer, "HammerSwingLR", "SwingHammer.playbackRate", this.duration * 0.8f, 0.2f);
                    break;
            }
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();

            this.accumulatedReductionPercent *= 0.5f;

            if (!hitEnemy)
            {
                hitEnemy = true;
                if (base.characterBody)
                {
                    OverclockController hc = base.gameObject.GetComponent<OverclockController>();
                    if (hc)
                    {
                        hc.ExtendOverclock(1f);
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
            this.outer.SetNextState(new SwingHammer
            {
                swingIndex = index
            });
        }

        public override void OnExit()
        {
            if (!this.outer.destroying && !setNextState)
            {
                float exitDuration = swingIndex == 0 ? 1.3f / this.attackSpeedStat : 0.3f;
                this.PlayCrossfade(animationLayer, "BufferEmpty", "SwingHammer.playbackRate", exitDuration, exitDuration);
            }

            RemoveBuff();

            base.OnExit();
        }

        private void RemoveBuff()
        {

            if (!removedBuff && NetworkServer.active && base.characterBody)
            {
                removedBuff = true;
                if (base.characterBody.HasBuff(RoR2Content.Buffs.Slow50)) base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
