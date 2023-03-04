using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using RMORMod.Modules;
using System;
using RMORMod.Content.Shared.Components.Body;

namespace EntityStates.RMOR.Primary
{
    public class PrimaryRocket : GenericProjectileBaseState
    {

        public static float BaseDuration = 1.3f;
        //delay here for example and to match animation
        //ordinarily I recommend not having a delay before projectiles. makes the move feel sluggish
        public static float BaseDelayDuration = 0.3f;
        public static float DamageCoefficient = 3.9f;
        //public static float OVCDamageCoefficient = 6f;

        private string animationLayer = "Gesture, Override";
        public static GameObject projectilePrefab;
        public static GameObject overclockPrefab;
        public static GameObject effectPrefab;
        public bool strikeIndex;

        public override void OnEnter()
        {
            OverclockController ovc = base.GetComponent<OverclockController>();
            bool hasOVC = ovc && ovc.BuffActive();
            base.projectilePrefab = hasOVC ? overclockPrefab : projectilePrefab; //shoots faster projectile in overclock
            base.effectPrefab = effectPrefab;
            this.targetMuzzle = strikeIndex ? "HandR" : "HandL";    //Anim names are reversed. This is correct.

            base.attackSoundString = "Play_RMOR_Primary";

            base.baseDuration = BaseDuration;
            //base.baseDelayBeforeFiringProjectile = BaseDelayDuration;

            base.damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab

            base.force = 80f;

            Ray aimRay = base.GetAimRay();

            if (base.characterMotor)
                base.characterMotor.ApplyForce(-force * 25f * aimRay.direction, false, false);
            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            base.recoilAmplitude = 0.1f;
            base.bloom = 10;
            base.characterBody.SetAimTimer(3f);

            base.OnEnter();
        }

        public override void OnExit()
        {
            if (!this.outer.destroying)
            {
                this.PlayAnimation(animationLayer, "RMOR|RocketShootHold");
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void FireProjectile()
        {
            base.FireProjectile();
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