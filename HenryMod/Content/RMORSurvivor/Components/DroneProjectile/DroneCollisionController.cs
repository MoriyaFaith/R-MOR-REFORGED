using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RMORMod.Content.RMORSurvivor.Components.DroneProjectile
{
    public class DroneCollisionController : MonoBehaviour
    {
        private ProjectileTargetComponent ptc;
        private int passThroughWallsFrames = 0;

        private void Awake()
        {
            stick = base.GetComponent<ProjectileStickOnImpact>();
            ptc = base.GetComponent<ProjectileTargetComponent>();
            projectileNoTargetStopwatch = 0f;
            projectileController = base.GetComponent<ProjectileController>();
        }

        private void FixedUpdate()
        {
            if (stick && !stick.syncVictim)
            {
                if (passThroughWallsFrames > 0)
                {
                    passThroughWallsFrames--;
                    if (passThroughWallsFrames <= 0)
                    {
                        base.gameObject.layer = LayerIndex.projectile.intVal;
                    }
                }

                //Check if target lost
                if (NetworkServer.active && ptc)
                {
                    if (ptc.target != null)
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
                passThroughWallsFrames = 0;
                base.gameObject.layer = LayerIndex.projectile.intVal;
                Destroy(base.GetComponent<ProjectileSteerTowardTarget>());
                Destroy(base.GetComponent<ProjectileSphereTargetFinder>());
                Destroy(ptc);
                Destroy(this);
                return;
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerIndex.world.intVal)
            {
                base.gameObject.layer = LayerIndex.collideWithCharacterHullOnly.intVal;
                passThroughWallsFrames = 15;
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
