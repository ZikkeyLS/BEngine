
namespace BEngine
{
	public struct PhysicsEntryData
	{
		public Vector3 Position;
		public Vector3 Rotation;
	}

	public enum ForceMode : int
	{
		Force = 0,
		Impulse = 1,
		VelocityChange = 2,
		Acceleration = 3,
	}

	public class Rigidbody : Script
	{
		public bool Kinematic
		{
			get
			{
				return internal_kinematic;
			}
			set
			{
				if (_collider != null)
					InternalCalls.PhysicsChangeKinematic(_collider.PhysicsID, value);
				internal_kinematic = value;
			}
		}

		[EditorIgnore]
		public Vector3 Velocity
		{
			get
			{
				if (_collider != null)
					return InternalCalls.PhysicsGetVelocity(_collider.PhysicsID);
				else
					return Vector3.zero;
			}
			set
			{
				if (_collider != null)
					InternalCalls.PhysicsSetVelocity(_collider.PhysicsID, value);
			}
		}

		[EditorIgnore]
		public Vector3 AngularVelocity
		{
			get
			{
				if (_collider != null)
					return InternalCalls.PhysicsGetAngularVelocity(_collider.PhysicsID);
				else
					return Vector3.zero;
			}
			set
			{
				if (_collider != null)
					InternalCalls.PhysicsSetAngularVelocity(_collider.PhysicsID, value);
			}
		}

		public Vector3Bool LockLinear
		{
			get
			{
				return internal_lock_linear;
			}
			set
			{
				internal_lock_linear = value;
				if (_collider != null)
					InternalCalls.PhysicsApplyLock(_collider.PhysicsID, internal_lock_linear, internal_lock_angular);
			}
		}

		public Vector3Bool LockAngular
		{
			get
			{
				return internal_lock_angular;
			}
			set
			{
				internal_lock_angular = value;
				if (_collider != null)
					InternalCalls.PhysicsApplyLock(_collider.PhysicsID, internal_lock_linear, internal_lock_angular);
			}
		}

		private Collider _collider;

		// temp solution just to serialize properly
		[EditorIgnore] public bool internal_kinematic;
		[EditorIgnore] public Vector3Bool internal_lock_linear;
		[EditorIgnore] public Vector3Bool internal_lock_angular;

		public override void OnStart()
		{
			Setup();
		}

		public override void OnFixedUpdate()
		{
			if (_collider == null)
			{
				Setup();
				return;
			}
		}

		private void Setup()
		{
			_collider = GetScript<Collider>();
			if (_collider == null || _collider.Prepared == false)
			{
				return;
			}

			InternalCalls.PhysicsChangeDynamic(_collider.PhysicsID, true);
			InternalCalls.PhysicsChangeKinematic(_collider.PhysicsID, internal_kinematic);
		}

		public override void OnDestroy()
		{
			if (_collider != null)
				InternalCalls.PhysicsChangeDynamic(_collider.PhysicsID, false);
		}

		public void AddForce(Vector3 force, ForceMode mode)
		{
			if (_collider != null && _collider.PhysicsID != string.Empty)
				InternalCalls.PhysicsAddForce(_collider.PhysicsID, force, mode);
		}

		public void AddTorque(Vector3 torque, ForceMode mode)
		{
			if (_collider != null && _collider.PhysicsID != string.Empty)
				InternalCalls.PhysicsAddTorque(_collider.PhysicsID, torque, mode);
		}
	}
}
