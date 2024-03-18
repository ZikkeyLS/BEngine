using MagicPhysX;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using static MagicPhysX.NativeMethods;

namespace BEngineCore
{
	public class PhysicsEntity
	{
		public unsafe PxActor* Actor;
		public unsafe PxShape* Shape;
		public unsafe PxMaterial* Material;
		public unsafe PxTransform Transform;
		public bool Dynamic;
		public bool Kinematic;
	}

	public struct ChangeActorScale
	{
		public PhysicsEntity entity;
		public Vector3 scale;
	}

	public class Physics
	{
		private unsafe PxFoundation* foundation;
		private unsafe PxPhysics* physics;
		private unsafe PxScene* scene;
		private unsafe PxDefaultCpuDispatcher* dispatcher;
		private unsafe PxMaterial* material;

		private bool running = true;

		private const float FixedFrames = 50;
		private const float ScaleIncrease = 2;
		private const float FallDecreaseScale = 10f;

		private List<PhysicsEntity> _addActors = new List<PhysicsEntity>();
		private List<PhysicsEntity> _removeActors = new();

		public ConcurrentDictionary<string, PhysicsEntity> Actors = new();

		public Physics()
		{

		}

		public unsafe void Initialize()
		{
			foundation = physx_create_foundation();

			PxPvd* pvd = phys_PxCreatePvd(foundation);
			string host = "127.0.0.1";
			int port = 5425;
			uint timeout = 100;

			fixed (byte* hostName = Encoding.UTF8.GetBytes(host))
			{
				PxPvdTransport* transport = phys_PxDefaultPvdSocketTransportCreate(hostName, port, timeout);
				bool connect = pvd->ConnectMut(transport, PxPvdInstrumentationFlags.All);
				if (connect == false)
				{
					Console.WriteLine("PVD connection error");
				}
				else
				{
					Console.WriteLine("PVD connected successfully!");
				}
			}

			// create physics
			uint PX_PHYSICS_VERSION_MAJOR = 5;
			uint PX_PHYSICS_VERSION_MINOR = 1;
			uint PX_PHYSICS_VERSION_BUGFIX = 3;
			uint versionNumber = (PX_PHYSICS_VERSION_MAJOR << 24) + (PX_PHYSICS_VERSION_MINOR << 16) + (PX_PHYSICS_VERSION_BUGFIX << 8);

			var tolerancesScale = new PxTolerancesScale { length = 1, speed = 10 };
			physics = phys_PxCreatePhysics(versionNumber, foundation, &tolerancesScale, true, pvd, null);

			phys_PxInitExtensions(physics, pvd);

			var sceneDescription = PxSceneDesc_new(PxPhysics_getTolerancesScale(physics));
			sceneDescription.gravity = new PxVec3() { x = 0.0f, y = -9.81f, z = 0.0f };
		
			dispatcher = phys_PxDefaultCpuDispatcherCreate(1, null, PxDefaultCpuDispatcherWaitForWorkMode.WaitForWork, 0);
			sceneDescription.cpuDispatcher = (PxCpuDispatcher*)dispatcher;
			sceneDescription.filterShader = get_default_simulation_filter_shader();

			scene = physics->CreateSceneMut(&sceneDescription);
			material = physics->CreateMaterialMut(0.5f, 0.5f, 0.5f);

			// pvd client
			var pvdClient = scene->GetScenePvdClientMut();
			if (pvdClient != null)
			{
				pvdClient->SetScenePvdFlagMut(PxPvdSceneFlag.TransmitConstraints, true);
				pvdClient->SetScenePvdFlagMut(PxPvdSceneFlag.TransmitContacts, true);
				pvdClient->SetScenePvdFlagMut(PxPvdSceneFlag.TransmitScenequeries, true);
			}

			Run();
		}

		private async void Run()
		{
			await Task.Run(() =>
			{
				while (running)
				{
					FixedUpdate();
				}
			});

			Clear();
		}

		private float currentTime = 0;
		private int fps = 0;

