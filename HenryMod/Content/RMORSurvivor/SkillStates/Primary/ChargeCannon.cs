using RoR2;
using UnityEngine;

namespace EntityStates.RMOR.Primary
{
    public class ChargeCannon : BaseState
    {
        public static float baseDuration = 1.5f;
        public static string partialChargeSoundString = "Play_engi_M1_chargeStock";
        public static string fullChargeSoundString = "Play_HOC_StartPunch";
        public static GameObject partialChargeEffect;
        public static GameObject fullChargeEffect;
        public static int maxChargeLevel = 3;

        private float duration;
        public int chargeLevel;

        public override void OnEnter()
        {
            base.OnEnter();
            chargeLevel = 0;
            this.duration = ChargeCannon.baseDuration / this.attackSpeedStat;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (chargeLevel < ChargeCannon.maxChargeLevel && base.fixedAge >= (chargeLevel + 1) * duration / (float)ChargeCannon.maxChargeLevel)
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
            }

            if (base.isAuthority)
            {
                if (!base.inputBank || !base.inputBank.skill1.down)
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
            }
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
