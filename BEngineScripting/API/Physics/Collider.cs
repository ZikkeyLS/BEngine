
namespace BEngine
{
	public abstract class Collider : Script
	{
		public enum ColliderType : byte
		{
			Cube = 0,
			Sphere,
			Capsule,
			Plane
		}

		public enum ColliderControlledBy : byte
		{
			Collider = 0,
			Rigidbody
		}

		private Vector3? _lastPosition;
		private Quaternion? _lastRotation;
		private Vector3? _lastScale;

		protected Transform transform;
		protected ColliderControlledBy colliderControlledBy = ColliderControlledBy.Collider;
		protected string physicsID = string.Empty;

		[EditorIgnore] public bool Prepared { get; protected set; } = false;

		[EditorIgnore] public string PhysicsID => physicsID;

		public override void OnStart()
		{
			Setup();
		}

		public override void OnFixedUpdate()
		{
			if (transform == null || physicsID == string.Empty)
			{
				Setup();
				return;
			}

			if (_lastPosition != transform.Position || _lastRotation != transform.Rotation)
			{
				InternalCalls.PhysicsApplyTransform(physicsID, transform.Position, transform.Rotation);
			}

			PhysicsEntryData data = InternalCalls.PhysicsGetActorData(physicsID);
			transform.Position = data.Position;
			transform.Rotation = data.Rotation;

			if (RequiresRescale())
			{
				InternalCalls.PhysicsUpdateActorSize(physicsID, [transform.Scale, .. GetAdditionalData()]);
				OnRescale();
			}

			_lastPosition = data.Position;
			_lastRotation = data.Rotation;
		}

		public override void OnDestroy()
		{
			InternalCalls.PhysicsRemoveActor(physicsID);
		}

		public abstract object[] GetAdditionalData();

		public abstract bool RequiresRescale();

		public abstract void OnRescale();

		public abstract void Setup();
	}
}
