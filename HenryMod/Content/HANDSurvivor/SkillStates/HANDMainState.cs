using EntityStates.HAND_Overclocked.Emotes;
using RMORMod.Modules;
using RoR2;
using UnityEngine;

namespace EntityStates.HAND_Overclocked
{
    public class HANDMainState : GenericCharacterMain
    {
        public LocalUser localUser;

        public override void OnEnter()
        {
            base.OnEnter();
            FindLocalUser();
        }

        private void FindLocalUser()
        {
            if (localUser == null)
            {
                if (base.characterBody)
                {
                    foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
                    {
                        if (lu.cachedBody == base.characterBody)
                        {
                            this.localUser = lu;
                            break;
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            FindLocalUser();

            if (base.isAuthority && base.characterMotor.isGrounded && localUser != null)
            {
                if (!this.localUser.isUIFocused)
                {
                    if (Config.GetKeyPressed(Config.KeybindEmote1))
                    {
                        this.outer.SetInterruptState(new Sit(), InterruptPriority.Any);
                        return;
                    }
                    else if (Config.GetKeyPressed(Config.KeybindEmote2))
                    {
                        this.outer.SetInterruptState(new Spin(), InterruptPriority.Any);
                        return;
                    }
                    else if (Config.GetKeyPressed(Config.KeybindEmoteCSS))
                    {
                        this.outer.SetInterruptState(new MenuPose(), InterruptPriority.Any);
                        return;
                    }
                }
            }
        }
    }
}
