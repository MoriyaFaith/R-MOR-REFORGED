using System;
using EntityStates;

namespace EntityStates.RMOR.Special
{
	// Token: 0x020003B2 RID: 946
	public class BaseLockOnMissileState : BaseSkillState
	{
		// Token: 0x060010F1 RID: 4337 RVA: 0x00014F2E File Offset: 0x0001312E
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}
	}
}
