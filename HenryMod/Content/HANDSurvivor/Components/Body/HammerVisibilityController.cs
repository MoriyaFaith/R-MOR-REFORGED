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

        private bool hammerEnabled = false;

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

        /*private void FixedUpdate()
        {
            if (hammerEnabled)
            {
                ShowHammer();
            }
            else
            {
                HideHammer();
            }
        }*/

        private void ShowHammer()
        {
            if (!HasShatteringJustice(inventory))
            {
                hammer.SetActive(true);
            }
            else
            {
                hammer.SetActive(false);
            }
        }

        private void HideHammer()
        {
            if (!HasHammerPrimary(skillLocator) || HasShatteringJustice(inventory)) hammer.SetActive(false);
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
            hammerEnabled = enabled;
            if (!hammerEnabled)
            {
                HideHammer();
            }
            else
            {
                ShowHammer();
            }
        }
    }
}
