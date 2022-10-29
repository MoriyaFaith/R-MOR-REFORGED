using RoR2;
using UnityEngine;
using R2API;

namespace HANDMod.Content
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType ResetVictimForce;
        public static DamageAPI.ModdedDamageType HANDPrimaryPunch;

        private static bool initialized = false;

        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            DamageTypes.ResetVictimForce = DamageAPI.ReserveDamageType();
            DamageTypes.HANDPrimaryPunch = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            CharacterBody cb = self.body;

            //This will only work on things that are run on the server.
            if (damageInfo.HasModdedDamageType(DamageTypes.ResetVictimForce))
            {
                if (cb.rigidbody)
                {
                    cb.rigidbody.velocity = new Vector3(0f, cb.rigidbody.velocity.y, 0f);
                    cb.rigidbody.angularVelocity = new Vector3(0f, cb.rigidbody.angularVelocity.y, 0f);
                }
                if (cb.characterMotor != null)
                {
                    cb.characterMotor.velocity.x = 0f;
                    cb.characterMotor.velocity.z = 0f;
                    cb.characterMotor.rootMotion.x = 0f;
                    cb.characterMotor.rootMotion.z = 0f;
                }
            }

            if (damageInfo.HasModdedDamageType(DamageTypes.HANDPrimaryPunch))
            {
                if (cb.isFlying)
                {
                    damageInfo.force.x *= 0.5f;
                    damageInfo.force.z *= 0.5f;
                }
                else if (cb.characterMotor != null)
                {
                    if (!cb.characterMotor.isGrounded)    //Multiply launched enemy force
                    {
                        damageInfo.force.x *= 1.8f;
                        damageInfo.force.z *= 1.8f;
                    }
                    else
                    {
                        if (cb.isChampion) //deal less knockback against bosses if they're on the ground
                        {
                            damageInfo.force.x *= 0.5f;
                            damageInfo.force.z *= 0.5f;
                        }
                    }
                }

                //Scale force to match mass
                Rigidbody rb = cb.rigidbody;
                if (rb)
                {
                    damageInfo.force *= Mathf.Max(rb.mass / 100f, 1f);
                }
            }
            orig(self, damageInfo);
        }
    }
}
