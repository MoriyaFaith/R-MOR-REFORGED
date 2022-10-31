using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.HANDSurvivor.Components.Body;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.HAND_Overclocked.Utility
{
    public class BeginFocus : BeginOverclock
    {
        public static Texture2D texGaugeNemesis;
        public static Texture2D texGaugeArrowNemesis;

		public virtual SkillDef GetCancelDef()
		{
			return SkillDefs.UtilityFocusCancel;
		}

		public virtual void StartOverclock()
		{
			if (this.overclockController)
			{
				this.overclockController.BeginOverclock();
			}
		}
	}

    public class CancelFocus : CancelOverclock
    {

    }
}
