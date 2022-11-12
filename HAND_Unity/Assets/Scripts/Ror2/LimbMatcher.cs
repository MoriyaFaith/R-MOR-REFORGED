using UnityEngine;
using System;

namespace RoR2 {
	public class LimbMatcher : MonoBehaviour {

		// Token: 0x04002C77 RID: 11383
		public bool scaleLimbs = true;

		// Token: 0x04002C79 RID: 11385
		public LimbMatcher.LimbPair[] limbPairs;

		// Token: 0x02000796 RID: 1942
		[Serializable]
		public struct LimbPair {
			// Token: 0x04002C7A RID: 11386
			public Transform originalTransform;

			// Token: 0x04002C7B RID: 11387
			public string targetChildLimb;

			// Token: 0x04002C7C RID: 11388
			public float originalLimbLength;

			// Token: 0x04002C7D RID: 11389
			[NonSerialized]
			public Transform targetTransform;
		}
	}
}