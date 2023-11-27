using RoR2;
using RMORMod.Content.Shared.Components.Body;
using UnityEngine;

namespace EntityStates.RMOR.Special
{
    public class ChargeCannon : BaseState
    {
        public static float baseDuration = 1.5f;
        public static string partialChargeSoundString = "Play_RMOR_SemiCharge";
        public static string fullChargeSoundString = "Play_RMOR_FullCharge";
        public static GameObject partialChargeEffect;
        public static GameObject fullChargeEffect;
        public static int maxChargeLevel = 3;

        private float duration;
        public int chargeLevel;

        public static float baseChargeDuration = 1.8f;
        private float minDuration;
        private float charge;
        public float chargePercent;
        private Animator modelAnimator;

        public static GameObject holdChargeVfxPrefab = EntityStates.Toolbot.ChargeSpear.holdChargeVfxPrefab;
        private GameObject holdChargeVfxGameObject = null;

        public override void OnEnter()
        {
            base.OnEnter();
            ModifyStats();
            this.minDuration = ChargeCannon.baseChargeDuration / this.attackSpeedStat;
            this.modelAnimator = base.GetModelAnimator();
            if (this.modelAnimator)
            {
                base.PlayAnimation("Gesture, Override", "PrepCannon", "ChargeHammer.playbackRate", this.minDuration);
            }
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(3f);
            }
            charge = 0f;
            chargeLevel = 1;

            OverclockController ovc = base.GetComponent<OverclockController>();
            bool hasOVC = ovc && ovc.BuffActive();

            //Attack is only agile while in OVC
            if (base.isAuthority && base.characterBody && !hasOVC)
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
                this.PlayAnimation("Gesture, Override", "BufferEmpty");
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.SetAimTimer(3f);

            charge += Time.deltaTime * this.attackSpeedStat;

            if (chargeLevel < ChargeCannon.maxChargeLevel && charge > baseDuration)
            {
                chargeLevel++;
                string soundString = ChargeCannon.partialChargeSoundString;
                GameObject effectPrefab = ChargeCannon.partialChargeEffect;
                if (chargeLevel >= ChargeCannon.maxChargeLevel)
                {
                    soundString = ChargeCannon.fullChargeSoundString;
                    effectPrefab = ChargeCannon.fullChargeEffect;
                }
                Util.PlaySound(soundString, base.gameObject);
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "HandL", false);
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "HandR", false);
                charge -= baseDuration;
            }
            if (chargeLevel > maxChargeLevel && charge % 3 == 0)
            {
                GameObject effectPrefab = ChargeCannon.fullChargeEffect;
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "HandL", false);
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "HandR", false);
            }

            if (base.isAuthority)
            {
                if (!base.inputBank || !base.inputBank.skill4.down)
                {
                    if (chargeLevel > 0)
                    {
                        SetNextState();
                        return;
                    }
                    else
                    {
                        this.outer.SetNextStateToMain();
                    }
                }
            }        }

        public virtual void ModifyStats() {
            maxChargeLevel = 3;
            baseDuration = 1.5f;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public virtual void SetNextState()
        {
            this.outer.SetNextState(new FireCannon() { chargeLevel = this.chargeLevel });
        }
    }
}
