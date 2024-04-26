using BEngine;

namespace BEngineCore
{
	public class InternalCalls
	{
		#region Logger

		public static void LogMessage(string message)
		{
			if (Logger.Main != null)
				Logger.Main.LogMessage(message);
		}

		public static void LogWarning(string warning)
		{
			if (Logger.Main != null)
				Logger.Main.LogWarning(warning);
		}

		public static void LogError(string error)
		{
			if (Logger.Main != null)
				Logger.Main.LogError(error);
		}
		#endregion

		#region Graphics
		public static void AddRenderModel(RenderModel renderModel)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject != null)
			{
				Model? model = loadedProject.AssetsReader.ModelContext.GetModel(renderModel.Model.GUID);
				if (model != null)
					loadedProject.Graphics.ModelsToRender.Add(new ModelRenderContext { Model = model, Transform = renderModel.Transform });
			}
		}
		#endregion

		#region Input
		public static bool IsKeyDown(Key key)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return false;

			return loadedProject.Input.IsKeyDown(key);
		}

		public static bool IsKeyUp(Key key)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return false;

			return loadedProject.Input.IsKeyUp(key);
		}

		public static bool IsKeyPressed(Key key)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return false;

			return loadedProject.Input.IsKeyPressed(key);
		}

		public static bool IsButtonDown(MouseButton mouseButton)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return false;

			return loadedProject.Input.IsButtonDown(mouseButton);
		}

		public static bool IsButtonUp(MouseButton mouseButton)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return false;

			return loadedProject.Input.IsButtonUp(mouseButton);
		}

		public static bool IsButtonPressed(MouseButton mouseButton)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return false;

			return loadedProject.Input.IsButtonPressed(mouseButton);
		}

		public static bool IsKeyboardConnected()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return false;

			return loadedProject.Input.IsKeyboardConnected();
		}

		public static bool IsMouseConnected()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return false;

			return loadedProject.Input.IsMouseConnected();
		}

		public static Vector2 GetMousePosition()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return Vector2.zero;

			return loadedProject.Input.GetMousePosition();
		}

		public void SetCursorMode(CursorMode mode)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Input.SetCursorMode(mode);
		}

		public CursorMode GetCursorMode()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return CursorMode.Disabled;

			return loadedProject.Input.GetCursorMode();
		}
		#endregion

		#region Time
		public static void SetTimeSpeed(float speed)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject != null)
				loadedProject.Time.Speed = speed;
		}

		public static float GetTimeSpeed()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject != null)
				return loadedProject.Time.Speed;

			return 0f;
		}

		public static float GetRawDeltaTime()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject != null)
				return loadedProject.Time.RawDeltaTime;

			return 0f;
		}

		public static float GetDeltaTime()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject != null)
				return loadedProject.Time.DeltaTime;

			return 0f;
		}
		#endregion

		#region Physics
		public static string PhysicsCreateCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return string.Empty;

			return loadedProject.Physics.CreateStaticCube(
				(System.Numerics.Vector3)position,
				(System.Numerics.Quaternion)rotation,
				(System.Numerics.Vector3)scale);
		}

		public static string PhysicsCreateSphere(Vector3 position, Quaternion rotation, float radius)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return string.Empty;

			return loadedProject.Physics.CreateStaticSphere(
				(System.Numerics.Vector3)position,
				(System.Numerics.Quaternion)rotation, radius);
		}

		public static string PhysicsCreatePlane(Vector3 position, Quaternion rotation, Vector2 size)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return string.Empty;

			return loadedProject.Physics.CreateStaticPlane(
				(System.Numerics.Vector3)position,
				(System.Numerics.Quaternion)rotation,
				(System.Numerics.Vector2)size);
		}

		public static string PhysicsCreateCapsule(Vector3 position, Quaternion rotation, float halfHeight, float radius)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return string.Empty;

			return loadedProject.Physics.CreateStaticCapsule(
				(System.Numerics.Vector3)position,
				(System.Numerics.Quaternion)rotation, halfHeight, radius);
		}

		public static PhysicsEntryData PhysicsGetActorData(string physicsID)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return new();

			return loadedProject.Physics.GetActorData(physicsID);
		}

		public static Vector3 PhysicsGetVelocity(string physicsID)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return new();

			return loadedProject.Physics.GetVelocity(physicsID);
		}

		public static Vector3 PhysicsGetAngularVelocity(string physicsID)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return new();

			return loadedProject.Physics.GetAngularVelocity(physicsID);
		}

		public static void PhysicsUpdateActorSize(string physicsID, object[] data)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.UpdateActorSize(physicsID, data);
		}

		public static void PhysicsRemoveActor(string physicsID)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.RemoveActor(physicsID);
		}

		public static void PhysicsChangeKinematic(string physicsID, bool kinematic)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.ChangeKinematic(physicsID, kinematic);
		}

		public static void PhysicsChangeDynamic(string physicsID, bool dynamic)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.ChangeDynamic(physicsID, dynamic);
		}

		public static void PhysicsApplyTransform(string physicsID, Vector3 position, Quaternion rotation)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.ApplyTransform(physicsID, (System.Numerics.Vector3)position, (System.Numerics.Quaternion)rotation);
		}

		public static void PhysicsApplyLock(string physicsID, Vector3Bool lockLinear, Vector3Bool lockAngular)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.ApplyLock(physicsID, lockLinear, lockAngular);
		}

		public static void PhysicsAddForce(string physicsID, Vector3 force, ForceMode mode)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.ApplyAddForce(physicsID, (System.Numerics.Vector3)force, (MagicPhysX.PxForceMode)mode);
		}

		public static void PhysicsAddTorque(string physicsID, Vector3 force, ForceMode mode)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.ApplyAddTorque(physicsID, (System.Numerics.Vector3)force, (MagicPhysX.PxForceMode)mode);
		}

		public static void PhysicsSetVelocity(string physicsID, Vector3 velocity)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.ApplyVelocity(physicsID, (System.Numerics.Vector3)velocity);
		}

		public static void PhysicsSetAngularVelocity(string physicsID, Vector3 velocity)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject == null)
				return;

			loadedProject.Physics.ApplyAngularVelocity(physicsID, (System.Numerics.Vector3)velocity);
		}
		#endregion

		#region Camera
		public static void CameraCreateRequest(string GUID, uint priority, Vector3 position, Quaternion rotation)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject != null)
			{
				loadedProject.Graphics.AddCameraRequest(new CameraHandlerRequest() { GUID = GUID, Priority = priority, 
					Position = (System.Numerics.Vector3)position, 
					Rotation = (System.Numerics.Quaternion)rotation });
			}
		}
		#endregion

		#region Project
		public static bool ProjectIsEditor()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;
			return loadedProject != null && loadedProject.Editor;
		}

		public static bool ProjectIsRuntime()
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;
			return loadedProject != null && loadedProject.Runtime;
		}
		#endregion
	}
}
