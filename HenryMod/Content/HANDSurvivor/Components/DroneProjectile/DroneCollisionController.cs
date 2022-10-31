using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.DroneProjectile
{
    public class DroneCollisionController : MonoBehaviour
    {
        ProjectileSteerTowardTarget pst;
        private void Awake()
        {
            stick = base.GetComponent<ProjectileStickOnImpact>();
            pst = base.GetComponent<ProjectileSteerTowardTarget>();
            projectileNoTargetStopwatch = 0f;
            projectileController = base.GetComponent<ProjectileController>();
        }

        private void FixedUpdate()
        {
            if (stick && !stick.syncVictim)
            {
                //Check if target lost
                if (NetworkServer.active && pst)
                {
                    if (pst.targetComponent && pst.targetComponent.target != null)
                    {
                        projectileNoTargetStopwatch = 0f;
                    }
                    projectileNoTargetStopwatch += Time.fixedDeltaTime;
                    if (projectileNoTargetStopwatch >= DroneCollisionController.destroyIfNoTargetTime)
                    {
                        Destroy(base.gameObject);
                        return;
                    }
                }
            }
            else
            {
                if (NetworkServer.active)
                {
                    //Reset lifetime.
                    ProjectileSimple ps = base.GetComponent<ProjectileSimple>();
                    ps.SetLifetime(30f);
                }
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
        private float projectileNoTargetStopwatch;
        private ProjectileStickOnImpact stick;
        private ProjectileController projectileController;
    }
}
