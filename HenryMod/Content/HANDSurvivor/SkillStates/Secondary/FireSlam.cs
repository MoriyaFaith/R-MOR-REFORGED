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
        public static GameObject swingEffect = null;

        public float chargePercent = 0f;

        public static float minDamageCoefficient = 5f;
        public static float maxDamageCoefficient = 15f;

        public static float minDownForce = 2400f;
        public static float maxDownForce = 3200f;

        public static float baseYPos = -14f;

        public static float baseYScale = 30f;
        public static float maxYScale = 60f;

        public static float baseZPos = 4.5f;
        public static float baseZScale = 30f;
        public static float maxZScale = 40f;

        public static float shortHop = 12f;
        public static float shortHopOnHit = 24f;

        public static NetworkSoundEventDef networkHitSound;
        public static GameObject earthquakeEffectPrefab;

        private HammerVisibilityController hammerController;

        private bool hitEnemy = false;
        public override void OnEnter()
        {
            //this.pushForce = 300f;
            //this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;

            //this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = null;
            this.hitEffectPrefab = null;
            if (FireSlam.networkHitSound) this.impactSound = networkHitSound.index;

            this.damageType = DamageType.Stun1s;
            this.hitHopVelocity = FireSlam.shortHopOnHit;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "";
            this.swingSoundString = "";
            this.hitboxName = "ChargeHammerHitbox";
            this.damageCoefficient = Mathf.Lerp(FireSlam.minDamageCoefficient, FireSlam.maxDamageCoefficient, chargePercent);
            this.procCoefficient = 1f;
            this.baseDuration = 0.8f;
            this.baseEarlyExitTime = 0.2f;
            this.attackStartTime = 0.35f;
            this.attackEndTime = 0.5f;
            this.pushForce = 0f;
            this.bonusForce = Vector3.down * Mathf.Lerp(FireSlam.minDownForce, FireSlam.maxDownForce, chargePercent);
            this.muzzleString = "SwingCenter";

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

                        float zScale = Mathf.Lerp(FireSlam.baseZScale, FireSlam.maxZScale, chargePercent);
                        float zOffset = baseZPos - (zScale - baseZScale) * 0.5f;

                        chargeHammerHitboxTransform.localScale = new Vector3(chargeHammerHitboxTransform.localScale.x, yScale, zScale);
                        chargeHammerHitboxTransform.localPosition = new Vector3(chargeHammerHitboxTransform.localPosition.x, yOffset, zOffset);
                    }
                }
            }

            base.OnEnter();

            if (this.attack != null)
            {
                ModifyDamageTypes();

                if (base.characterBody && base.characterBody.HasBuff(Buffs.NemesisFocus))
                {
                    this.attack.damageColorIndex = DamageColorIndex.Sniper;
                }
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
            base.PlayAnimation("FullBody, Override", "FireHammer", "ChargeHammer.playbackRate", this.duration);
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (!hitEnemy)
            {
                hitEnemy = true;
                if (base.characterBody)
                {
                    OverclockController hc = base.gameObject.GetComponent<OverclockController>();
                    if (hc)
                    {
                        hc.ExtendOverclock(Mathf.Lerp(0.8f, 1.6f, chargePercent));
                    }

                    DroneStockController dsc = base.GetComponent<DroneStockController>();
                    if (dsc)
                    {
                        dsc.MeleeHit();
                    }
                }
            }
        }

        public override void OnFiredAttack()
        {
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
                        origin = base.transform.position + (0 + 12) * directionFlat - 2f * Vector3.up,
                        scale = 0.5f
                    }, true); ;


                if (chargePercent >= 1f)
                    EffectManager.SpawnEffect(FireSlam.earthquakeEffectPrefab, new EffectData
                    {
                        origin = base.transform.position + (0 + 16) * directionFlat - 2f * Vector3.up,
                        scale = 0.5f
                    }, true); ;

                if (base.characterMotor && !base.characterMotor.isGrounded)
                {
                    base.SmallHop(base.characterMotor, FireSlam.shortHop);
                }

                ShakeEmitter se = ShakeEmitter.CreateSimpleShakeEmitter(base.transform.position, new Wave()
                {
                    amplitude = 12f,
                    cycleOffset = 0f,
                    frequency = 6f
                },
                0.75f, 30f, true);
                se.transform.parent = base.transform;
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
                this.PlayAnimation("FullBody, Override", "Empty");
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
