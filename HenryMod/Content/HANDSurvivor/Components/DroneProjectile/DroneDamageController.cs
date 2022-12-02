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
                durationBetweenTicks = baseDurationBetweenTicks;
                tickCount = baseTickCount;
            }
        }

        public void Start()
        {
            if (projectileController)
            {
                owner = projectileController.owner;
                if (owner)
                {
                    ownerHealthComponent = projectileController.owner.GetComponent<HealthComponent>();
                    TeamComponent tc = owner.GetComponent<TeamComponent>();
                    teamIndex = tc.teamIndex;
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

                        tickCount = Mathf.FloorToInt(tickCount * droneAttackSpeed);
                        durationBetweenTicks /= droneAttackSpeed;
                    }
                }
            }
        }

        private void OnFirstHit()
        {
            if (victimHealthComponent.body)
            {
                if (victimHealthComponent.body.teamComponent && victimHealthComponent.body.teamComponent.teamIndex == teamIndex)
                {
                    victimHealthComponent.body.AddTimedBuff(RoR2Content.Buffs.SmallArmorBoost, (float)tickCount * durationBetweenTicks);
                }
            }
            EffectManager.SimpleSoundEffect(startSound.index, base.transform.position, true);
        }

        private bool CheckValidVictim()
        {
            bool valid = true;
            if (!victimHealthComponent)
            {
                victimHealthComponent = stick.victim.GetComponent<HealthComponent>();
                if (!victimHealthComponent)
                {
                    valid = false;
                }
            }
            else if (victimHealthComponent && !victimHealthComponent.alive)
            {
                valid = false;
            }
            return valid;
        }

        private void VictimFixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch > durationBetweenTicks)
            {
                damageTicks++;
                if (damageTicks > tickCount)
                {
                    Destroy(this.gameObject);
                }

                if (victimHealthComponent && projectileDamage)
                {
                    if (firstHit)
                    {
                        firstHit = false;
                        OnFirstHit();
                    }
                    else
                    {
                        EffectManager.SimpleSoundEffect(hitSound.index, base.transform.position, true);
                    }

                    float currentTickDamage = projectileDamage.damage / (float)DroneDamageController.baseTickCount;
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
                        float minTotalHeal = victimHealthComponent.fullHealth * 0.1f / (float)DroneDamageController.baseTickCount;

                        HealOrb healOrb = new HealOrb();
                        healOrb.origin = this.transform.position;
                        healOrb.target = victimHealthComponent.body.mainHurtBox;
                        healOrb.healValue = Mathf.Max(minTotalHeal, currentTickDamage);
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
                        GlobalEventManager.instance.OnHitAll(droneDamage, victimHealthComponent.gameObject);

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
                                missileOrb.speed = 55f;
                                OrbManager.instance.AddOrb(missileOrb);
                            }
                        }
                    }
                }
                EffectManager.SimpleEffect(DroneDamageController.hitEffectPrefab, base.transform.position, default, true);
                stopwatch -= durationBetweenTicks;
            }
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (stick.stuck)
                {
                    if (stick.victim)
                    {
                        bool validVictim = CheckValidVictim();
                        if (!validVictim)
                        {
                            Destroy(base.gameObject);
                            return;
                        }

                        VictimFixedUpdate();
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
        }

        public static float procCoefficient = 0.5f;
        public static float baseDurationBetweenTicks = 0.5f;
        public static int baseTickCount = 8;
        public static float damageHealFraction = 0.4f;
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/OmniImpactVFXSlashSyringe.prefab").WaitForCompletion();
        public static NetworkSoundEventDef startSound;
        public static NetworkSoundEventDef hitSound;

        private float durationBetweenTicks;
        private float stopwatch;
        private ProjectileStickOnImpact stick;
        private int damageTicks;
        private int tickCount;

        private bool firstHit;

        private int dronePartsCount = 0;
        private int coolantCount = 0;

        private CharacterMaster master;
        private GameObject owner;
        private TeamIndex teamIndex;
        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
        private HealthComponent ownerHealthComponent;
        private HealthComponent victimHealthComponent;
    }
}
