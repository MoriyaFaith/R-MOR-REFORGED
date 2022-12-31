using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using System.Collections.Generic;
using RoR2.UI;
using System;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace RMORMod.Modules
{
    internal static class Assets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        private const string assetbundleName = "rmorassetbundle";
        private const string csProjName = "RMORMod";

        internal static GameObject lockOnTarget;

        internal static void Initialize()
        {
            LoadAssetBundle();
            SwapShadersGhetti();
            LoadSoundbank();
            PopulateAssets();
        }

        internal static void LoadAssetBundle()
        {
            try
            {
                if (mainAssetBundle == null)
                {
                    using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{csProjName}.{assetbundleName}"))
                    {
                        mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e);
                return;
            }
        }
        private static void SwapShadersGhetti()
        {
            Shader cloudRemap = Addressables.LoadAssetAsync<Shader>("RoR2/Base/Shaders/HGCloudRemap.shader").WaitForCompletion();
            Shader standard = Addressables.LoadAssetAsync<Shader>("RoR2/Base/Shaders/HGStandard.shader").WaitForCompletion();
            Shader triplanar = Addressables.LoadAssetAsync<Shader>("RoR2/Base/Shaders/HGTriplanarTerrainBlend.shader").WaitForCompletion();

            var materials = mainAssetBundle.LoadAllAssets<Material>();
            foreach (Material mat in materials)
            {
                switch (mat.shader.name)
                {
                    case "StubbedRoR2/Base/Shaders/HGCloudRemap": // name may differ
                        mat.shader = cloudRemap;
                        break;

                    case "StubbedRoR2/Base/Shaders/HGStandard": // name may differ
                        mat.shader = standard;
                        break;
                    case "StubbedRoR2/Base/Shaders/HGTriplanarTerrainBlend": // name may differ
                        mat.shader = triplanar;
                        break;
                }
            }
        }
        private static void SwapShadersFromMaterials()
        {
            var shaders = mainAssetBundle.LoadAllAssets<Shader>().Where(shader => shader.name.StartsWith("StubbedShader"));
            foreach (Shader shader in shaders)
            {
                try
                {
                    SwapShader(shader);
                }
                catch (Exception e) { Debug.LogError(e); }
            }
        }
        private static async void SwapShader(Shader shader)
        {
            var shaderName = shader.name.Substring("Stubbed".Length);
            var adressablePath = $"{shaderName}.shader";
            shader = Addressables.LoadAssetAsync<Shader>(adressablePath).WaitForCompletion();
        }

        internal static void LoadSoundbank()
        {                                                                
            //soundbank currently broke, but this is how you should load yours
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{csProjName}.HAND_Overclocked_Soundbank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }

        internal static void PopulateAssets()
        {
            if (!mainAssetBundle)
            {
                Log.Error("There is no AssetBundle to load assets from.");
                return;
            }
            lockOnTarget = CreateLockOnIndicator();
        }

        private static GameObject CreateLockOnIndicator()
        {

            GameObject indicatorPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/LightningIndicator"), "LockOnIndicator", false);

            //UnityEngine.Object.DestroyImmediate(indicatorPrefab.transform.Find("TextMeshPro").gameObject);
            //UnityEngine.Object.DestroyImmediate(indicatorPrefab.transform.Find("Holder/Brackets").gameObject);

            indicatorPrefab.transform.localScale = Vector3.one * .15f;
            indicatorPrefab.transform.localPosition = Vector3.zero;
            indicatorPrefab.transform.Find("Holder").rotation = Quaternion.identity;
            indicatorPrefab.transform.Find("Holder/Brackets").rotation = Quaternion.identity;

            SpriteRenderer spriteRenderer = indicatorPrefab.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = null;
            spriteRenderer.color = Color.green;
            spriteRenderer.size = Vector2.zero;
            spriteRenderer.transform.localRotation = Quaternion.identity;
            spriteRenderer.transform.localPosition = Vector3.zero;

            //UnityEngine.Object.Internal_InstantiateSingleWithParent(LoadAsset<GameObject>("ConquererTargetMarker"), indicatorPrefab.transform.Find("Holder/Brackets"), Vector3.zero, Quaternion.identity);

            return indicatorPrefab;
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            Modules.Content.AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (Renderer i in objectToConvert.GetComponentsInChildren<Renderer>())
            {
                if (i is ParticleSystemRenderer)
                    continue;
                i?.material?.SetHopooMaterial();
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }


        public static GameObject LoadSurvivorModel(string modelName) {
            GameObject model = mainAssetBundle.LoadAsset<GameObject>(modelName);
            if (model == null) {
                Log.Error("Trying to load a null model- check to see if the BodyName in your code matches the prefab name of the object in Unity\nFor Example, if your prefab in unity is 'mdlHenry', then your BodyName must be 'Henry'");
                return null;
            }

            return PrefabAPI.InstantiateClone(model, model.name, false);
        }

        internal static Texture LoadCharacterIconGeneric(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        internal static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            if (!newEffect)
            {
                Log.Error("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            Modules.Content.AddEffectDef(newEffectDef);
        }
    }
}