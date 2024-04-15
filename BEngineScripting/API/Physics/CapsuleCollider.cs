
namespace BEngine
{
	public class CapsuleCollider : Collider
	{
		public float HalfHeight = 1;
		public float Radius = 1;

		private float _lastHalfHeight = 0;
		private float _lastRadius = 0;

		public override object[] GetAdditionalData()
		{
			return [HalfHeight, Radius];
		}

		public override void OnRescale()
		{
			_lastHalfHeight = HalfHeight;
			_lastRadius = Radius;
		}

		public override bool RequiresRescale()
		{
			return _lastHalfHeight != HalfHeight || _lastRadius != Radius;
		}

		public override void Setup()
		{
			transform = GetScript<Transform>();
			if (transform != null)
			{
				physicsID = InternalCalls.PhysicsCreateCapsule(transform.Position, transform.Rotation, HalfHeight, Radius);
				Prepared = true;
			}
		}
	}
}
