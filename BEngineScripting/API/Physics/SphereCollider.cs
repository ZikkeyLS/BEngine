
namespace BEngine
{
	public class SphereCollider : Collider
	{
		public float Radius = 1f;
		private float _lastRadius = 0f;

		public override object[] GetAdditionalData()
		{
			return [Radius];
		}

		public override void OnRescale()
		{
			_lastRadius = Radius;
		}

		public override bool RequiresRescale()
		{
			return _lastRadius != Radius;
		}

		public override void Setup()
		{
			transform = GetScript<Transform>();
			if (transform != null)
			{
				physicsID = InternalCalls.PhysicsCreateSphere(transform.Position, transform.Rotation, Radius);
				Prepared = true;
			}
		}
	}
}
