using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.CollisionDetection.CollisionTasks;
using BepuPhysics.Constraints;
using BepuPhysics.Trees;
using BepuUtilities;
using BepuUtilities.Memory;
using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Vulkan;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BEngineCore
{
	/// <summary>
	/// Bit masks which control whether different members of a group of objects can collide with each other.
	/// </summary>
	public struct SubgroupCollisionFilter
	{
		/// <summary>
		/// A mask of 16 bits, each set bit representing a collision group that an object belongs to.
		/// </summary>
		public ushort SubgroupMembership;
		/// <summary>
		/// A mask of 16 bits, each set bit representing a collision group that an object can interact with.
		/// </summary>
		public ushort CollidableSubgroups;

		/// <summary>
		/// Initializes a collision filter that belongs to one specific subgroup and can collide with any other subgroup.
		/// </summary>
		/// <param name="groupId">Id of the group that this filter operates within.</param>
		/// <param name="groupId">Id of the subgroup to put this collidable into.</param>
		public SubgroupCollisionFilter(int groupId)
		{
			Debug.Assert(groupId >= 0 && groupId < 16, "The subgroup field is a ushort; it can only hold 16 distinct subgroups.");
			SubgroupMembership = (ushort)(1 << groupId);
			CollidableSubgroups = ushort.MaxValue;
		}

		/// <summary>
		/// Disables a collision between this filter and the specified subgroup.
		/// </summary>
		/// <param name="groupId">Subgroup id to disable collision with.</param>
		public void DisableCollision(int groupId)
		{
			Debug.Assert(groupId >= 0 && groupId < 16, "The subgroup field is a ushort; it can only hold 16 distinct subgroups.");
			CollidableSubgroups ^= (ushort)(1 << groupId);
		}

		/// <summary>
		/// Modifies the interactable subgroups such that filterB does not interact with the subgroups defined by filter a and vice versa.
		/// </summary>
		/// <param name="a">Filter from which to remove collisions with filter b's subgroups.</param>
		/// <param name="b">Filter from which to remove collisions with filter a's subgroups.</param>
		public static void DisableCollision(ref SubgroupCollisionFilter filterA, ref SubgroupCollisionFilter filterB)
		{
			filterA.CollidableSubgroups &= (ushort)~filterB.SubgroupMembership;
			filterB.CollidableSubgroups &= (ushort)~filterA.SubgroupMembership;
		}

		/// <summary>
		/// Checks if the filters can collide by checking if b's membership can be collided by a's collidable groups.
		/// </summary>
		/// <param name="a">First filter to test.</param>
		/// <param name="b">Second filter to test.</param>
		/// <returns>True if the filters can collide, false otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AllowCollision(in SubgroupCollisionFilter a, in SubgroupCollisionFilter b)
		{
			return (a.CollidableSubgroups & b.SubgroupMembership) > 0;
		}

	}

	public struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
	{
		public CollidableProperty<SubgroupCollisionFilter> CollisionFilters;
		public PairMaterialProperties Material;

		public NarrowPhaseCallbacks(CollidableProperty<SubgroupCollisionFilter> filters)
		{
			CollisionFilters = filters;
			Material = new PairMaterialProperties(1, 2, new SpringSettings(30, 1));
		}
		public NarrowPhaseCallbacks(CollidableProperty<SubgroupCollisionFilter> filters, PairMaterialProperties material)
		{
			CollisionFilters = filters;
			Material = material;
		}

		/// <summary>
		/// Performs any required initialization logic after the Simulation instance has been constructed.
		/// </summary>
		/// <param name="simulation">Simulation that owns these callbacks.</param>
		public void Initialize(Simulation simulation)
		{
			CollisionFilters.Initialize(simulation);
		}

		/// <summary>
		/// Chooses whether to allow contact generation to proceed for two overlapping collidables.
		/// </summary>
		/// <param name="workerIndex">Index of the worker that identified the overlap.</param>
		/// <param name="a">Reference to the first collidable in the pair.</param>
		/// <param name="b">Reference to the second collidable in the pair.</param>
		/// <param name="speculativeMargin">Reference to the speculative margin used by the pair.
		/// The value was already initialized by the narrowphase by examining the speculative margins of the involved collidables, but it can be modified.</param>
		/// <returns>True if collision detection should proceed, false otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
		{
			if (b.Mobility != CollidableMobility.Static)
			{
				return SubgroupCollisionFilter.AllowCollision(CollisionFilters[a], CollisionFilters[b]);
			}
			return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
		}

		/// <summary>
		/// Chooses whether to allow contact generation to proceed for the children of two overlapping collidables in a compound-including pair.
		/// </summary>
		/// <param name="workerIndex">Index of the worker thread processing this pair.</param>
		/// <param name="pair">Parent pair of the two child collidables.</param>
		/// <param name="childIndexA">Index of the child of collidable A in the pair. If collidable A is not compound, then this is always 0.</param>
		/// <param name="childIndexB">Index of the child of collidable B in the pair. If collidable B is not compound, then this is always 0.</param>
		/// <returns>True if collision detection should proceed, false otherwise.</returns>
		/// <remarks>This is called for each sub-overlap in a collidable pair involving compound collidables. If neither collidable in a pair is compound, this will not be called.
		/// For compound-including pairs, if the earlier call to AllowContactGeneration returns false for owning pair, this will not be called. Note that it is possible
		/// for this function to be called twice for the same subpair if the pair has continuous collision detection enabled; 
		/// the CCD sweep test that runs before the contact generation test also asks before performing child pair tests.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
		{
			return true;
		}

		/// <summary>
		/// Provides a notification that a manifold has been created for a pair. Offers an opportunity to change the manifold's details. 
		/// </summary>
		/// <param name="workerIndex">Index of the worker thread that created this manifold.</param>
		/// <param name="pair">Pair of collidables that the manifold was detected between.</param>
		/// <param name="manifold">Set of contacts detected between the collidables.</param>
		/// <param name="pairMaterial">Material properties of the manifold.</param>
		/// <returns>True if a constraint should be created for the manifold, false otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
		{
			pairMaterial = Material;
			return true;
		}

		/// <summary>
		/// Provides a notification that a manifold has been created between the children of two collidables in a compound-including pair.
		/// Offers an opportunity to change the manifold's details. 
		/// </summary>
		/// <param name="workerIndex">Index of the worker thread that created this manifold.</param>
		/// <param name="pair">Pair of collidables that the manifold was detected between.</param>
		/// <param name="childIndexA">Index of the child of collidable A in the pair. If collidable A is not compound, then this is always 0.</param>
		/// <param name="childIndexB">Index of the child of collidable B in the pair. If collidable B is not compound, then this is always 0.</param>
		/// <param name="manifold">Set of contacts detected between the collidables.</param>
		/// <returns>True if this manifold should be considered for constraint generation, false otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
		{
			return true;
		}

		/// <summary>
		/// Releases any resources held by the callbacks. Called by the owning narrow phase when it is being disposed.
		/// </summary>
		public void Dispose()
		{
			CollisionFilters.Dispose();
		}
	}

	//Note that the engine does not require any particular form of gravity- it, like all the contact callbacks, is managed by a callback.
	public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
	{
		/// <summary>
		/// Performs any required initialization logic after the Simulation instance has been constructed.
		/// </summary>
		/// <param name="simulation">Simulation that owns these callbacks.</param>
		public void Initialize(Simulation simulation)
		{
			//In this demo, we don't need to initialize anything.
			//If you had a simulation with per body gravity stored in a CollidableProperty<T> or something similar, having the simulation provided in a callback can be helpful.
		}

		/// <summary>
		/// Gets how the pose integrator should handle angular velocity integration.
		/// </summary>
		public readonly AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

		/// <summary>
		/// Gets whether the integrator should use substepping for unconstrained bodies when using a substepping solver.
		/// If true, unconstrained bodies will be integrated with the same number of substeps as the constrained bodies in the solver.
		/// If false, unconstrained bodies use a single step of length equal to the dt provided to Simulation.Timestep. 
		/// </summary>
		public readonly bool AllowSubstepsForUnconstrainedBodies => false;

		/// <summary>
		/// Gets whether the velocity integration callback should be called for kinematic bodies.
		/// If true, IntegrateVelocity will be called for bundles including kinematic bodies.
		/// If false, kinematic bodies will just continue using whatever velocity they have set.
		/// Most use cases should set this to false.
		/// </summary>
		public readonly bool IntegrateVelocityForKinematics => false;

		public Vector3 Gravity;

		public PoseIntegratorCallbacks(Vector3 gravity) : this()
		{
			Gravity = gravity;
		}

		//Note that velocity integration uses "wide" types. These are array-of-struct-of-arrays types that use SIMD accelerated types underneath.
		//Rather than handling a single body at a time, the callback handles up to Vector<float>.Count bodies simultaneously.
		Vector3Wide gravityWideDt;

		/// <summary>
		/// Callback invoked ahead of dispatches that may call into <see cref="IntegrateVelocity"/>.
		/// It may be called more than once with different values over a frame. For example, when performing bounding box prediction, velocity is integrated with a full frame time step duration.
		/// During substepped solves, integration is split into substepCount steps, each with fullFrameDuration / substepCount duration.
		/// The final integration pass for unconstrained bodies may be either fullFrameDuration or fullFrameDuration / substepCount, depending on the value of AllowSubstepsForUnconstrainedBodies. 
		/// </summary>
		/// <param name="dt">Current integration time step duration.</param>
		/// <remarks>This is typically used for precomputing anything expensive that will be used across velocity integration.</remarks>
		public void PrepareForIntegration(float dt)
		{
			//No reason to recalculate gravity * dt for every body; just cache it ahead of time.
			gravityWideDt = Vector3Wide.Broadcast(Gravity * dt);
		}

		/// <summary>
		/// Callback for a bundle of bodies being integrated.
		/// </summary>
		/// <param name="bodyIndices">Indices of the bodies being integrated in this bundle.</param>
		/// <param name="position">Current body positions.</param>
		/// <param name="orientation">Current body orientations.</param>
		/// <param name="localInertia">Body's current local inertia.</param>
		/// <param name="integrationMask">Mask indicating which lanes are active in the bundle. Active lanes will contain 0xFFFFFFFF, inactive lanes will contain 0.</param>
		/// <param name="workerIndex">Index of the worker thread processing this bundle.</param>
		/// <param name="dt">Durations to integrate the velocity over. Can vary over lanes.</param>
		/// <param name="velocity">Velocity of bodies in the bundle. Any changes to lanes which are not active by the integrationMask will be discarded.</param>
		public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation, BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
		{
			//This also is a handy spot to implement things like position dependent gravity or per-body damping.
			//We don't have to check for kinematics; IntegrateVelocityForKinematics returns false in this type, so we'll never see them in this callback.
			//Note that these are SIMD operations and "Wide" types. There are Vector<float>.Count lanes of execution being evaluated simultaneously.
			//The types are laid out in array-of-structures-of-arrays (AOSOA) format. That's because this function is frequently called from vectorized contexts within the solver.
			//Transforming to "array of structures" (AOS) format for the callback and then back to AOSOA would involve a lot of overhead, so instead the callback works on the AOSOA representation directly.
			velocity.Linear += gravityWideDt;
		}
	}

	struct RayHit
	{
		public float T;
		public CollidableReference HitCollider;
	}

	class RayHitHandler : IRayHitHandler
	{
		public List<RayHit> hits = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AllowTest(CollidableReference collidable)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AllowTest(CollidableReference collidable, int childIndex)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnRayHit(in RayData ray, ref float maximumT, float t, in Vector3 normal, CollidableReference collidable, int childIndex)
		{
			//We are only interested in the earliest hit. This callback is executing within the traversal, so modifying maximumT informs the traversal
			//that it can skip any AABBs which are more distant than the new maximumT.

			maximumT = t;
			//Cache the earliest impact.
			hits.Add(new RayHit() { T = t, HitCollider = collidable });
		}
	}

	public class PhysicsEntity
	{
		public int AttachedLayer;
		public bool IsBody;
		public BodyHandle? Body;
		public StaticHandle? Static;

		public PhysicsEntity(BodyHandle bodyHandle, int attachedLayer = 0)
		{
			AttachedLayer = attachedLayer;
			IsBody = true;
			Body = bodyHandle;
		}

		public PhysicsEntity(StaticHandle staticHandle, int attachedLayer = 0)
		{
			AttachedLayer = attachedLayer;
			IsBody = false;
			Static = staticHandle;
		}
	}

	public class Physics
	{
		private Simulation _simulation;
		private BufferPool _bufferPool;
		private ThreadDispatcher _dispatcher;
		private CollidableProperty<SubgroupCollisionFilter> _collisionFilter;

		private bool running = true;

		public Dictionary<string, BodyHandle> Bodies = new();

		public Physics()
		{

		}

		public void Initialize()
		{
			_collisionFilter = new CollidableProperty<SubgroupCollisionFilter>();
			_bufferPool = new BufferPool();
			_simulation = Simulation.Create(_bufferPool, new NarrowPhaseCallbacks(_collisionFilter),
				new PoseIntegratorCallbacks(new Vector3(0, -10, 0)), new SolveDescription(8, 1));
			_dispatcher = new ThreadDispatcher(Environment.ProcessorCount);

			//var sphere = new Box(1, 2, 1);
			//var sphereInertia = sphere.ComputeInertia(1);
			//_body = _simulation.Bodies.Add(BodyDescription.CreateDynamic(new Vector3(0, 5, 0), sphereInertia, _simulation.Shapes.Add(sphere), 0.01f));
			//_body2 = _simulation.Bodies.Add(BodyDescription.CreateDynamic(new Vector3(0, 5, 0), sphereInertia, _simulation.Shapes.Add(sphere), 0.01f));
			//_simulation.Statics.Add(new StaticDescription(new Vector3(0, 0, 0), _simulation.Shapes.Add(new Box(500, 1, 500))));

			//ref var test = ref collisionFilters.Allocate(_body);
			//test = new SubgroupCollisionFilter(0);

			//ref var test2 = ref collisionFilters.Allocate(_body2);
			//test2 = new SubgroupCollisionFilter(0);
			//SubgroupCollisionFilter.DisableCollision(ref test, ref test2);

			Run();
		}

		public string CreateCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{

			var cube = new Box(scale.X, scale.Y, scale.Z);
			var inertia = cube.ComputeInertia(1);
			BodyHandle result = _simulation.Bodies.Add(BodyDescription.CreateDynamic(new RigidPose() { Position = position, Orientation = rotation },
				inertia, _simulation.Shapes.Add(cube), 0.01f));

			ref var filters = ref _collisionFilter.Allocate(result);
			filters = new SubgroupCollisionFilter(0);
			
			return AttachBody(result);
		}

		public BEngine.PhysicsBodyData GetBodyData(string physicsID)
		{
			BodyReference body = _simulation.Bodies[Bodies[physicsID]];
			return new BEngine.PhysicsBodyData() { Position = body.Pose.Position, Rotation = body.Pose.Orientation, Scale = Vector3.One };
		}

		private string AttachBody(BodyHandle handle)
		{
			string id = Guid.NewGuid().ToString();
			Bodies.Add(id, handle);
			return id;
		}

		private async void Run()
		{
			await Task.Run(() =>
			{
				while (running)
				{
					_simulation.Timestep(0.02f, _dispatcher);
					FixedUpdate();
				}

				Clear();
			});
		}

		private void FixedUpdate()
		{
			//Console.WriteLine("Start!!");
			//Console.WriteLine(_simulation.Bodies[_body].Pose.Position);
			//Console.WriteLine(_simulation.Bodies[_body2].Pose.Position);
			//var hitHandler = new RayHitHandler();
			//_simulation.RayCast(new Vector3(0, 4f, 0), new Vector3(0, -1.5f, 0), 1000, ref hitHandler);

			//foreach (RayHit hit in hitHandler.hits)
			//{
			//	Console.WriteLine(hit.HitCollider.Mobility);
			//	if (hit.HitCollider.Mobility == CollidableMobility.Dynamic)
			//	{
			//		Console.WriteLine("F" + _simulation.Bodies[hit.HitCollider.BodyHandle].Pose.Position.ToString());
			//	}
			//	else if (hit.HitCollider.Mobility == CollidableMobility.Static)
			//	{
			//		Console.WriteLine(_simulation.Statics[hit.HitCollider.StaticHandle].Pose.Position.ToString());
			//	}
			//}
		}

		private void Clear()
		{
			_simulation.Dispose();
			_dispatcher.Dispose();
			_bufferPool.Clear();
		}
	}

	public static class PhysicsExtensions
	{

	}
}
