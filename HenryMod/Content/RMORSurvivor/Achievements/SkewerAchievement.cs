using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System;

namespace RMORMod.Content.RMOR.Achievements
{
    [RegisterAchievement("MoriyaRMORSkewerUnlock", "Skills.RMOR.Skewer", null, null)]
    public class SkewerAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RMORBody");
        }

        private void CheckTeleporter(TeleporterInteraction teleporterInteraction)
        {
            if (!this.failed && this.meetsBodyRequirement)
            {
                base.Grant();
            }
        }
        public override void OnInstall()
        {
            base.OnInstall();
            RoR2Application.onFixedUpdate += this.CheckHealth;
            TeleporterInteraction.onTeleporterChargedGlobal += this.CheckTeleporter;
        }
        public override void OnUninstall()
        {
            RoR2Application.onFixedUpdate -= this.CheckHealth;
            TeleporterInteraction.onTeleporterChargedGlobal -= this.CheckTeleporter;
            base.OnUninstall();
        }
        public override void OnBodyRequirementBroken()
        {
            this.Fail();
            base.OnBodyRequirementBroken();
        }
        private void Fail()
        {
            this.failed = true;
        }

        private void CheckHealth()
        {
            if (this.healthComponent && this.healthComponent.combinedHealth < (this.healthComponent.fullCombinedHealth * 0.6f)) //40% Health
            {
                this.Fail();
            }
        }

        private HealthComponent healthComponent;
        private bool failed;
    }
}
