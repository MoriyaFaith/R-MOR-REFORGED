using System;
using EntityStates;
using EntityStates.Engi.EngiMissilePainter;

namespace EntityStates.RMOR.Special
{
	public class Startup : BaseLockOnMissileState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Startup.baseDuration / this.attackSpeedStat;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && this.duration <= base.fixedAge)
			{
				this.outer.SetNextState(new Paint());
			}
		}

		public static float baseDuration;
		private float duration;
	}
}
