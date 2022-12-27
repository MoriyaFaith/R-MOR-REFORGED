using RoR2;
using UnityEngine;
using System;
using static RoR2.CameraTargetParams;
using RMORMod.Content.HANDSurvivor.Components.Body;
using RMORMod.Modules;
using RMORMod.Content.Shared.Components.Body;
using EntityStates;

namespace EntityStates.RMOR.Emotes
{
    public class RMOREmotes : BaseState
    {
        public string soundString;
        public string animString;
        public float duration;
        public float animDuration;

        private Animator animator;
        private ChildLocator childLocator;

        public LocalUser localUser;
        public bool useHammer;
        private HammerVisibilityController hammerVisibility;

        private CharacterCameraParamsData emoteCameraParams = new CharacterCameraParamsData()
        {
            maxPitch = 70,
            minPitch = -70,
            pivotVerticalOffset = 1f,
            idealLocalCameraPos = emoteCameraPosition,
            wallCushion = 0.1f,
        };

        public static Vector3 emoteCameraPosition = new Vector3(0, 0.0f, -7.9f);

        private CameraParamsOverrideHandle camOverrideHandle;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            childLocator = GetModelChildLocator();
            FindLocalUser();

            characterBody.hideCrosshair = true;

            if (GetAimAnimator()) GetAimAnimator().enabled = false;
            animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0);
            animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0);

            if (animDuration == 0 && duration != 0) animDuration = duration;

            if (duration > 0) PlayAnimation("FullBody, Override", animString, "Emote.playbackRate", duration);
            else PlayAnimation("FullBody, Override", animString, "Emote.playbackRate", animDuration);

            CameraParamsOverrideRequest request = new CameraParamsOverrideRequest
            {
                cameraParamsData = emoteCameraParams,
                priority = 0,
            };

            camOverrideHandle = cameraTargetParams.AddParamsOverride(request, 0.5f);

            hammerVisibility = GetComponent<HammerVisibilityController>();
            if (hammerVisibility)
            {
                hammerVisibility.SetEmote(true);
                hammerVisibility.SetHammerEnabled(useHammer);
            }
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

        public override void OnExit()
        {
            if (hammerVisibility)
            {
                hammerVisibility.SetHammerEnabled(false);
                hammerVisibility.SetEmote(false);
            }

            characterBody.hideCrosshair = false;

            if (GetAimAnimator()) GetAimAnimator().enabled = true;
            animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1);
            animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1);

            base.PlayAnimation("FullBody, Override", "BufferEmpty");

            cameraTargetParams.RemoveParamsOverride(camOverrideHandle, 0.5f);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool flag = false;

            if (characterMotor)
            {
                if (!characterMotor.isGrounded) flag = true;
                if (characterMotor.velocity != Vector3.zero) flag = true;
            }

            if (inputBank)
            {
                if (inputBank.skill1.down) flag = true;
                if (inputBank.skill2.down) flag = true;
                if (inputBank.skill3.down) flag = true;
                if (inputBank.skill4.down) flag = true;
                if (inputBank.jump.down) flag = true;

                if (inputBank.moveVector != Vector3.zero) flag = true;
            }

            FindLocalUser();

            //emote cancels
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

            if (duration > 0 && fixedAge >= duration) flag = true;

            if (flag)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }

    public class MenuPose : RMOREmotes
    {
        public override void OnEnter()
        {
            animString = "MenuPose";
            animDuration = 2.3666666667f;
            useHammer = true;
            base.OnEnter();

            Util.PlaySound("Play_HOC_StartHammer", gameObject);
        }
    }

    public class Spin : RMOREmotes
    {
        private bool playedSound1 = false;
        private bool playedSound2 = false;
        private bool playedSound3 = false;
        private static string startSoundString = "Play_MULT_shift_start";
        private static string endSoundString = "Play_MULT_shift_end";
        public override void OnEnter()
        {
            animString = "Emote2";
            animDuration = 4.3f;
            useHammer = false;
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            float animPercent = fixedAge / animDuration;
            if (!playedSound1 && animPercent >= 10f / 129f)
            {
                playedSound1 = true;
                Util.PlaySound(startSoundString, gameObject);
            }
            if (!playedSound2 && animPercent >= 120f / 129f)
            {
                playedSound2 = true;
                Util.PlaySound(endSoundString, gameObject);
                Util.PlaySound("Play_mult_shift_hit", gameObject);
            }
            if (!playedSound3 && animPercent >= 136f / 129f)
            {
                playedSound3 = true;
                Util.PlaySound("Play_HOC_StartPunch", gameObject);
            }
        }

        public override void OnExit()
        {
            if (playedSound1 && !playedSound2)
            {
                Util.PlaySound(endSoundString, gameObject);
            }
            base.OnExit();
        }
    }

    public class Sit : RMOREmotes
    {
        private bool playedSound = false;
        public override void OnEnter()
        {
            animString = "Emote1";
            animDuration = 1.2666666667f;
            useHammer = false;
            base.OnEnter();
            Util.PlaySound("Play_drone_deathpt1", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!playedSound && fixedAge / animDuration >= 30f / 38f)
            {
                playedSound = true;
                Util.PlaySound("Play_drone_deathpt2", gameObject);
            }
        }
    }
}
