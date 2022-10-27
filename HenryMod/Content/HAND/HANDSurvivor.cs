using HANDMod.Modules.Survivors;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.Skills;
using HANDMod.Modules.Characters;
using HANDMod.Modules;
using System;
using HANDMod;
using System.Collections.Generic;

namespace HANDMod
{
    internal class HANDSurvivor : SurvivorBase
    {
        public const string HAND_PREFIX = HandPlugin.DEVELOPER_PREFIX + "_HAND_BODY_";
        public override string survivorTokenPrefix => HAND_PREFIX;

        public override UnlockableDef characterUnlockableDef => null;

        public override string bodyName => "Henry";

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "HenryBody",
            bodyNameToken = HandPlugin.DEVELOPER_PREFIX + "_HAND_BODY_NAME",
            subtitleNameToken = HandPlugin.DEVELOPER_PREFIX + "_HAND_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = new Color(0.556862745f, 0.682352941f, 0.690196078f),

            crosshair = LegacyResourcesAPI.Load<GameObject>("prefabs/crosshair/simpledotcrosshair"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/robocratepod"),//RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")

            maxHealth = 160f,
            healthRegen = 2.5f,
            armor = 0f,

            jumpCount = 1
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] { };

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = HandPlugin.DEVELOPER_PREFIX;
        }

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            RegisterStates();
            bodyPrefab.AddComponent<HANDMod.Components.HAND_Body.HANDNetworkComponent>();
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(HAND_PREFIX + "DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                model);
            skins.Add(defaultSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }

        private void RegisterStates()
        {

        }
    }
}
