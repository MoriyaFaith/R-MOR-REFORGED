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
        private void SubscribeHealthCheck()
        {
            RoR2Application.onFixedUpdate += this.CheckHealth;
        }
        private void UnsubscribeHealthCheck()
        {
            RoR2Application.onFixedUpdate -= this.CheckHealth;
        }
        private void SubscribeTeleporterCheck()
        {
            TeleporterInteraction.onTeleporterChargedGlobal += this.CheckTeleporter;
        }
        private void UnsubscribeTeleporterCheck()
        {
            TeleporterInteraction.onTeleporterChargedGlobal -= this.CheckTeleporter;
        }
        private void CheckTeleporter(TeleporterInteraction teleporterInteraction)
        {
            if (this.sceneOk && this.characterOk && !this.failed)
            {
                base.Grant();
            }
        }
        public override void OnInstall()
        {
            base.OnInstall();
            this.healthCheck = new ToggleAction(new Action(this.SubscribeHealthCheck), new Action(this.UnsubscribeHealthCheck));
            this.teleporterCheck = new ToggleAction(new Action(this.SubscribeTeleporterCheck), new Action(this.UnsubscribeTeleporterCheck));
            base.localUser.onBodyChanged += this.OnBodyChanged;
        }
        public override void OnUninstall()
        {
            base.localUser.onBodyChanged -= this.OnBodyChanged;
            this.healthCheck.Dispose();
            this.teleporterCheck.Dispose();
            base.OnUninstall();
        }
        private void OnBodyChanged()
        {
            if (this.sceneOk && this.characterOk && !this.failed && base.localUser.cachedBody)
            {
                this.healthComponent = base.localUser.cachedBody.healthComponent;
                this.healthCheck.SetActive(true);
                this.teleporterCheck.SetActive(true);
            }
        }
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            this.characterOk = true;
        }
        public override void OnBodyRequirementBroken()
        {
            this.characterOk = false;
            this.Fail();
            base.OnBodyRequirementBroken();
        }
        private void Fail()
        {
            this.failed = true;
            this.healthCheck.SetActive(false);
            this.teleporterCheck.SetActive(false);
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
        private bool sceneOk;
        private bool characterOk;
        private ToggleAction healthCheck;
        private ToggleAction teleporterCheck;
    }
}
