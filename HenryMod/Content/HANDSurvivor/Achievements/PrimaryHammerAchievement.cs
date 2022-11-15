using RoR2;
using RoR2.Achievements;
using UnityEngine;
using R2API;

namespace HANDMod.Content.HANDSurvivor.Achievements
{
    [RegisterAchievement("MoffeinHANDOverclockedHammerPrimaryUnlock", "Skills.HANDOverclocked.HammerPrimary", null, typeof(HANDOverclockedHammerPrimaryUnlockServerAchievement))]
    public class HANDOverclockedHammerPrimaryUnlockAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("HANDOverclockedBody");
        }
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }

        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }

        private class HANDOverclockedHammerPrimaryUnlockServerAchievement : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
            }

            public override void OnUninstall()
            {
                GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
                base.OnUninstall();
            }
            private void OnCharacterDeath(DamageReport damageReport)
            {
                if (!damageReport.victimBody)
                {
                    return;
                }
                Debug.Log(damageReport.victimIsBoss);
                if (base.IsCurrentBody(damageReport.attackerBody)
                    && damageReport.victimIsBoss
                    && (damageReport.damageInfo.HasModdedDamageType(DamageTypes.HANDSecondary) || damageReport.damageInfo.HasModdedDamageType(DamageTypes.HANDSecondaryScepter)))
                {
                    base.Grant();
                }
            }
        }
    }
}
