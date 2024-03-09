
namespace BEngine
{
	public struct PhysicsEntryData
	{
		public Vector3 Position;
		public Quaternion Rotation;
	}

	public class CubePhysicsDynamic : Script
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
			PhysicsEntryData data = InternalCalls.PhysicsGetDynamicData(physicsID);
			Transform.Position = data.Position;
			Transform.Rotation = data.Rotation;
			Console.WriteLine(Transform.Position);
			// Transform.Scale = data.Scale;
		}

		public override void OnEditorDestroy()
		{
			InternalCalls.PhysicsRemoveDynamic(physicsID);
		}

		private bool Setup()
		{
			Transform = GetScript<Transform>();

			if (Transform != null)
			{
				physicsID = InternalCalls.PhysicsCreateDynamicCube(Transform.Position, Transform.Rotation, Transform.Scale);
				if (physicsID != string.Empty)
					return true;
			}

			return false;
		}
	}
}
