
namespace BEngine
{
	public class CubePhysicsStatic : Script
	{
		public Transform Transform { get; set; }

		public string physicsID;

		public override void OnEditorStart()
		{
			Setup();
		}

		public override void OnEditorUpdate()
		{
			if (Transform == null)
				Setup();

			if (Transform == null || physicsID == string.Empty)
				return;

			// get physics data
			PhysicsBodyData data = InternalCalls.PhysicsGetBodyData(physicsID);
			Transform.Position = data.Position;
			Transform.Rotation = data.Rotation;
			// Transform.Scale = data.Scale;
		}

		private bool Setup()
		{
			Transform = GetScript<Transform>();

			if (Transform != null)
			{
				physicsID = InternalCalls.PhysicsCreateCube(Transform.Position, Transform.Rotation, Transform.Scale);
				if (physicsID != string.Empty)
					return true;
			}

			return false;
		}
	}
}
