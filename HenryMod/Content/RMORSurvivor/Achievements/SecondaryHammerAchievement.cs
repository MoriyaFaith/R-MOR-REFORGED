using RoR2;
using RoR2.Achievements;
using UnityEngine;
using R2API;
using RMORMod.Content.HANDSurvivor.Components;

namespace RMORMod.Content.RMOR.Achievements
{
    [RegisterAchievement("MoffeinHANDOverclockedHammerPrimaryUnlock", "Skills.HANDOverclocked.HammerPrimary", null, null)]
    public class HANDOverclockedHammerPrimaryUnlockAchievement : BaseAchievement
    {
        BodyIndex mithrixBody;
        BodyIndex voidlingBody;
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("HANDOverclockedBody");
        }
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            mithrixBody = BodyCatalog.FindBodyIndex("BrotherHurtBody");
            voidlingBody = BodyCatalog.FindBodyIndex("VoidRaidCrabBody");
            SquashedComponent.onSquashedGlobal += SquashedComponent_onSquashedGlobal;
        }

        public override void OnBodyRequirementBroken()
        {
            SquashedComponent.onSquashedGlobal -= SquashedComponent_onSquashedGlobal;
            base.OnBodyRequirementBroken();
        }

        private void SquashedComponent_onSquashedGlobal(SquashedComponent sq)
        {
            if (sq.triggerer)
            {
                CharacterBody triggererBody = sq.triggerer.GetComponent<CharacterBody>();
                if (triggererBody == base.localUser.cachedBody)
                {
                    BodyIndex victimBodyIndex = sq.GetBodyIndex();
                    if (victimBodyIndex == mithrixBody || victimBodyIndex == voidlingBody)
                    {
                        base.Grant();
                    }
                }
            }
        }
    }
}
