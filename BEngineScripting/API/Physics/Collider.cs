
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

		private PhysicsEntryData? _lastPhysicsEntryData;
		private Vector3? _lastScale;

		protected Transform transform;
		protected ColliderControlledBy colliderControlledBy = ColliderControlledBy.Collider;
		protected string physicsID = string.Empty;

		[EditorIgnore] public bool Prepared { get; protected set; } = false;

		[EditorIgnore] public string PhysicsID => physicsID;
		[EditorIgnore] public PhysicsEntryData? LastPhysicsEntryData => _lastPhysicsEntryData;

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

			if (_lastPhysicsEntryData?.Position != transform.Position || _lastPhysicsEntryData?.Rotation != transform.Rotation)
			{
				InternalCalls.PhysicsApplyTransform(physicsID, transform.Position, transform.Rotation);
			}

			PhysicsEntryData data = InternalCalls.PhysicsGetActorData(physicsID);

			if (RequiresRescale())
			{
				InternalCalls.PhysicsUpdateActorSize(physicsID, [transform.Scale, .. GetAdditionalData()]);
				OnRescale();
			}

			if (GetScript<Rigidbody>() != null)
			{
				transform.Position = data.Position;
				transform.Rotation = data.Rotation;
			}

			_lastPhysicsEntryData = data;
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
