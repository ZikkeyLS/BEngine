
namespace BEngine
{
	public class Camera : Script
	{
		public uint priority = 0;

		private Transform _transform;

		[RunInAny]
		public override void OnUpdate()
		{
			if (_transform == null)
			{
				_transform = GetScript<Transform>();
				return;
			}

			InternalCalls.CameraCreateRequest(GUID, priority, _transform.Position, _transform.Rotation);
		}
	}
}
