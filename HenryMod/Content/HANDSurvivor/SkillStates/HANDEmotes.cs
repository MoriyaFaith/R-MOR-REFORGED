using RoR2;
using UnityEngine;
using System;
using static RoR2.CameraTargetParams;
using HANDMod.Content.HANDSurvivor.Components.Body;
using HANDMod.Modules;
using HANDMod.Content.Shared.Components.Body;

namespace EntityStates.HAND_Overclocked.Emotes
{
    public class HANDEmotes : BaseState
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
            this.animator = base.GetModelAnimator();
            this.childLocator = base.GetModelChildLocator();
            FindLocalUser();

            base.characterBody.hideCrosshair = true;

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = false;
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0);
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0);

            if (this.animDuration == 0 && this.duration != 0) this.animDuration = this.duration;

            if (this.duration > 0) base.PlayAnimation("FullBody, Override", this.animString, "Emote.playbackRate", this.duration);
            else base.PlayAnimation("FullBody, Override", this.animString, "Emote.playbackRate", this.animDuration);

            CameraParamsOverrideRequest request = new CameraParamsOverrideRequest
            {
                cameraParamsData = emoteCameraParams,
                priority = 0,
            };

            camOverrideHandle = base.cameraTargetParams.AddParamsOverride(request, 0.5f);

            hammerVisibility = base.GetComponent<HammerVisibilityController>();
            if (hammerVisibility)
            {
                hammerVisibility.SetEmote(true);
                hammerVisibility.SetHammerEnabled(this.useHammer);
            }
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

        public override void OnExit()
        {
            if (hammerVisibility)
            {
                hammerVisibility.SetHammerEnabled(false);
                hammerVisibility.SetEmote(false);
            }

            base.characterBody.hideCrosshair = false;

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = true;
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1);
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1);

            base.PlayAnimation("FullBody, Override", "BufferEmpty");

            base.cameraTargetParams.RemoveParamsOverride(camOverrideHandle, 0.5f);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool flag = false;

            if (base.characterMotor)
            {
                if (!base.characterMotor.isGrounded) flag = true;
                if (base.characterMotor.velocity != Vector3.zero) flag = true;
            }

            if (base.inputBank)
            {
                if (base.inputBank.skill1.down) flag = true;
                if (base.inputBank.skill2.down) flag = true;
                if (base.inputBank.skill3.down) flag = true;
                if (base.inputBank.skill4.down) flag = true;
                if (base.inputBank.jump.down) flag = true;

                if (base.inputBank.moveVector != Vector3.zero) flag = true;
            }

            FindLocalUser();

            //emote cancels
            if (base.isAuthority && base.characterMotor.isGrounded && this.localUser != null)
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

            if (this.duration > 0 && base.fixedAge >= this.duration) flag = true;

            if (flag)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }

    public class MenuPose : HANDEmotes
    {
        public override void OnEnter()
        {
            this.animString = "MenuPose";
            this.animDuration = 2.3666666667f;
            this.useHammer = true;
            base.OnEnter();

            Util.PlaySound("Play_HOC_StartHammer", base.gameObject);
        }
    }

    public class Spin : HANDEmotes
    {
        private bool playedSound1 = false;
        private bool playedSound2 = false;
        private bool playedSound3 = false;
        private static string startSoundString = "Play_MULT_shift_start";
        private static string endSoundString = "Play_MULT_shift_end";
        public override void OnEnter()
        {
            this.animString = "Emote2";
            this.animDuration = 4.3f;
            this.useHammer = false;
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            float animPercent = base.fixedAge / this.animDuration;
            if (!playedSound1 && animPercent >= (10f/129f))
            {
                playedSound1 = true;
                Util.PlaySound(startSoundString, base.gameObject);
            }
            if (!playedSound2 && animPercent >= (120f/129f))
            {
                playedSound2 = true;
                Util.PlaySound(endSoundString, base.gameObject);
                Util.PlaySound("Play_mult_shift_hit", base.gameObject);
            }
            if (!playedSound3 && animPercent >= 136f/129f)
            {
                playedSound3 = true;
                Util.PlaySound("Play_HOC_StartPunch", base.gameObject);
            }
        }

        public override void OnExit()
        {
            if (playedSound1 && !playedSound2)
            {
                Util.PlaySound(endSoundString, base.gameObject);
            }
            base.OnExit();
        }
    }

    public class Sit : HANDEmotes
    {
        private bool playedSound = false;
        public override void OnEnter()
        {
            this.animString = "Emote1";
            this.animDuration = 1.2666666667f;
            this.useHammer = false;
            base.OnEnter();
            Util.PlaySound("Play_drone_deathpt1", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!playedSound && base.fixedAge/this.animDuration >= (30f/38f))
            {
                playedSound = true;
                Util.PlaySound("Play_drone_deathpt2", base.gameObject);
            }
        }
    }
}
