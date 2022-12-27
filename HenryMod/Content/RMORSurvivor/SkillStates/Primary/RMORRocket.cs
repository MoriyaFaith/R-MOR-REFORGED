using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.RMOR.Primary
{
    public class RMORRocket : GenericProjectileBaseState
    {

        public static float BaseDuration = 1.3f;
        //delay here for example and to match animation
        //ordinarily I recommend not having a delay before projectiles. makes the move feel sluggish
        public static float BaseDelayDuration = 0.3f;
        public static float DamageCoefficient = 4.2f;
        private string animationLayer = "Gesture, Override";
        public static GameObject projectilePrefab;
        public bool strikeIndex;

        public override void OnEnter()
        {
            base.projectilePrefab = projectilePrefab;
            base.effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLoader");
            this.targetMuzzle = strikeIndex ? "HandR" : "HandL";    //Anim names are reversed. This is correct.

            base.attackSoundString = "Play_HOC_StartPunch";

            base.baseDuration = BaseDuration;
            //base.baseDelayBeforeFiringProjectile = BaseDelayDuration;

            base.damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            base.force = 80f;

            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            base.recoilAmplitude = 0.1f;
            base.bloom = 10;

            base.OnEnter();
            base.characterBody.SetAimTimer(3f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void FireProjectile()
        {
            base.FireProjectile();
            strikeIndex = !strikeIndex;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void PlayAnimation(float duration)
        {
            if (this.strikeIndex)
            {
                base.PlayCrossfade(animationLayer, "PunchR", "Punch.playbackRate", this.duration, 0.2f);
            }
            else
            {
                base.PlayCrossfade(animationLayer, "PunchL", "Punch.playbackRate", this.duration, 0.2f);
            }
        }
    }
}