
namespace BEngine
{
	public class Camera : Script
	{
		public uint priority = 0;

		private Transform _transform;

		public override void OnUpdate()
		{
			if (Project.IsEditor)
				return;

			if (_transform == null)
			{
				_transform = GetScript<Transform>();
				return;
			}

			InternalCalls.CameraCreateRequest(GUID, priority, _transform.Position, _transform.Rotation);
		}


		public override void OnEditorUpdate()
		{
			if (Project.IsEditor == false)
				return;

			if (_transform == null)
			{
				_transform = GetScript<Transform>();
				return;
			}

			InternalCalls.CameraCreateRequest(GUID, priority, _transform.Position, _transform.Rotation);
		}
	}
}
