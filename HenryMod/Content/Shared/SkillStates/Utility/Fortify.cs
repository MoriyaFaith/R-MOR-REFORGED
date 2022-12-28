using RMORMod.Content.HANDSurvivor;
using RMORMod.Content.HANDSurvivor.Components.Body;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RMOR.Utility
{
    public class BeginFortify : BeginOverclock
    {
        public static Texture2D texGaugeFortify;
        public static Texture2D texGaugeArrowFortify;
        public static new Material overlayMaterial;

        public override void LoadStats()
        {
            cancelDef = RMORMod.Content.Shared.SkillDefs.UtilityOverclockCancel;
            buffDef = RMORMod.Content.Shared.Buffs.Fortify;
            gaugeInternal = texGaugeFortify;
            gaugeArrowInternal = texGaugeArrowFortify;
            internalOverlayMaterial = BeginFortify.overlayMaterial;
            //fortifyMat.SetTexture();
            internalOverlayMaterial.SetTexture("_RemapTex", RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texFortifyColor"));
            internalOverlayMaterial.SetColor("_TintColor", new Color(0, 1, 0));
        }
    }
}
