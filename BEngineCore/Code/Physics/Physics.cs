using MagicPhysX;
using System.Numerics;
using System.Text;
using static MagicPhysX.NativeMethods;

namespace BEngineCore
{
	public class PhysicsEntity
	{
		public unsafe PxActor* Actor;
		public unsafe PxTransform Transform;
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
		private List<string> _removeActors = new();

		public Dictionary<string, PhysicsEntity> Actors = new();

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

		private unsafe void FixedUpdate()
		{
			for (int i = 0; i < _removeActors.Count; i++)
			{
				RemoveActor(_removeActors[i]);
			}
			_removeActors.Clear();

			for (int i = 0; i < _addActors.Count; i++)
				scene->AddActorMut(_addActors[i].Actor, null);
			_addActors.Clear();

			lock (Actors)
			{
				foreach (var actor in Actors)
				{
					PxTransform transform = PxRigidActor_getGlobalPose((PxRigidActor*)actor.Value.Actor);
					actor.Value.Transform = transform;
				}
			}

			float calculatedPhysicsDt = 1f / FixedFrames;

			if (ProjectAbstraction.LoadedProject?.Time != null)
			{
				calculatedPhysicsDt *= ProjectAbstraction.LoadedProject.Time.DeltaTime;
			}
			else
			{
				calculatedPhysicsDt *= 0.01f;
			}

			scene->SimulateMut(calculatedPhysicsDt, null, null, 0, true);

			uint error = 0;
			scene->FetchResultsMut(true, &error);

			ExecuteInMainContext(() => {
				ProjectAbstraction.LoadedProject?.LoadedScene?.CallEvent(BEngine.EventID.FixedUpdate);
				ProjectAbstraction.LoadedProject?.LoadedScene?.CallEvent(BEngine.EventID.EditorFixedUpdate);
			});

			//var pose = PxRigidActor_getGlobalPose((PxRigidActor*)sphere);
			//Console.WriteLine($"{i:000}: {pose.p.y}");
		}

		private unsafe void Clear()
		{
			// release resources
			PxScene_release_mut(scene);
			PxDefaultCpuDispatcher_release_mut(dispatcher);
			PxPhysics_release_mut(physics);
		}

		void ExecuteInMainContext(Action action)
		{
			var synchronization = SynchronizationContext.Current;
			if (synchronization != null)
			{
				synchronization.Send(_ => action(), null);//sync
														  //OR
				synchronization.Post(_ => action(), null);//async
			}
			else
				Task.Factory.StartNew(action);
		}

		public unsafe string CreateStaticCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			var cube = PxBoxGeometry_new_1(scale);
			PxVec3 pos = position;
			var transform = PxTransform_new_1(&pos);

			PxShape* shape = physics->CreateShapeMut((PxGeometry*)&cube, material, true, PxShapeFlags.SimulationShape);

			PxRigidStatic* staticResult = physics->PhysPxCreateStatic1(&transform, shape);
			return AttachActor((PxActor*)staticResult);
		}

		public unsafe string CreateDynamicCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			var cube = PxBoxGeometry_new_1(scale);
			PxVec3 pos = position;
			var transform = PxTransform_new_1(&pos);

			PxShape* shape = physics->CreateShapeMut((PxGeometry*)&cube, material, true, PxShapeFlags.SimulationShape);

			PxRigidDynamic* dynamicResult = physics->PhysPxCreateDynamic1(&transform, shape, 0.5f);
			PxRigidBody_setAngularDamping_mut((PxRigidBody*)dynamicResult, 0.5f);

		//	PxVec3 middle = scale / 2;
		//	PxRigidBodyExt_updateMassAndInertia_1((PxRigidBody*)dynamicResult, 10f, &middle, true);

			return AttachActor((PxActor*)dynamicResult);
		}

		private unsafe string AttachActor(PxActor* handle)
		{
			string id = Guid.NewGuid().ToString();

			var physicsEntity = new PhysicsEntity() { Actor = handle };
			_addActors.Add(physicsEntity);
			Actors.Add(id, physicsEntity);

			return id;
		}

		public unsafe BEngine.PhysicsEntryData GetActorData(string physicsID)
		{
			if (Actors.ContainsKey(physicsID) == false)
				return new BEngine.PhysicsEntryData();

			PxTransform transform = Actors[physicsID].Transform;

			return new BEngine.PhysicsEntryData() { Position = (Vector3)transform.p, Rotation = (Quaternion)transform.q };

		}

		public void UpdateActorScale(string physicsID, BEngine.Vector3 scale)
		{
			// TO DO
		}

		public void PreRemoveActor(string physicsID)
		{
			_removeActors.Add(physicsID);
		}

		public unsafe void RemoveActor(string physicsID)
		{
			if (Actors.ContainsKey(physicsID) == false)
				return;

			scene->RemoveActorMut(Actors[physicsID].Actor, true);
			Actors.Remove(physicsID);
		}
	}
}
