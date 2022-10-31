using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.HANDSurvivor.Components.Body;
using RoR2;
using UnityEngine;

namespace EntityStates.HAND_Overclocked.Secondary
{
    public class ChargeSlam : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_HOC_StartHammer", base.gameObject);
            this.minDuration = ChargeSlam.baseMinDuration / this.attackSpeedStat;
            this.modelAnimator = base.GetModelAnimator();
            if (this.modelAnimator)
            {
                base.PlayAnimation("Gesture, Override", HammerVisibilityController.HasHammerPrimary(base.skillLocator) ? "PrepHammerH" : "PrepHammer", "ChargeHammer.playbackRate", this.minDuration);
            }
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(3f);
            }
            charge = 0f;
            chargePercent = 0f;
            chargeDuration = ChargeSlam.baseChargeDuration / this.attackSpeedStat;

            hammerController = base.GetComponent<HammerVisibilityController>();
            if (hammerController)
            {
                hammerController.SetHammerEnabled(true);
            }

            //Attack is only agile while in OVC
            if (base.isAuthority && base.characterBody && !base.characterBody.HasBuff(Buffs.Overclock))
            {
                base.characterBody.isSprinting = false;
            }
        }

        public override void OnExit()
        {
            if (this.holdChargeVfxGameObject)
            {
                EntityState.Destroy(this.holdChargeVfxGameObject);
                this.holdChargeVfxGameObject = null;
            }
            if (!this.outer.destroying)
            {
                this.PlayAnimation("Gesture, Override", "Empty");
            }
            if (hammerController)
            {
                hammerController.SetHammerEnabled(false);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(3f);
            }

            if (base.fixedAge > this.minDuration && charge < chargeDuration)
            {
                if (!startedChargeAnim)
                {
                    startedChargeAnim = true;
                    base.PlayCrossfade("Gesture, Override", "ChargeHammer", "ChargeHammer.playbackRate", (this.chargeDuration - this.minDuration)/0.9f, 0.2f);
                }

                charge += Time.deltaTime * this.attackSpeedStat;
                if (charge > chargeDuration)
                {
                    Util.PlaySound("Play_HOC_StartPunch", base.gameObject);
                    charge = chargeDuration;
                    EffectManager.SpawnEffect(chargeEffectPrefab, new EffectData
                    {
                        origin = base.transform.position
                    }, false);
                }
                chargePercent = charge / chargeDuration;
            }

            if (base.fixedAge >= this.minDuration)
            {
                if (base.isAuthority && base.inputBank && !base.inputBank.skill2.down)
                {
                    SetNextState();
                    return;
                }
            }
        }

        public virtual void SetNextState()
        {
            this.outer.SetNextState(new FireSlam() { chargePercent = chargePercent });
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static float baseMinDuration = 0.5f;
        public static float baseChargeDuration = 1.5f;
        private float minDuration;
        private float chargeDuration;
        private float charge;
        public float chargePercent;
        private Animator modelAnimator;
        public static GameObject chargeEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLoader");
        private bool startedChargeAnim = false;

        private HammerVisibilityController hammerController;

        public static GameObject holdChargeVfxPrefab = EntityStates.Toolbot.ChargeSpear.holdChargeVfxPrefab;
        private GameObject holdChargeVfxGameObject = null;
    }
}
