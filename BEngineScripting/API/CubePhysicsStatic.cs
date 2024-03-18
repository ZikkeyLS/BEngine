
namespace BEngine
{
	public class CubePhysicsStatic : Script
	{
		public Transform Transform { get; set; }

		public string physicsID = string.Empty;

		public override void OnStart()
		{
			Setup();
		}

		public override void OnFixedUpdate()
		{
			if (Transform == null || physicsID == string.Empty)
			{
				Setup();
				return;
			}

			PhysicsEntryData data = InternalCalls.PhysicsGetActorData(physicsID);
			Transform.Position = data.Position;
			Transform.Rotation = data.Rotation;
			InternalCalls.PhysicsUpdateActorScale(physicsID, Transform.Scale);
		}

		public override void OnDestroy()
		{
			InternalCalls.PhysicsRemoveActor(physicsID);
		}

		private bool Setup()
		{
			Transform = GetScript<Transform>();
			if (Transform != null)
			{
				physicsID = InternalCalls.PhysicsCreateStaticCube(Transform.Position, Transform.Rotation, Transform.Scale);
				if (physicsID != string.Empty)
					return true;
			}

			return false;
		}
	}
}
