using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System;

namespace RMORMod.Content.RMOR.Achievements
{
    [RegisterAchievement("MoriyaRMORSkewerUnlock", "Skills.RMOR.Skewer", null, null)]
    public class SkewerAchievement : BaseAchievement
    {
        // Token: 0x06005710 RID: 22288 RVA: 0x00160BDC File Offset: 0x0015EDDC
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RMORBody");
        }

        // Token: 0x06005711 RID: 22289 RVA: 0x00160D85 File Offset: 0x0015EF85
        private void SubscribeHealthCheck()
        {
            RoR2Application.onFixedUpdate += this.CheckHealth;
        }

        // Token: 0x06005712 RID: 22290 RVA: 0x00160D98 File Offset: 0x0015EF98
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
            if (!this.healthComponent || 0.3f < this.healthComponent.combinedHealthFraction)
            {
                Fail();
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
