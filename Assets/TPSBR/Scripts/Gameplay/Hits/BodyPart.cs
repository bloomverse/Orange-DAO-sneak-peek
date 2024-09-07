using Fusion;
using UnityEngine;

namespace TPSBR
{
	public class BodyPart : Hitbox
	{
		public float  DamageMultiplier => _damageMultiplier;
		public bool   IsCritical       => _isCritical;
		//public EHitBodyPart HitBodyPart => _hitBodyPart;

		[SerializeField]
		private float _damageMultiplier = 1f;
		[SerializeField]
		private bool  _isCritical;
		//[SerializeField]
		//private EHitBodyPart _hitBodyPart;
	}
}