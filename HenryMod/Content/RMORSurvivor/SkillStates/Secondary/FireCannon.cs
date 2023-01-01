using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;

namespace EntityStates.RMOR.Secondary
{
    public class FireCannon : BaseState
    {
        public static string attackSoundString = "Play_RMOR_Rocket";
        public static GameObject level1Prefab;
        public static GameObject level2Prefab;
        public static GameObject level3Prefab;
        public static GameObject level4Prefab;
        public static float baseExitDuration = 0.6f;
        public static float baseDurationBetweenShots = 0.5f;
        public static float damageCoefficient = 6.0f;
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
            //shotsRemaining = Mathf.ClosestPowerOfTwo(chargeLevel - 1);
            shotsRemaining = 1; //yes, I know this is scuffed, but like come on
            crit = base.RollCrit();
            fireStopwatch = 0f;
            durationBetweenShots = FireCannon.baseDurationBetweenShots / this.attackSpeedStat;
            totalDuration = FireCannon.baseExitDuration / this.attackSpeedStat + durationBetweenShots * shotsRemaining;
            base.characterBody.SetAimTimer(3f);
            this.PlayAnimation("Gesture, Override", "FireCannon");
            FireProjectile();
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

        public override void OnExit()
        {
            if (!this.outer.destroying)
            {
                this.PlayAnimation("Gesture, Override", "Empty");
            }
            base.OnExit();
        }

        public GameObject GetProjectilePrefab()
        {
            switch (chargeLevel)
            {
                default:
                case 1:
                    return level1Prefab;
                case 2:
                    return level2Prefab;
                case 3:
                    return level3Prefab;
                case 4:
                    return level4Prefab;
                    
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
                ProjectileManager.instance.FireProjectile(GetProjectilePrefab(), aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient * chargeLevel, force * chargeLevel, crit);
            }
        }
    }
}
