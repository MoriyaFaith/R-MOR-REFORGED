using HANDMod.Content.HANDSurvivor.Components.Body;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace HANDMod
{
    public class DroneSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new DroneSkillDef.InstanceData
			{
				targetingController = skillSlot.GetComponent<HANDTargetingController>()
			};
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return DroneSkillDef.HasTarget(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && DroneSkillDef.HasTarget(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public HANDTargetingController targetingController;
		}

		private static bool HasTarget([NotNull] GenericSkill skillSlot)
		{
			HANDTargetingController targeter = ((DroneSkillDef.InstanceData)skillSlot.skillInstanceData).targetingController;
			return (targeter != null) ? targeter.HasTarget() : false;
		}
	}
}
