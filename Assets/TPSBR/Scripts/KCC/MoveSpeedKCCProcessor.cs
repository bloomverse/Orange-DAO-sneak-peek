using UnityEngine;
using Fusion.Addons.KCC;

namespace TPSBR
{
	public sealed class MoveSpeedKCCProcessor : KCCProcessor, IKCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float _moveSpeedMultiplier = 1.0f;

		public override float GetPriority(KCC kcc) => float.MaxValue;

		public void Execute(PrepareData stage, KCC kcc, KCCData data)
		{
			Debug.Log("Executing speed");
			data.KinematicSpeed *= _moveSpeedMultiplier;
		}
	}
}
