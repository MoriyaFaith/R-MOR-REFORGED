using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.RMOR.Primary
{
    public class FireCannon : BaseState
    {
        public static string attackSoundString;
        public static GameObject projectilePrefab;
        public static float baseExitDuration = 0.6f;
        public static float baseDurationBetweenShots = 0.2f;
        public static float damageCoefficient = 5f;
        public static float force = 2000f;
        public static GameObject muzzleflashEffectPrefab;

        public int chargeLevel;
        private bool crit;

        private float totalDuration;
        private float durationBetweenShots;
        private float fireStopwatch;
        private int shotsRemaining;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            shotsRemaining = chargeLevel;
            crit = base.RollCrit();
            fireStopwatch = 0f;
            durationBetweenShots = FireCannon.baseDurationBetweenShots / this.attackSpeedStat;
            totalDuration = FireCannon.baseExitDuration / this.attackSpeedStat + durationBetweenShots * shotsRemaining;

            if (shotsRemaining > 0)
            {
                FireProjectile();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (shotsRemaining > 0)
            {
                fireStopwatch += Time.fixedDeltaTime;
                if (fireStopwatch >= durationBetweenShots)
                {
                    FireProjectile();
                    fireStopwatch -= durationBetweenShots;
                }
            }
            else
            {
                if (base.isAuthority && base.fixedAge > totalDuration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        private void FireProjectile()
        {
            shotsRemaining--;
            Util.PlaySound(FireCannon.attackSoundString, base.gameObject);
            EffectManager.SimpleMuzzleFlash(FireCannon.muzzleflashEffectPrefab, base.gameObject, "HandL", false);
            EffectManager.SimpleMuzzleFlash(FireCannon.muzzleflashEffectPrefab, base.gameObject, "HandR", false);
            Ray aimRay = base.GetAimRay();
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(FireCannon.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireCannon.damageCoefficient, FireCannon.force, crit);
            }
        }
    }
}
