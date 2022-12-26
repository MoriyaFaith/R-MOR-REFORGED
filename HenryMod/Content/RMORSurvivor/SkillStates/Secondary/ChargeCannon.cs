using RoR2;
using RMORMod.Content.Shared.Components.Body;
using UnityEngine;

namespace EntityStates.RMOR.Secondary
{
    public class ChargeCannon : BaseState
    {
        public static float baseDuration = 1.5f;
        public static string partialChargeSoundString = "Play_engi_M1_chargeStock";
        public static string fullChargeSoundString = "Play_HOC_StartHammer";
        public static GameObject partialChargeEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLoader");
        public static GameObject fullChargeEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLoader");
        public static int maxChargeLevel = 3;

        private float duration;
        public int chargeLevel;

        public static float baseMinDuration = 0.5f;
        public static float baseChargeDuration = 1.5f;
        private float minDuration;
        private float charge;
        public float chargePercent;
        private Animator modelAnimator;
        public static GameObject chargeEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLoader");
        private bool startedChargeAnim = false;

        public static GameObject holdChargeVfxPrefab = EntityStates.Toolbot.ChargeSpear.holdChargeVfxPrefab;
        private GameObject holdChargeVfxGameObject = null;

        public override void OnEnter()
        {
            base.OnEnter();
            ModifyStats();
            this.minDuration = ChargeCannon.baseMinDuration / this.attackSpeedStat;
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
                //EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "HandL", false);
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "HandL", false);
                charge -= baseDuration;
            }

            if (base.isAuthority)
            {
                if (!base.inputBank || !base.inputBank.skill2.down)
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

        public virtual void ModifyStats() { }

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
