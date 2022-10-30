using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.DroneProjectile
{
    public class PreventGroundCollision : NetworkBehaviour
    {
        MissileController mc;
        public void Awake()
        {
            if (NetworkServer.active)
            {
                stick = base.GetComponent<ProjectileStickOnImpact>();
                previousPos = this.transform.position;
                previousPos2 = this.transform.position;
                stopwatch = 0f;
                intangibleStopwatch = 0f;
                mc = base.GetComponent<MissileController>();
                missileNoTargetStopwatch = 0f;
            }
        }

        [ClientRpc]
        private void RpcSetLayer(int layer)
        {
            this.gameObject.layer = layer;
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (stick && !stick.stuck)
                {
                    if (mc)
                    {
                        if (mc.targetComponent && mc.targetComponent.target != null)
                        {
                            missileNoTargetStopwatch = 0f;
                        }
                        missileNoTargetStopwatch += Time.fixedDeltaTime;
                        if (missileNoTargetStopwatch >= PreventGroundCollision.destroyIfNoTargetTime)
                        {
                            Destroy(base.gameObject);
                            return;
                        }
                    }

                    if (intangibleStopwatch > 0f)
                    {
                        intangibleStopwatch -= Time.fixedDeltaTime;
                        if (intangibleStopwatch <= 0f)
                        {
                            RpcSetLayer(LayerIndex.projectile.intVal);
                        }
                    }
                    else
                    {
                        stopwatch += Time.fixedDeltaTime;
                        if (stopwatch > 0.05f)
                        {
                            stopwatch -= 0.05f;

                            previousPos2 = previousPos;
                            previousPos = this.transform.position;
                        }

                        float distanceSqr = (previousPos - previousPos2).sqrMagnitude;

                        if (distanceSqr < 0.1f)
                        {
                            intangibleStopwatch = 0.1f;
                            RpcSetLayer(LayerIndex.entityPrecise.intVal);
                        }
                    }
                    previousPos = this.transform.position;
                }
                else
                {
                    if (mc) Destroy(mc);
                    Destroy(this);
                    return;
                }
            }
        }

        public static float destroyIfNoTargetTime = 5f;
        private float missileNoTargetStopwatch;
        private float intangibleStopwatch;
        private float stopwatch;
        private Vector3 previousPos;
        private Vector3 previousPos2;
        private ProjectileStickOnImpact stick;
    }
}
