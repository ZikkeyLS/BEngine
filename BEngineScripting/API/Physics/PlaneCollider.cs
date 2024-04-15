
namespace BEngine
{
	public class PlaneCollider : Collider
	{
		public Vector2 Size = Vector2.one;
		private Vector2 _lastScale;

		public override object[] GetAdditionalData()
		{
			return [Size];
		}

		public override void OnRescale()
		{
			_lastScale = Size;
		}

		public override bool RequiresRescale()
		{
			return _lastScale != Size;
		}

		public override void Setup()
		{
			transform = GetScript<Transform>();
			if (transform != null)
			{
				physicsID = InternalCalls.PhysicsCreatePlane(transform.Position, transform.Rotation, Size);
				Prepared = true;
			}
		}
	}
}
