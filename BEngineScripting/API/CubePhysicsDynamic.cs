
namespace BEngine
{
	public struct PhysicsEntryData
	{
		public Vector3 Position;
		public Quaternion Rotation;
	}

	public class CubePhysicsDynamic : Script
	{
		private Transform _transform;

		public string physicsID = string.Empty;

		public override void OnStart()
		{
			Setup();
		}

		public override void OnFixedUpdate()
		{
			if (_transform == null || physicsID == string.Empty)
			{
				Setup();
				return;
			}

			PhysicsEntryData data = InternalCalls.PhysicsGetActorData(physicsID);
			_transform.Position = data.Position;
			_transform.Rotation = data.Rotation;
			InternalCalls.PhysicsUpdateActorScale(physicsID, _transform.Scale);
		}

		public override void OnDestroy()
		{
			InternalCalls.PhysicsRemoveActor(physicsID);
		}

		private bool Setup()
		{
			_transform = GetScript<Transform>();
			if (_transform != null)
			{
				physicsID = InternalCalls.PhysicsCreateDynamicCube(_transform.Position, _transform.Rotation, _transform.Scale);
				if (physicsID != string.Empty)
					return true;
			}

			return false;
		}
	}
}
