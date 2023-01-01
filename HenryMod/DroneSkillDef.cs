using RMORMod.Content.RMORSurvivor.Components.Body;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RMORMod
{
    public class DroneSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new DroneSkillDef.InstanceData
			{
				targetingController = skillSlot.GetComponent<RMORTargetingController>()
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
			public RMORTargetingController targetingController;
		}

		private static bool HasTarget([NotNull] GenericSkill skillSlot)
		{
			RMORTargetingController targeter = ((DroneSkillDef.InstanceData)skillSlot.skillInstanceData).targetingController;
			return (targeter != null) ? targeter.HasTarget() : false;
		}
	}
}
