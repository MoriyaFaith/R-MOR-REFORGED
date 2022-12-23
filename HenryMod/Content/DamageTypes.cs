using RoR2;
using UnityEngine;
using R2API;
using RMORMod.Content.HANDSurvivor.Components.Body;
using UnityEngine.Networking;

namespace RMORMod.Content
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType ResetVictimForce;
        public static DamageAPI.ModdedDamageType HANDPrimaryPunch;
        public static DamageAPI.ModdedDamageType HANDPrimaryHammer;
        public static DamageAPI.ModdedDamageType HANDSecondary;
        public static DamageAPI.ModdedDamageType HANDSecondaryScepter;
        public static DamageAPI.ModdedDamageType SquashOnKill;

        private static bool initialized = false;

        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            DamageTypes.ResetVictimForce = DamageAPI.ReserveDamageType();
            DamageTypes.HANDPrimaryPunch = DamageAPI.ReserveDamageType();
            DamageTypes.HANDPrimaryHammer = DamageAPI.ReserveDamageType();
            DamageTypes.HANDSecondary = DamageAPI.ReserveDamageType();
            DamageTypes.HANDSecondaryScepter = DamageAPI.ReserveDamageType();
            DamageTypes.SquashOnKill = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                CharacterBody cb = self.body;

                if (damageInfo.attacker)
                {
                    if (damageInfo.HasModdedDamageType(DamageTypes.SquashOnKill))
                    {
                        HANDNetworkComponent hnc = damageInfo.attacker.GetComponent<HANDNetworkComponent>();
                        if (hnc)
                        {
                            if (cb.master)
                            {
                                NetworkIdentity ni = cb.master.GetComponent<NetworkIdentity>();
                                if (ni)
                                {
                                    hnc.SquashEnemy(ni.netId.Value);
                                }
                            }
                        }
                    }
                }

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
                        damageInfo.force.x *= 0.4375f;//0.5 * 7/8
                        damageInfo.force.z *= 0.4375f;
                    }
                    else if (cb.characterMotor != null)
                    {
                        if (!cb.characterMotor.isGrounded)    //Multiply launched enemy force
                        {
                            damageInfo.force.x *= 1.575f;//1.8 * 7/8
                            damageInfo.force.z *= 1.575f;

                            if (cb.isChampion)
                            {
                                damageInfo.force.x *= 0.7f;
                                damageInfo.force.z *= 0.7f;
                            }
                        }
                        else
                        {
                            if (cb.isChampion) //deal less knockback against bosses if they're on the ground
                            {
                                damageInfo.force.x *= 0.4375f;//0.5 * 7/8
                                damageInfo.force.z *= 0.4375f;
                            }
                        }
                    }

                    if (cb.rigidbody)
                    {
                        damageInfo.force *= Mathf.Max(cb.rigidbody.mass / 100f, 1f);
                    }

                    //Plays the DRONE sound when spawned via BaseMeleeAttack hitEffectPrefab for some reason.
                    EffectManager.SimpleEffect(EntityStates.HAND_Junked.Primary.SwingStab.hitEffect, damageInfo.position, default, true);
                }

                //Make sure this doesn't stack with punch damagetype.
                if (damageInfo.HasModdedDamageType(DamageTypes.HANDPrimaryHammer))
                {
                    if (cb.isFlying)
                    {
                        damageInfo.force.x *= 0.4375f;//0.5 * 7/8
                        damageInfo.force.z *= 0.4375f;
                    }
                    else if (cb.characterMotor != null)
                    {
                        if (!cb.characterMotor.isGrounded)    //Multiply launched enemy force
                        {
                            //damageInfo.force.x *= 1.2f;
                            //damageInfo.force.z *= 1.2f;

                            if (cb.isChampion)
                            {
                                damageInfo.force.x *= 0.7f;
                                damageInfo.force.z *= 0.7f;
                            }
                        }
                        else
                        {
                            if (cb.isChampion) //deal less knockback against bosses if they're on the ground
                            {
                                damageInfo.force.x *= 0.5f;
                                damageInfo.force.z *= 0.5f;
                            }
                        }

                        //Plays the DRONE sound when spawned via BaseMeleeAttack hitEffectPrefab for some reason.
                        EffectManager.SimpleEffect(EntityStates.HAND_Junked.Primary.SwingHammer.hitEffect, damageInfo.position, default, true);
                    }

                    if (cb.rigidbody)
                    {
                        float forceMult = Mathf.Max(cb.rigidbody.mass / 100f, 1f);
                        damageInfo.force *= forceMult;

                        if (cb.isFlying)
                        {
                            damageInfo.force += 1500f * Vector3.down * Mathf.Min(7.5f, forceMult);
                        }
                    }
                }

                bool isSecondary = damageInfo.HasModdedDamageType(DamageTypes.HANDSecondary);
                bool isScepter = damageInfo.HasModdedDamageType(DamageTypes.HANDSecondaryScepter);
                if (isSecondary || isScepter)
                {
                    bool launchEnemy = false;
                    //Downwards force is determined when setting up the attack.
                    //Force gets overwritten into upwards force if target is grounded.
                    if (cb.characterMotor && cb.characterMotor.isGrounded)
                    {
                        launchEnemy = true;
                        damageInfo.force.y = 2000f;
                    }

                    if (cb.rigidbody)
                    {
                        float forceMult = Mathf.Max(cb.rigidbody.mass / 100f, 1f);
                        if (!launchEnemy && !isScepter)
                        {
                            forceMult = Mathf.Min(7.5f, forceMult);
                        }
                        damageInfo.force *= forceMult;
                    }

                    //Plays the DRONE sound when spawned via BaseMeleeAttack hitEffectPrefab for some reason.
                    EffectManager.SimpleEffect(EntityStates.HAND_Junked.Secondary.FireSlam.hitEffect, damageInfo.position, default, true);
                }
            }
            orig(self, damageInfo);
        }
    }
}
