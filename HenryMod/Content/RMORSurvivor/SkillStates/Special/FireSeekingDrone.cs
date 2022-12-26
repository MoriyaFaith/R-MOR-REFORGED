using RMORMod.Content.HANDSurvivor;
using RMORMod.Content.RMORSurvivor.Components.Body;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.RMOR.Special
{
    public class FireSeekingDrone : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            hasFired = false;
            Transform modelTransform = base.GetModelTransform();
            targetingController = base.GetComponent<RMORTargetingController>();
            Util.PlaySound("Play_HOC_Drone", base.gameObject);
            if (base.isAuthority && targetingController)
            {
                this.initialOrbTarget = targetingController.GetTrackingTarget();
                //handController.CmdHeal();
            }
            this.duration = baseDuration; /// this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration + 1f);
            }
            this.isCrit = base.RollCrit();
        }

        public override void OnExit()
        {
            if (!hasFired && base.isAuthority)
            {
                FireProjectile(this.initialOrbTarget, base.inputBank.aimOrigin);
            }
            base.OnExit();
        }

        private void FireProjectile(HurtBox target, Vector3 position)
        {
            hasFired = true;
            if (target)
            {

                MissileUtils.FireMissile(position, base.characterBody, default(ProcChainMask), target.gameObject, this.damageStat * FireSeekingDrone.damageCoefficient,
                    base.RollCrit(), FireSeekingDrone.projectilePrefab, DamageColorIndex.Default, Vector3.up, FireSeekingDrone.force, false);
            }
            else
            {
                MissileUtils.FireMissile(position, base.characterBody, default(ProcChainMask), default(GameObject), this.damageStat * FireSeekingDrone.damageCoefficient,
                    base.RollCrit(), FireSeekingDrone.projectilePrefab, DamageColorIndex.Default, Vector3.up, FireSeekingDrone.force, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!hasFired && base.isAuthority)
            {
                FireProjectile(this.initialOrbTarget, base.inputBank.aimOrigin);
            }
            if (base.fixedAge > this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private bool hasFired;


        public static float damageCoefficient = 8f;
        public static GameObject projectilePrefab;
        public static string muzzleString;
        public static GameObject muzzleflashEffectPrefab;
        public static float baseDuration = 0.25f;
        public static float force = 250f;

        private float duration;
        protected bool isCrit;
        private HurtBox initialOrbTarget = null;
        private RMORTargetingController targetingController;
    }
}
