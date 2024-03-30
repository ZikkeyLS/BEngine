
namespace BEngine
{
	public class CubeCollider : Collider
	{
		public override ColliderType GetColliderType()
		{
			return ColliderType.Cube;
		}

		public override void Setup()
		{
			transform = GetScript<Transform>();
			if (transform != null)
			{
				physicsID = InternalCalls.PhysicsCreateCube(transform.Position, transform.Rotation, transform.Scale);
				Prepared = true;
			}
		}
	}
}
