using RoR2;
using UnityEngine;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    //Doesn't work, hammer transform seems to ignore modifications to localScale.
    public class HammerVisibilityController : MonoBehaviour
    {
        private ChildLocator childLocator;
        private SkillLocator skillLocator;
        private Transform hammerTransform;

        private bool hammerEnabled = false;

        private void Awake()
        {
            childLocator = base.GetComponentInChildren<ChildLocator>();
            if (!childLocator)
            {
                Destroy(this);
                return;
            }
            skillLocator = base.GetComponent<SkillLocator>();
            hammerTransform = childLocator.FindChild("HanDHammer");

            HideHammer();
        }

        private void FixedUpdate()
        {
            if (hammerEnabled)
            {
                ShowHammer();
            }
            else
            {
                HideHammer();
            }
        }

        private void ShowHammer()
        {
            hammerTransform.localScale = Vector3.one;
        }

        private void HideHammer()
        {
            hammerTransform.localScale = Vector3.zero;
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
