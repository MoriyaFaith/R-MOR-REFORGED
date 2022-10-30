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


        public static float baseYPos = 6f;

        public static float baseZScale = 25f;
        public static float maxZScale = 60f;

        public static float minRange = 9f;
        public static float maxRange = 22f;
        private float hitRange;

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


            this.damageType = DamageType.Stun1s;
            this.hitHopVelocity = 24f;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "Play_MULT_shift_hit";
            this.swingSoundString = "";
            this.hitboxName = "ChargeHammerHitbox";
            this.damageCoefficient = Mathf.Lerp(FireSlam.minDamageCoefficient, FireSlam.maxDamageCoefficient, chargePercent);
            this.procCoefficient = 1f;
            this.baseDuration = 1f;
            this.baseEarlyExitTime = 0.5f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.5f;
            this.pushForce = 0f;
            this.bonusForce = Vector3.down * Mathf.Lerp(FireSlam.minDownForce, FireSlam.maxDownForce, chargePercent);

            hammerController = base.GetComponent<HammerVisibilityController>();
            if (hammerController)
            {
                hammerController.SetHammerEnabled(true);
            }

            hitRange = Mathf.Lerp(FireSlam.minRange, FireSlam.maxRange, chargePercent);

            //Only client knows charge percent
            if (base.isAuthority)
            {
                ChildLocator cl = base.GetModelChildLocator();
                if (cl)
                {
                    Transform chargeHammerHitboxTransform = cl.FindChild("ChargeHammerHitbox");
                    //Debug.Log(chargeHammerHitboxTransform.localPosition);
                    if (chargeHammerHitboxTransform)
                    {
                        float zScale = Mathf.Lerp(FireSlam.baseZScale, FireSlam.maxZScale, chargePercent);
                        float zOffset = (zScale - baseZScale) * 0.5f;
                        chargeHammerHitboxTransform.localScale = new Vector3(chargeHammerHitboxTransform.localScale.x, chargeHammerHitboxTransform.localScale.y, zScale);
                        chargeHammerHitboxTransform.localPosition = new Vector3(chargeHammerHitboxTransform.localPosition.x, baseYPos - zOffset, chargeHammerHitboxTransform.localPosition.z);  //Hitbox is rotated in unity, which is why this is what gets changed.
                    }
                }
            }

            base.OnEnter();

            if (this.attack != null)
            {
                this.attack.AddModdedDamageType(DamageTypes.HANDSecondary);
                this.attack.AddModdedDamageType(DamageTypes.SquashOnKill);
                this.attack.AddModdedDamageType(DamageTypes.ResetVictimForce);
            }
        }


        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("Gesture, Override", "FireHammer", "ChargeHammer.playbackRate", this.duration, 0.05f);
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
                    for (int i = 5; i <= Mathf.RoundToInt(hitRange) + 1; i += 2)
                    {
                        EffectManager.SpawnEffect(FireSlam.earthquakeEffectPrefab, new EffectData
                        {
                            origin = base.transform.position + i * directionFlat - 1.8f * Vector3.up,
                            scale = 0.5f
                        }, true); ;
                    }
                }
            }
        }

        public override void OnExit()
        {
            if (hammerController)
            {
                hammerController.SetHammerEnabled(false);
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