		private unsafe void FixedUpdate()
		{
			for (int i = 0; i < _removeActors.Count; i++)
			{
				scene->RemoveActorMut(_removeActors[i].Actor, true);
			}
			_removeActors.Clear();

			for (int i = 0; i < _addActors.Count; i++)
			{
				scene->AddActorMut(_addActors[i].Actor, null);
			}
			_addActors.Clear();

			foreach (var actor in Actors)
			{
				if (actor.Key == null || actor.Value == null)
					continue;
				PxTransform transform = PxRigidActor_getGlobalPose((PxRigidActor*)actor.Value.Actor);
				actor.Value.Transform = transform;
			}

			if (ProjectAbstraction.LoadedProject != null)
			{
				scene->SimulateMut(1f / FixedFrames, null, null, 0, true);
				//scene->CollideMut(ProjectAbstraction.LoadedProject.Time.RawDeltaTime, null, null, 0, true);
				//scene->FetchCollisionMut(true);
				//scene->AdvanceMut(null);
				uint error = 0;
				scene->FetchResultsMut(true, &error);

				ProjectAbstraction? project = ProjectAbstraction.LoadedProject;

				if (project != null && project.LoadedScene != null)
				{
					if (project.Runtime)
					{
						project.LoadedScene.CallEvent(BEngine.EventID.FixedUpdate);
					}

					project.LoadedScene.CallEvent(BEngine.EventID.EditorFixedUpdate);
				}
				Thread.Sleep((int)(ProjectAbstraction.LoadedProject.Time.RawDeltaTime * 1000));
			}
		}

		private unsafe void Clear()
		{
			// Clear existing objects

			PxScene_release_mut(scene);
			PxDefaultCpuDispatcher_release_mut(dispatcher);
			PxPhysics_release_mut(physics);
		}

		public unsafe string CreateStaticCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			var cube = PxBoxGeometry_new_1(scale);
			var transform = CreateTransform(position, rotation);
			var shape = CreateShape((PxGeometry*)&cube, material);

			PxRigidStatic* staticResult = physics->PhysPxCreateStatic1(&transform, shape);
			return AttachActor(new PhysicsEntity() { Actor = (PxActor*)staticResult, Shape = shape, Material = material, Dynamic = false, Kinematic = false });
		}

		public unsafe string CreateDynamicCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			var cube = PxBoxGeometry_new_1(scale);
			var transform = CreateTransform(position, rotation);
			var shape = CreateShape((PxGeometry*)&cube, material);

			PxRigidDynamic* dynamicResult = physics->PhysPxCreateDynamic1(&transform, shape, 0.5f);
			PxRigidBody_setAngularDamping_mut((PxRigidBody*)dynamicResult, 0.5f);

			//	PxVec3 middle = scale / 2;
			//	PxRigidBodyExt_updateMassAndInertia_1((PxRigidBody*)dynamicResult, 10f, &middle, true);

			return AttachActor(new PhysicsEntity() { Actor = (PxActor*)dynamicResult, Shape = shape, Material = material, Dynamic = true, Kinematic = false });
		}

		private unsafe void UpdateKinematicState(string physicsID, bool kinematic)
		{
			if (Actors.ContainsKey(physicsID) == false || Actors[physicsID].Dynamic == false)
				return;

			Actors[physicsID].Kinematic = kinematic;
			PxRigidBody_setRigidBodyFlag_mut((PxRigidBody*)Actors[physicsID].Actor, PxRigidBodyFlag.Kinematic, true);
		}

		public unsafe void UpdateActorScale(string physicsID, Vector3 scale)
		{
			if (Actors.ContainsKey(physicsID) == false)
				return;

			PhysicsEntity entity = Actors[physicsID];

			PxShape* shape = entity.Shape;
			PxGeometryHolder* holder = (PxGeometryHolder*)shape->GetGeometry();

			// switch type
			holder->Box()->halfExtents = scale;

			entity.Shape->SetGeometryMut(holder->Any());
		}

		private unsafe PxTransform CreateTransform(PxVec3 position, PxQuat rotation)
		{
			return PxTransform_new_5(&position, &rotation);
		}

		private unsafe PxShape* CreateShape(PxGeometry* geometry, PxMaterial* material)
		{
			return physics->CreateShapeMut(geometry, material, true, PxShapeFlags.SimulationShape);
		}

		private unsafe string AttachActor(PhysicsEntity physicsEntity)
		{
			string id = Guid.NewGuid().ToString();

			_addActors.Add(physicsEntity);
			Actors.TryAdd(id, physicsEntity);

			return id;
		}

		public unsafe BEngine.PhysicsEntryData GetActorData(string physicsID)
		{
			if (Actors.ContainsKey(physicsID) == false)
				return new BEngine.PhysicsEntryData();

			PxTransform transform = Actors[physicsID].Transform;

			return new BEngine.PhysicsEntryData() { Position = (Vector3)transform.p, Rotation = (Quaternion)transform.q };
		}

		public unsafe void RemoveActor(string physicsID)
		{
			if (Actors.ContainsKey(physicsID) == false)
				return;

			_removeActors.Add(Actors[physicsID]);
			Actors.TryRemove(physicsID, out PhysicsEntity? old);
		}
	}
}
