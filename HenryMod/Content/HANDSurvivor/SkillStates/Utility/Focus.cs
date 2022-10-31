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

		public override SkillDef GetCancelDef()
		{
			return SkillDefs.UtilityFocusCancel;
		}

		public override void StartOverclock()
		{
			if (this.overclockController)
			{
				this.overclockController.BeginFocus();
			}
		}

        public override void FixedUpdate()
        {
            base.FixedUpdate();
			if (base.characterBody) base.characterBody.isSprinting = false;
        }
    }

    public class CancelFocus : CancelOverclock
	{
        public override void EndOverclock()
        {
			if (this.overclockController)
			{
				this.overclockController.EndFocus();
			}
		}
    }
}
