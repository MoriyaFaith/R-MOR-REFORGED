using System;
using EntityStates;
using EntityStates.Engi.EngiMissilePainter;

namespace EntityStates.RMOR.Special
{
	public class Finish : BaseLockOnMissileState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				this.outer.SetNextState(new Idle());
			}
		}
	}
}