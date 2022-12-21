using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.RMOR.Secondary
{
    public class FireCannon : GenericProjectileBaseState
    {
        public new static GameObject projectilePrefab;
        public static float baseExitDuration = 0.6f;
        public static float baseDurationBetweenShots = 0.2f;
        public static float DamageCoefficient = 4.2f;
        public new static float force = 2000f;
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
            base.projectilePrefab = projectilePrefab;
            //shotsRemaining = Mathf.ClosestPowerOfTwo(chargeLevel - 1);
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

        public override void FireProjectile()
        {
            shotsRemaining--;
            base.FireProjectile();
        }
    }
}
