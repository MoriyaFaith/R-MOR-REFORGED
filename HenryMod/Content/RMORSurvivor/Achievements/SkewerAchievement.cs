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
            base.localUser.onBodyChanged += this.OnBodyChanged;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            SceneCatalog.onMostRecentSceneDefChanged += this.OnMostRecentSceneDefChanged;
            TeleporterInteraction.onTeleporterChargedGlobal += this.CheckTeleporter;
        }
        public override void OnUninstall()
        {
            base.localUser.onBodyChanged -= this.OnBodyChanged;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
			SceneCatalog.onMostRecentSceneDefChanged -= this.OnMostRecentSceneDefChanged;
            TeleporterInteraction.onTeleporterChargedGlobal -= this.CheckTeleporter;
            base.OnUninstall();
        }
        private void OnBodyChanged()
        {
            if (!this.failed && base.localUser.cachedBody)
            {
                this.healthComponent = base.localUser.cachedBody.healthComponent;
            }
        }
        private void OnMostRecentSceneDefChanged(SceneDef sceneDef)
        {

            this.failed = false;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);

            if (this.healthComponent && this.healthComponent.combinedHealth < (this.healthComponent.fullCombinedHealth * 0.6f)) //40% Health
            {
                this.failed = true;
            }
        }

        private HealthComponent healthComponent;
        private bool failed = false;
    }
}
