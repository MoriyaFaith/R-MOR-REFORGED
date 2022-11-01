using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    public class OverclockController : NetworkBehaviour
    {
        public void StartOverclock(Texture2D gauge, Texture2D arrow)
        {
            buffActive = true;
            texGauge = gauge;
            texGaugeArrow = arrow;
        }
        public void EndOverclock()
        {
            buffActive = false;
            texGauge = null;
            texGaugeArrow = null;
            extensionTime = 0f;
        }

        public void Awake()
        {
            rectGauge = new Rect();
            rectGaugeArrow = new Rect();
        }

        public void ExtendOverclock(float time)
        {
            if (buffActive)
            {
                extensionTime += time;
            }
        }
        public float ConsumeExtensionTime()
        {
            float time = extensionTime;
            extensionTime = 0f;
            return time;
        }

        #region GUI stuff
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
            if (this.hasAuthority && buffActive && !menuActive && !RoR2.PauseManager.isPaused)
            {
                GUI.DrawTexture(rectGauge, texGauge, ScaleMode.StretchToFill, true, 0f);

                rectGaugeArrow.position = new Vector2(Mathf.Lerp(gaugeLeftBound, gaugeRightBound, buffPercent), gaugeArroyYPos);
                GUI.DrawTexture(rectGaugeArrow, texGaugeArrow, ScaleMode.StretchToFill, true, 0f);
            }
        }
        #endregion

        public float buffPercent = 0f;
        private bool buffActive = false;
        private float extensionTime = 0f;

        public static SkillDef ovcDef;

        public Texture2D texGauge;
        public Texture2D texGaugeArrow;
        private Rect rectGauge;
        private Rect rectGaugeArrow;
        public static float gaugeScale = 0.3f;
        private float gaugeLeftBound;
        private float gaugeRightBound;
        private float gaugeArroyYPos;

        public bool menuActive = false;
    }
}
