using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.DroneProjectile
{
    public class DroneDamageController : MonoBehaviour
    {
        private bool playedHitSound = false;
        private int dronePartsCount = 0;
        private int coolantCount = 0;

        public void Awake()
        {
            if (NetworkServer.active)
            {
                stick = this.gameObject.GetComponent<ProjectileStickOnImpact>();
                stopwatch = 0f;
                damageTicks = 0;
                firstHit = true;
                projectileDamage = this.gameObject.GetComponent<ProjectileDamage>();
                projectileController = this.gameObject.GetComponent<ProjectileController>();
                tickRate = baseTickRate;

                if (projectileController)
                {
                    owner = projectileController.owner;
                    if (owner)
                    {
                        CharacterBody cb = owner.GetComponent<CharacterBody>();
                        if (cb && cb.inventory)
                        {
                            master = cb.master;
                            float droneAttackSpeed = 1f;

                            dronePartsCount = cb.inventory.GetItemCount(DLC1Content.Items.DroneWeapons);
                            if (dronePartsCount > 0)
                            {
                                droneAttackSpeed += 0.5f * dronePartsCount;
                            }

                            ItemIndex droneCoolantIndex = ItemCatalog.FindItemIndex("ITEM_DRONE_COOLANT_BOOST");
                            if (droneCoolantIndex != ItemIndex.None)
                            {
                                coolantCount = cb.inventory.GetItemCount(droneCoolantIndex);
                                if (coolantCount > 0)
                                {
                                    droneAttackSpeed += 0.1f * coolantCount;
                                }
                            }

                            damageTicks = Mathf.FloorToInt(damageTicks * droneAttackSpeed);
                            tickRate = tickRate / droneAttackSpeed;
                        }
                    }
                }
            }
        }

        public void OnDestroy()
        {
            if (bleedEffect)
            {
                Destroy(bleedEffect);
            }
            if (NetworkServer.active)
            {
                if (damageTicks < damageTicksTotal && ownerHealthComponent)
                {
                    HealOrb healOrb = new HealOrb();
                    healOrb.origin = this.transform.position;
                    healOrb.target = ownerHealthComponent.body.mainHurtBox;
                    healOrb.healValue = projectileDamage.damage * DroneDamageController.damageHealFraction;
                    if (projectileDamage && projectileDamage.crit)
                    {
                        float ownerCritMult = 2f;
                        if (ownerHealthComponent.body) ownerCritMult = ownerHealthComponent.body.critMultiplier;
                        healOrb.healValue *= ownerCritMult;
                    }
                    float remainingHealMult = (damageTicksTotal - damageTicks) / (float)damageTicksTotal;
                    healOrb.healValue *= remainingHealMult;

                     healOrb.overrideDuration = 0.3f;
                    OrbManager.instance.AddOrb(healOrb);
                }
            }
        }

        public void FixedUpdate()
        {
            if (!firstHit && !bleedEffect)
            {
                bleedEffect = UnityEngine.Object.Instantiate<GameObject>(bleedEffectPrefab, this.transform);
            }
            if (NetworkServer.active)
            {
                if (projectileController && !owner)
                {
                    owner = projectileController.owner;
                    if (owner)
                    {
                        ownerHealthComponent = projectileController.owner.GetComponent<HealthComponent>();
                        TeamComponent tc = owner.GetComponent<TeamComponent>();
                        teamIndex = tc.teamIndex;
                    }
                }
                if (stick.stuck)
                {
                    if (stick.victim)
                    {
                        if (!victimHealthComponent)
                        {
                            victimHealthComponent = stick.victim.GetComponent<HealthComponent>();
                            if (!victimHealthComponent)
                            {
                                Destroy(this.gameObject);
                                return;
                            }
                        }
                        else if (victimHealthComponent && !victimHealthComponent.alive)
                        {
                            Destroy(this.gameObject);
                            return;
                        }

                        stopwatch += Time.fixedDeltaTime;
                        if (stopwatch > tickRate)
                        {
                            damageTicks++;
                            if (damageTicks > damageTicksTotal)
                            {
                                Destroy(this.gameObject);
                            }

                            if (!playedHitSound)
                            {
                                playedHitSound = true;
                                EffectManager.SimpleSoundEffect(startSound.index, base.transform.position, true);
                            }


                            if (victimHealthComponent && projectileDamage)
                            {
                                if (firstHit)
                                {
                                    firstHit = false;
                                    if (victimHealthComponent.body)
                                    {
                                        if (victimHealthComponent.body.teamComponent && victimHealthComponent.body.teamComponent.teamIndex == teamIndex)
                                        {
                                            victimHealthComponent.body.AddTimedBuff(RoR2Content.Buffs.SmallArmorBoost, (float)damageTicksTotal * baseTickRate);
                                        }
                                        else
                                        {
                                            //victimHealthComponent.body.AddTimedBuff(Buffs.DroneDebuff, (float)damageTicksTotal * damageTimer);
                                        }
                                    }
                                }

                                float currentTickDamage = projectileDamage.damage / (float)damageTicksTotal;
                                float ownerCritMult = 2f;

                                if (ownerHealthComponent)
                                {
                                    ownerCritMult = ownerHealthComponent.body.critMultiplier;
                                    HealOrb healOrb = new HealOrb();
                                    healOrb.origin = this.transform.position;
                                    healOrb.target = ownerHealthComponent.body.mainHurtBox;
                                    healOrb.healValue = damageHealFraction * currentTickDamage;
                                    if (projectileDamage.crit) healOrb.healValue *= ownerCritMult;
                                    healOrb.overrideDuration = 0.3f;
                                    OrbManager.instance.AddOrb(healOrb);
                                }

                                if (victimHealthComponent.body && victimHealthComponent.body.teamComponent && victimHealthComponent.body.teamComponent.teamIndex == teamIndex)
                                {
                                    HealOrb healOrb = new HealOrb();
                                    healOrb.origin = this.transform.position;
                                    healOrb.target = victimHealthComponent.body.mainHurtBox;
                                    healOrb.healValue = currentTickDamage;
                                    if (projectileDamage.crit) healOrb.healValue *= ownerCritMult;
                                    healOrb.overrideDuration = 0.3f;
                                    OrbManager.instance.AddOrb(healOrb);
                                }
                                else
                                {
                                    DamageInfo droneDamage = new DamageInfo
                                    {
                                        attacker = owner,
                                        inflictor = owner,
                                        damage = currentTickDamage,
                                        damageColorIndex = DamageColorIndex.Default,
                                        damageType = DamageType.Generic,
                                        crit = projectileDamage.crit,
                                        dotIndex = DotController.DotIndex.None,
                                        force = projectileDamage.force * Vector3.down,
                                        position = this.transform.position,
                                        procChainMask = default(ProcChainMask),
                                        procCoefficient = procCoefficient
                                    };

                                    //Coolant scales burn damage but I don't want to rewrite the hook for it.
                                    if (coolantCount > 0 && Util.CheckRoll(10f + 10f * coolantCount, master))
                                    {
                                        droneDamage.damageType |= DamageType.IgniteOnHit;
                                    }

                                    victimHealthComponent.TakeDamage(droneDamage);
                                    GlobalEventManager.instance.OnHitEnemy(droneDamage, victimHealthComponent.gameObject);

                                    if (dronePartsCount > 0 && victimHealthComponent.body && victimHealthComponent.body.mainHurtBox)
                                    {
                                        if (Util.CheckRoll(10f, master))
                                        {
                                            MicroMissileOrb missileOrb = new MicroMissileOrb();
                                            missileOrb.origin = base.transform.position;
                                            missileOrb.damageValue = currentTickDamage * 3f;
                                            missileOrb.isCrit = projectileDamage.crit;
                                            missileOrb.teamIndex = teamIndex;
                                            missileOrb.attacker = owner;
                                            missileOrb.procChainMask = default;
                                            missileOrb.procCoefficient = 0.25f;
                                            missileOrb.damageColorIndex = DamageColorIndex.Item;
                                            missileOrb.target = victimHealthComponent.body.mainHurtBox;
                                            missileOrb.speed = 25f; //Same as misisleprojectile. Default is 55f
                                            missileOrb.procChainMask.AddProc(ProcType.Missile);
                                            OrbManager.instance.AddOrb(missileOrb);
                                        }
                                    }
                                }
                            }
                            EffectManager.SimpleEffect(DroneDamageController.hitEffectPrefab, base.transform.position, default, true);
                            stopwatch -= tickRate;
                        }
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
        }

        public static float procCoefficient = 0.5f;
        public static float baseTickRate = 0.5f;
        public static int damageTicksTotal = 8;
        public static float damageHealFraction = 0.5f;
        public static GameObject bleedEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/BleedEffect");
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/OmniImpactVFXSlashSyringe.prefab").WaitForCompletion();
        public static NetworkSoundEventDef startSound;

        private float tickRate;
        private float stopwatch;
        private ProjectileStickOnImpact stick;
        private int damageTicks;

        private bool firstHit;

        private GameObject bleedEffect;
        private CharacterMaster master;
        private GameObject owner;
        private TeamIndex teamIndex;
        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
        private HealthComponent ownerHealthComponent;
        private HealthComponent victimHealthComponent;
    }
}
