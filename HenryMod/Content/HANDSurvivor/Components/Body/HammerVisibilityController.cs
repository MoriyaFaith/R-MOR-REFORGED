using RoR2;
using UnityEngine;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    //Doesn't work, hammer transform seems to ignore modifications to localScale.
    public class HammerVisibilityController : MonoBehaviour
    {
        private Animator animator;
        private ChildLocator childLocator;
        private SkillLocator skillLocator;
        private CharacterBody characterBody;
        private Inventory inventory;
        private GameObject hammer;
        private CharacterModel characterModel;

        private bool usingHammer = false;
        private bool inEmote = false;

        private static bool initialized = false;
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            On.RoR2.CharacterModel.UpdateItemDisplay += CharacterModel_UpdateItemDisplay;
        }

        private static void CharacterModel_UpdateItemDisplay(On.RoR2.CharacterModel.orig_UpdateItemDisplay orig, CharacterModel self, Inventory inventory)
        {
            orig(self, inventory);
            if (self.body)
            {
                HammerVisibilityController hvc = self.body.GetComponent<HammerVisibilityController>();
                if (hvc)
                {
                    hvc.UpdateHammer();
                }
            }
        }

        //This method ends up running before itemdisplays get updated, so the Shattering Justice hiding doesnt work.
        /*private void OnEnable()
        {
            if (characterBody)
            {
                characterBody.onInventoryChanged += CharacterBody_onInventoryChanged;
            }
        }

        private void OnDisable()
        {
            if (characterBody)
            {
                characterBody.onInventoryChanged -= CharacterBody_onInventoryChanged;
            }
        }

        private void CharacterBody_onInventoryChanged()
        {
            UpdateHammer();
        }*/

        private void DisableShatteringJustice()
        {
            if (HasShatteringJustice(inventory) && characterModel)
            {
                characterModel.DisableItemDisplay(RoR2Content.Items.ArmorReductionOnHit.itemIndex);
            }
        }

        private void EnableShatteringJustice()
        {
            if (HasShatteringJustice(inventory) && characterModel)
            {
                characterModel.EnableItemDisplay(RoR2Content.Items.ArmorReductionOnHit.itemIndex);
            }
        }

        private void Awake()
        {
            childLocator = base.GetComponentInChildren<ChildLocator>();
            if (!childLocator)
            {
                Destroy(this);
                return;
            }
            characterBody = base.GetComponent<CharacterBody>();
            skillLocator = base.GetComponent<SkillLocator>();
            hammer = childLocator.FindChildGameObject("HanDHammer");
            ModelLocator ml = base.GetComponent<ModelLocator>();
            if (ml && ml.modelTransform)
            {
                animator = ml.modelTransform.GetComponent<Animator>();
            }
        }

        private void Start()
        {
            if (characterBody)
            {
                inventory = characterBody.inventory;
                if (characterBody.modelLocator && characterBody.modelLocator.modelTransform && characterBody.modelLocator.modelTransform.gameObject)
                {
                    characterModel = characterBody.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
                }
            }
            if (HasHammerPrimary(skillLocator))
            {
                SetHammerEnabled(true);
                if (animator)
                {
                    animator.SetFloat("hammerIdle", 1f);
                    animator.SetLayerWeight(1, 1);
                }
            }
            else
            {
                SetHammerEnabled(false);
            }
        }

        private void ShowHammer()
        {
            usingHammer = true;
            if (!HasShatteringJustice(inventory))
            {
                hammer.SetActive(true);
            }
            else
            {
                hammer.SetActive(false);
                EnableShatteringJustice();
            }
        }

        private void HideHammer()
        {
            if (inEmote || !HasHammerPrimary(skillLocator))
            {
                usingHammer = false;
                hammer.SetActive(false);
                DisableShatteringJustice();
            }
            else
            {
                ShowHammer();
            }
        }

        public static bool HasHammerPrimary(SkillLocator sk)
        {
            return sk && sk.primary && sk.primary.skillDef == SkillDefs.PrimaryHammer;
        }

        public static bool HasShatteringJustice(Inventory inv)
        {
            return (inv && inv.GetItemCount(RoR2Content.Items.ArmorReductionOnHit) > 0);
        }

        public void SetHammerEnabled(bool enabled)
        {
            if (enabled)
            {
                ShowHammer();
            }
            else
            {
                HideHammer();
            }
        }

        public void SetEmote(bool inEmote)
        {
            this.inEmote = inEmote;
            UpdateHammer();
        }

        public void UpdateHammer()
        {
            if (usingHammer)
            {
                ShowHammer();
            }
            else
            {
                HideHammer();
            }
        }
    }
}
