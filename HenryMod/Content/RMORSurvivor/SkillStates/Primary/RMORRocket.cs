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
        public static float BaseDelayDuration = 0.283f;
        public static float DamageCoefficient = 4.2f;
        private string animationLayer = "FullBody, Override";
        public static GameObject projectilePrefab;
        public int strikeIndex;

        public override void OnEnter()
        {
            base.projectilePrefab = projectilePrefab;
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            this.targetMuzzle = (strikeIndex % 2 == 0) ? "MuzzleHandL" : "MuzzleHandR";    //Anim names are reversed. This is correct.

            base.attackSoundString = "Play_HOC_StartPunch";

            base.baseDuration = BaseDuration;
            base.baseDelayBeforeFiringProjectile = BaseDelayDuration;

            base.damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            base.force = 80f;

            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            base.recoilAmplitude = 0.1f;
            base.bloom = 10;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void PlayAnimation(float duration)
        {
            if (this.strikeIndex % 2 == 0)
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