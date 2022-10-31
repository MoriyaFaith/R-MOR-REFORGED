using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.DroneProjectile
{
    public class DroneCollisionController : MonoBehaviour
    {
        MissileController mc;
        private void Awake()
        {
            stick = base.GetComponent<ProjectileStickOnImpact>();
            mc = base.GetComponent<MissileController>();
            missileNoTargetStopwatch = 0f;
            projectileController = base.GetComponent<ProjectileController>();
        }

        private void FixedUpdate()
        {
            if (stick && !stick.syncVictim)
            {
                //Check if target lost
                if (NetworkServer.active && mc)
                {
                    if (mc.targetComponent && mc.targetComponent.target != null)
                    {
                        missileNoTargetStopwatch = 0f;
                    }
                    missileNoTargetStopwatch += Time.fixedDeltaTime;
                    if (missileNoTargetStopwatch >= DroneCollisionController.destroyIfNoTargetTime)
                    {
                        Destroy(base.gameObject);
                        return;
                    }
                }
            }
            else
            {
                if (NetworkServer.active && mc) Destroy(mc);
                base.gameObject.layer = LayerIndex.projectile.intVal;
                Destroy(this);
                return;
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerIndex.world.intVal)
            {
                base.gameObject.layer = LayerIndex.collideWithCharacterHullOnly.intVal;
            }
            else
            {
                base.gameObject.layer = LayerIndex.projectile.intVal;
            }
        }

        public static float destroyIfNoTargetTime = 5f;
        private float missileNoTargetStopwatch;
        private ProjectileStickOnImpact stick;
        private ProjectileController projectileController;
    }
}
