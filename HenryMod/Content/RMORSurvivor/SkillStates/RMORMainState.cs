using EntityStates;
using EntityStates.RMOR.Emotes;
using RMORMod.Modules;
using RoR2;
using UnityEngine;

namespace EntityStates.RMOR
{
    public class RMORMainState : GenericCharacterMain
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
                if (characterBody)
                {
                    foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
                    {
                        if (lu.cachedBody == characterBody)
                        {
                            localUser = lu;
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

            if (isAuthority && characterMotor.isGrounded && localUser != null)
            {
                if (!localUser.isUIFocused)
                {
                    if (Config.GetKeyPressed(Config.KeybindEmote1))
                    {
                        outer.SetInterruptState(new Sit(), InterruptPriority.Any);
                        return;
                    }
                    else if (Config.GetKeyPressed(Config.KeybindEmote2))
                    {
                        outer.SetInterruptState(new Spin(), InterruptPriority.Any);
                        return;
                    }
                    else if (Config.GetKeyPressed(Config.KeybindEmoteCSS))
                    {
                        outer.SetInterruptState(new MenuPose(), InterruptPriority.Any);
                        return;
                    }
                }
            }
        }
    }
}
