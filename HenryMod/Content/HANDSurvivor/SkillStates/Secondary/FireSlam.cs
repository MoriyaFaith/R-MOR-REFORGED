using UnityEngine;
using RoR2;
using HANDMod.SkillStates.BaseStates;
using HANDMod.Content.HANDSurvivor.Components.Body;
using R2API;
using HANDMod.Content.HANDSurvivor;
using HANDMod.Content;

namespace EntityStates.HAND_Overclocked.Secondary
{
    public class FireSlam : BaseMeleeAttack
    {
        public static GameObject swingEffect;

        public float chargePercent = 0f;

        public static float minDamageCoefficient = 5f;
        public static float maxDamageCoefficient = 15f;

        public static float minDownForce = 2400f;
        public static float maxDownForce = 3200f;

        public static float baseYPos = -14f;

        public static float baseYScale = 30f;
        public static float maxYScale = 60f;

        public static NetworkSoundEventDef networkHitSound;
        public static GameObject earthquakeEffectPrefab;

        private HammerVisibilityController hammerController;

        private bool spawnedEffects = false;
        private bool hitEnemy = false;
        public override void OnEnter()
        {
            //this.pushForce = 300f;
            //this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;

            //this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = null;
            this.hitEffectPrefab = null;
            this.impactSound = networkHitSound.index;

            this.damageType = DamageType.Stun1s;
            this.hitHopVelocity = 22f;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "";
            this.swingSoundString = "";
            this.hitboxName = "ChargeHammerHitbox";
            this.damageCoefficient = Mathf.Lerp(FireSlam.minDamageCoefficient, FireSlam.maxDamageCoefficient, chargePercent);
            this.procCoefficient = 1f;
            this.baseDuration = 0.8f;
            this.baseEarlyExitTime = 0.2f;
            this.attackStartTime = 0.4f;
            this.attackEndTime = 0.5f;
            this.pushForce = 0f;
            this.bonusForce = Vector3.down * Mathf.Lerp(FireSlam.minDownForce, FireSlam.maxDownForce, chargePercent);

            ModifyStats();

            hammerController = base.GetComponent<HammerVisibilityController>();
            if (hammerController)
            {
                hammerController.SetHammerEnabled(true);
            }

            //Hitreg is clientside, so only change it on the client
            if (base.isAuthority)
            {
                ChildLocator cl = base.GetModelChildLocator();
                if (cl)
                {
                    Transform chargeHammerHitboxTransform = cl.FindChild("ChargeHammerHitbox");
                    //Debug.Log(chargeHammerHitboxTransform.localPosition);
                    if (chargeHammerHitboxTransform)
                    {
                        float yScale = Mathf.Lerp(FireSlam.baseYScale, FireSlam.maxYScale, chargePercent);
                        float yOffset = baseYPos - (yScale - baseYScale) * 0.5f;
                        chargeHammerHitboxTransform.localScale = new Vector3(chargeHammerHitboxTransform.localScale.x, yScale, chargeHammerHitboxTransform.localScale.z);
                        chargeHammerHitboxTransform.localPosition = new Vector3(chargeHammerHitboxTransform.localPosition.x, yOffset, chargeHammerHitboxTransform.localPosition.z);
                    }
                }
            }

            base.OnEnter();

            if (this.attack != null)
            {
                ModifyDamageTypes();
            }

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(3f);
            }
        }

        public virtual void ModifyStats() { }
        public virtual void ModifyDamageTypes()
        {
            this.attack.AddModdedDamageType(DamageTypes.HANDSecondary);
            this.attack.AddModdedDamageType(DamageTypes.SquashOnKill);
            this.attack.AddModdedDamageType(DamageTypes.ResetVictimForce);
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Gesture, Override", "FireHammer", "ChargeHammer.playbackRate", this.duration);
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (!hitEnemy)
            {
                hitEnemy = true;
                if (base.characterBody && base.characterBody.HasBuff(Buffs.Overclock))
                {
                    OverclockController hc = base.gameObject.GetComponent<OverclockController>();
                    if (hc)
                    {
                        hc.MeleeHit();
                        hc.ExtendOverclock(Mathf.Lerp(0.8f, 1.6f, chargePercent));
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.hasFired && !spawnedEffects)
            {
                spawnedEffects = true;
                Util.PlaySound("Play_parent_attack1_slam", base.gameObject);
                Util.PlaySound("Play_UI_podImpact", base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();
                    Vector3 directionFlat = aimRay.direction;
                    directionFlat.y = 0;
                    directionFlat.Normalize();

                    //These cover base hitbox
                    EffectManager.SpawnEffect(FireSlam.earthquakeEffectPrefab, new EffectData
                    {
                        origin = base.transform.position + (0 + 4) * directionFlat - 2f * Vector3.up,
                        scale = 0.5f
                    }, true); ;
                    EffectManager.SpawnEffect(FireSlam.earthquakeEffectPrefab, new EffectData
                    {
                        origin = base.transform.position + (0 + 8) * directionFlat - 2f * Vector3.up,
                        scale = 0.5f
                    }, true); ;

                    if (chargePercent >= 0.5f)
                        EffectManager.SpawnEffect(FireSlam.earthquakeEffectPrefab, new EffectData
                    {
                        origin = base.transform.position + (0 + 12) * directionFlat -2f * Vector3.up,
                        scale = 0.5f
                    }, true); ;


                    if (chargePercent >= 1f)
                        EffectManager.SpawnEffect(FireSlam.earthquakeEffectPrefab, new EffectData
                    {
                        origin = base.transform.position + (0 + 16) * directionFlat - 2f * Vector3.up,
                        scale = 0.5f
                    }, true); ;

                    //Allow hammer to break fall, but dont make it springy like OVC.
                    /*if (base.characterMotor && !hitEnemy)
                    {
                        if (base.characterMotor.velocity.y < 0)
                        {
                            base.SmallHop(base.characterMotor, 10f);
                        }
                    }*/
                }
            }
        }

        public override void OnExit()
        {
            if (hammerController)
            {
                hammerController.SetHammerEnabled(false);
            }
            if (!this.outer.destroying)
            {
                this.PlayAnimation("Gesture, Override", "Empty");
            }
            base.OnExit();
        }

        protected override void SetNextState()
        {
            this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.hasFired && base.fixedAge >= (this.duration - this.earlyExitTime))
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.PrioritySkill;
        }
    }
}
