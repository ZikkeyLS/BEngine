using BEngine;
using MagicPhysX;
using Silk.NET.Vulkan.Video;
using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using static BEngine.Collider;
using static MagicPhysX.NativeMethods;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace BEngineCore
{
	public class PhysicsEntity
	{
		public unsafe PxActor* Actor;
		public unsafe PxShape* Shape;
		public unsafe PxMaterial* Material;
		public unsafe PxTransform Transform;
		public ColliderType ColliderType;
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

		private List<PhysicsEntity> _addActors = new();
		private List<PhysicsEntity> _removeActors = new();
		private List<PhysicsEntity> _swipeActors = new();
		private List<PhysicsEntity> _changeKinematic = new();
		private List<PhysicsEntity> _applyTransform = new();

		public ConcurrentDictionary<string, PhysicsEntity> Actors = new();

		public Physics()
		{
			// left impl Sphere, Capsule, Collisions, RaycastLayers, Raycasts
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

			for (int i = 0; i < _swipeActors.Count; i++)
			{
				PhysicsEntity entity = _swipeActors[i];

				scene->RemoveActorMut(entity.Actor, true);

				PxActor* actor;
				PxTransform transform = entity.Transform;
				if (entity.Dynamic)
				{
					actor = (PxActor*)physics->PhysPxCreateDynamic1(&transform, entity.Shape, 0.5f);
				}
				else
				{
					actor = (PxActor*)physics->PhysPxCreateStatic1(&transform, entity.Shape);
				}

				entity.Actor = actor;
				scene->AddActorMut(actor, null);
			}
			_swipeActors.Clear();

			for (int i = 0; i < _changeKinematic.Count; i++)
			{
				PhysicsEntity entity = _changeKinematic[i];

				PxRigidBody_setRigidBodyFlag_mut((PxRigidBody*)entity.Actor, PxRigidBodyFlag.Kinematic, entity.Kinematic);
				if (entity.Kinematic == false)
				{
					// To remove sleeping state we need to apply global pose again
					PxTransform transform = entity.Transform;
					PxRigidActor_setGlobalPose_mut((PxRigidActor*)entity.Actor, &transform, true);
				}
			}
			_changeKinematic.Clear();

			for (int i = 0; i < _applyTransform.Count; i++)
			{
				PhysicsEntity entity = _applyTransform[i];
				PxTransform transform = entity.Transform;
				PxRigidActor_setGlobalPose_mut((PxRigidActor*)entity.Actor, &transform, true);
			}
			_applyTransform.Clear();

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
						project.LoadedScene.CallEvent(EventID.FixedUpdate);
					}

					project.LoadedScene.CallEvent(EventID.EditorFixedUpdate);
				}

				currentTime += 1f / FixedFrames;
				fps += 1;
				
				if (currentTime > 1)
				{
					Console.WriteLine(fps);
					fps = 0;
					currentTime = 0;
				}

				Thread.Sleep((int)(1f / FixedFrames * 1000));
			}
		}

		private unsafe void Clear()
		{
			// Clear existing objects

			PxScene_release_mut(scene);
			PxDefaultCpuDispatcher_release_mut(dispatcher);
			PxPhysics_release_mut(physics);
		}

		public void ChangeDynamic(string physicsID, bool dynamic)
		{
			if (Actors.TryGetValue(physicsID, out PhysicsEntity? actor) == false)
				return;

			actor.Dynamic = dynamic;
			_swipeActors.Add(actor);
		}

		public void ChangeKinematic(string physicsID, bool kinematic)
		{
			if (Actors.TryGetValue(physicsID, out PhysicsEntity? actor) == false)
				return;

			actor.Kinematic = kinematic;
			_changeKinematic.Add(actor);
		}

		public void ApplyTransform(string physicsID, Vector3 position, Quaternion rotation)
		{
			if (Actors.TryGetValue(physicsID, out PhysicsEntity? actor) == false)
				return;

			actor.Transform.p = position;
			actor.Transform.q = rotation;
			_applyTransform.Add(actor);
		}

		public unsafe string CreateStaticCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			var cube = PxBoxGeometry_new_1(scale);
			var transform = CreateTransform(position, rotation);
			var shape = CreateShape((PxGeometry*)&cube, material);

			PxRigidStatic* staticResult = physics->PhysPxCreateStatic1(&transform, shape);
			return AttachActor(new PhysicsEntity() { Actor = (PxActor*)staticResult, 
				Shape = shape, ColliderType = ColliderType.Cube,
				Material = material, Dynamic = false, Kinematic = false, 
				Transform = transform });
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

		public unsafe PhysicsEntryData GetActorData(string physicsID)
		{
			if (Actors.ContainsKey(physicsID) == false)
				return new PhysicsEntryData();

			PxTransform transform = Actors[physicsID].Transform;

			return new PhysicsEntryData() { Position = (Vector3)transform.p, Rotation = (Quaternion)transform.q };
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
