using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    public class OverclockController : NetworkBehaviour
    {
        public void Awake()
        {
            buffTimer = 0f;
            buffActive = false;
            characterBody = base.GetComponent<CharacterBody>();
            healthComponent = characterBody.healthComponent;

            rectGauge = new Rect();
            rectGaugeArrow = new Rect();
        }

        public void Start()
        {
            if (characterBody.modelLocator && characterBody.modelLocator.modelTransform)
            {
                characterModel = characterBody.modelLocator.modelTransform.GetComponent<CharacterModel>();
            }
        }

        public void FixedUpdate()
        {
            if (buffActive)
            {
                characterBody.skillLocator.utility.rechargeStopwatch = initialSkillCooldown;

                buffTimer -= Time.fixedDeltaTime;
                buffPercent = buffTimer / OverclockController.OverclockDuration;

                if (buffTimer > OverclockController.OverclockDuration)
                {
                    buffTimer = OverclockController.OverclockDuration;
                }
                else if (buffTimer < 0f)
                {
                    if (characterBody.skillLocator.utility.stock > 0)
                    {
                        characterBody.skillLocator.utility.stock--;
                    }
                    if (overclockActive) EndOverclock();
                    if (focusActive) EndFocus();
                }
            }
        }

        public void Update()
        {
            if (OverclockController.overclockMat) UpdateOverlay();
        }

        private void UpdateOverlay()
        {
            if (characterBody.HasBuff(Buffs.Overclock))
            {
                if (!overlay)
                {
                    overlay = characterModel.gameObject.AddComponent<TemporaryOverlay>();
                    overlay.duration = Mathf.Infinity;
                    overlay.animateShaderAlpha = true;
                    overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    overlay.destroyComponentOnEnd = true;
                    overlay.originalMaterial = OverclockController.overclockMat;
                    overlay.AddToCharacerModel(characterModel);

                }
            }
            else
            {
                if (overlay)
                {
                    overlay.RemoveFromCharacterModel();
                    UnityEngine.Object.Destroy(overlay);
                    overlay = null;
                }
            }
        }

        public void MeleeHit()
        {
            if (characterBody.skillLocator.special.stock < characterBody.skillLocator.special.maxStock && characterBody.skillLocator.special.skillDef == SkillDefs.SpecialDrone)
            {
                characterBody.skillLocator.special.rechargeStopwatch += 1.2f;
            }
        }

        public void ExtendOverclock(float time)
        {
            if (buffActive && allowExtend)
            {
                buffTimer += time;
            }
        }

        public void BeginFocus()
        {
            this.texGauge = EntityStates.HAND_Overclocked.Utility.BeginFocus.texGaugeNemesis;
            this.texGaugeArrow = EntityStates.HAND_Overclocked.Utility.BeginFocus.texGaugeArrowNemesis;
            allowExtend = false;
            if (hasAuthority)
            {
                ReiszeOverclockGauge();
                buffTimer = OverclockController.OverclockDuration;
                buffPercent = 1f;
                buffActive = true;
                focusActive = true;
                initialSkillCooldown = characterBody.skillLocator.utility.rechargeStopwatch;

                CmdBeginFocusServer();
            }
        }

        [Command]
        private void CmdBeginFocusServer()
        {
            RpcPlayOverclockStart();
            if (!characterBody.HasBuff(Buffs.NemesisFocus))
            {
                characterBody.AddBuff(Buffs.NemesisFocus);
            }
            focusActive = true;
            allowExtend = false;
        }

        public void EndFocus()
        {
            if (hasAuthority)
            {
                focusActive = false;
                buffActive = false;
                buffTimer = 0;
                CmdEndFocusServer();
            }
        }

        [Command]
        private void CmdEndFocusServer()
        {
            if (characterBody.HasBuff(Buffs.NemesisFocus))
            {
                characterBody.RemoveBuff(Buffs.NemesisFocus);
            }
            focusActive = false;
            RpcPlayOverclockEnd();
        }

        public void BeginOverclock()
        {
            this.texGauge = EntityStates.HAND_Overclocked.Utility.BeginOverclock.texGauge;
            this.texGaugeArrow = EntityStates.HAND_Overclocked.Utility.BeginOverclock.texGaugeArrow;
            allowExtend = true;
            if (hasAuthority)
            {
                ReiszeOverclockGauge();
                buffTimer = OverclockController.OverclockDuration;
                buffPercent = 1f;
                buffActive = true;
                overclockActive = true;
                initialSkillCooldown = characterBody.skillLocator.utility.rechargeStopwatch;

                CmdBeginOverclockServer();
            }
        }

        [Command]
        private void CmdBeginOverclockServer()
        {
            RpcPlayOverclockStart();
            if (!characterBody.HasBuff(Buffs.Overclock))
            {
                characterBody.AddBuff(Buffs.Overclock);
            }
            allowExtend = true;
            overclockActive = true;
        }

        public void EndOverclock()
        {
            if (hasAuthority)
            {
                overclockActive = false;
                buffActive = false;
                buffTimer = 0;
                CmdEndOverclockServer();
            }
        }

        [Command]
        private void CmdEndOverclockServer()
        {
            if (characterBody.HasBuff(Buffs.Overclock))
            {
                characterBody.RemoveBuff(Buffs.Overclock);
            }
            overclockActive = false;
            RpcPlayOverclockEnd();
        }

        [ClientRpc]
        public void RpcPlayOverclockEnd()
        {
            Util.PlaySound("Play_MULT_shift_end", base.gameObject);
        }

        [ClientRpc]
        public void RpcPlayOverclockStart()
        {
            Util.PlaySound("Play_MULT_shift_start", base.gameObject);
        }

        private void ReiszeOverclockGauge()
        {
            float height = Camera.current.pixelHeight;
            float width = Camera.current.pixelWidth;

            rectGauge.width = height * texGauge.width * gaugeScale / 1080f;
            rectGauge.height = height * texGauge.height * gaugeScale / 1080f;

            
            rectGauge.position = new Vector2(width / 2f - rectGauge.width / 2f, height / 2f + rectGauge.height * 2f);

            rectGaugeArrow.width = height * texGaugeArrow.width * gaugeScale / 1080f;
            rectGaugeArrow.height = height * texGaugeArrow.height * gaugeScale / 1080f;

            gaugeLeftBound = rectGauge.position.x - rectGaugeArrow.width / 2f;
            gaugeRightBound = gaugeLeftBound + rectGauge.width;
            gaugeArroyYPos = height / 2f + rectGauge.height * 2f;
        }
        private void OnGUI()
        {
            if (this.hasAuthority && buffActive && !menuActive && !RoR2.PauseManager.isPaused && healthComponent && healthComponent.alive)
            {
                GUI.DrawTexture(rectGauge, texGauge, ScaleMode.StretchToFill, true, 0f);

                rectGaugeArrow.position = new Vector2(Mathf.Lerp(gaugeLeftBound, gaugeRightBound, buffPercent), gaugeArroyYPos);
                GUI.DrawTexture(rectGaugeArrow, texGaugeArrow, ScaleMode.StretchToFill, true, 0f);
            }
        }

        public bool buffActive
        {
            get
            {
                return _buffActive;
            }
            protected set
            {
                _buffActive = value;
            }
        }
        private bool _buffActive;

        public float buffTimer
        {
            get
            {
                return _ovcTimer;
            }
            protected set
            {
                _ovcTimer = value;
            }
        }
        private float _ovcTimer;

        private CharacterModel characterModel;
        public static Material overclockMat;
        private TemporaryOverlay overlay;

        public static float OverclockDuration = 4f;

        private CharacterBody characterBody;
        private HealthComponent healthComponent;

        public static SkillDef ovcDef;

        private bool allowExtend = true;
        public Texture2D texGauge;
        public Texture2D texGaugeArrow;
        private Rect rectGauge;
        private Rect rectGaugeArrow;
        public static float gaugeScale = 0.3f;
        private float gaugeLeftBound;
        private float gaugeRightBound;
        private float gaugeArroyYPos;

        private float initialSkillCooldown;
        private float buffPercent;

        public bool menuActive = false;

        private bool overclockActive = false;
        private bool focusActive = false;
    }
}
