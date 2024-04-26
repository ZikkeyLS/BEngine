using BEngineScripting;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BEngineCore", AllInternalsVisible = true)]

namespace BEngine
{
	internal class InternalCalls
	{
		private static Type _internalCalls;

		public static void LoadInternalCallsAPI()
		{
			Type? internalCallsAPI = ScriptingUtils.GetTypeByName("BEngineCore.InternalCalls");
			if (internalCallsAPI != null)
			{
				_internalCalls = internalCallsAPI;

				LoadLoggerAssembly();
				LoadGraphicsAssembly();
				LoadInputsAssembly();
				LoadTimeAssembly();
				LoadPhysicsAssembly();
				LoadCameraAssembly();
				LoadProjectAssembly();
			}
		}

		private static MethodInfo? GetMethod(string name) => _internalCalls.GetMethod(name);

		#region Logger
		private static MethodInfo? _logMessage;
		private static MethodInfo? _logWarning;
		private static MethodInfo? _logError;

		private static void LoadLoggerAssembly()
		{
			_logMessage = GetMethod("LogMessage");
			_logWarning = GetMethod("LogWarning");
			_logError = GetMethod("LogErorr");
		}

		public static void LogMessage(string message)
		{
			_logMessage?.Invoke(null, new object[] { message });
		}

		public static void LogWarning(string warning)
		{
			_logWarning?.Invoke(null, new object[] { warning });
		}

		public static void LogError(string error)
		{
			_logError?.Invoke(null, new object[] { error });
		}
		#endregion

		#region Graphics
		private static MethodInfo? _addRenderModel;

		private static void LoadGraphicsAssembly()
		{
			_addRenderModel = GetMethod("AddRenderModel");
		}

		public static void AddRenderModel(RenderModel renderModel)
		{
			_addRenderModel?.Invoke(null, new object[] { renderModel });
		}
		#endregion

		#region Inputs
		private static MethodInfo? _isMouseConnected;
		private static MethodInfo? _isKeyboardConnected;
		private static MethodInfo? _getMousePosition;

		private static MethodInfo? _isKeyDown;
		private static MethodInfo? _isKeyUp;
		private static MethodInfo? _isKeyPressed;

		private static MethodInfo? _isButtonDown;
		private static MethodInfo? _isButtonUp;
		private static MethodInfo? _isButtonPressed;

		private static MethodInfo? _getCursorMode;
		private static MethodInfo? _setCursorMode;

		private static void LoadInputsAssembly()
		{
			_isMouseConnected = GetMethod("IsMouseConnected");
			_isKeyboardConnected = GetMethod("IsKeyboardConnected");
			_getMousePosition = GetMethod("GetMousePosition");

			_isKeyDown = GetMethod("IsKeyDown");
			_isKeyUp = GetMethod("IsKeyUp");
			_isKeyPressed = GetMethod("IsKeyPressed");

			_isButtonDown = GetMethod("IsButtonDown");
			_isButtonUp = GetMethod("IsButtonUp");
			_isButtonPressed = GetMethod("IsButtonPressed");

			_getCursorMode = GetMethod("GetCursorMode");
			_setCursorMode = GetMethod("SetCursorMode");
		}

		public static bool IsMouseConnected()
		{
			return (bool)(_isMouseConnected?.Invoke(null, new object[] { }));
		}

		public static bool IsKeyboardConnected()
		{
			return (bool)(_isKeyboardConnected?.Invoke(null, new object[] { }));
		}

		public static Vector2 GetMousePosition()
		{
			return (Vector2)(_getMousePosition?.Invoke(null, new object[] { }));
		}

		public static bool IsKeyDown(Key key)
		{
			return (bool)(_isKeyDown?.Invoke(null, new object[] { key }));
		}

		public static bool IsKeyUp(Key key)
		{
			return (bool)(_isKeyUp?.Invoke(null, new object[] { key }));
		}

		public static bool IsKeyPressed(Key key)
		{
			return (bool)(_isKeyPressed?.Invoke(null, new object[] { key }));
		}

		public static bool IsButtonDown(MouseButton button)
		{
			return (bool)(_isButtonDown?.Invoke(null, new object[] { button }));
		}

		public static bool IsButtonUp(MouseButton button)
		{
			return (bool)(_isButtonUp?.Invoke(null, new object[] { button }));
		}

		public static bool IsButtonPressed(MouseButton button)
		{
			return (bool)(_isButtonPressed?.Invoke(null, new object[] { button }));
		}

		public static CursorMode GetCursorMode()
		{
			return (CursorMode)(_getCursorMode?.Invoke(null, new object[] { }));
		}

		public static void SetCursorMode(CursorMode mode)
		{
			_setCursorMode?.Invoke(null, new object[] { mode });
		}
		#endregion

		#region Time
		private static MethodInfo? _setTimeSpeed;
		private static MethodInfo? _getTimeSpeed;
		private static MethodInfo? _getRawDeltaTime;
		private static MethodInfo? _getDeltaTime;

		private static void LoadTimeAssembly()
		{
			_setTimeSpeed = GetMethod("SetTimeSpeed");
			_getTimeSpeed = GetMethod("GetTimeSpeed");
			_getRawDeltaTime = GetMethod("GetRawDeltaTime");
			_getDeltaTime = GetMethod("GetDeltaTime");
		}

		public static void SetTimeSpeed(float speed)
		{
			_setTimeSpeed?.Invoke(null, new object[] { speed });
		}

		public static float GetTimeSpeed()
		{
			return (float)(_getTimeSpeed?.Invoke(null, new object[] { }));
		}

		public static float GetRawDeltaTime()
		{
			return (float)(_getRawDeltaTime?.Invoke(null, new object[] { }));
		}

		public static float GetDeltaTime()
		{
			return (float)(_getDeltaTime?.Invoke(null, new object[] { }));
		}
		#endregion

		#region Physics
		private static MethodInfo? _physicsCreateCube;
		private static MethodInfo? _physicsCreateSphere;
		private static MethodInfo? _physicsCreatePlane;
		private static MethodInfo? _physicsCreateCapsule;

		private static MethodInfo? _physicsGetActorData;
		private static MethodInfo? _physicsUpdateActorSize;
		private static MethodInfo? _physicsRemoveActor;

		private static MethodInfo? _physicsChangeKinematic;
		private static MethodInfo? _physicsChangeDynamic;

		private static MethodInfo? _physicsApplyTransform;
		private static MethodInfo? _physicsApplyLock;
		private static MethodInfo? _physicsAddForce;
		private static MethodInfo? _physicsAddTorque;
		private static MethodInfo? _physicsSetVelocity;
		private static MethodInfo? _physicsSetAngularVelocity;
		private static MethodInfo? _physicsGetVelocity;
		private static MethodInfo? _physicsGetAngularVelocity;

		private static void LoadPhysicsAssembly()
		{
			_physicsCreateCube = GetMethod("PhysicsCreateCube");
			_physicsCreateSphere = GetMethod("PhysicsCreateSphere");
			_physicsCreatePlane = GetMethod("PhysicsCreatePlane");
			_physicsCreateCapsule = GetMethod("PhysicsCreateCapsule");

			_physicsGetActorData = GetMethod("PhysicsGetActorData");
			_physicsUpdateActorSize = GetMethod("PhysicsUpdateActorSize");
			_physicsRemoveActor = GetMethod("PhysicsRemoveActor");

			_physicsChangeKinematic = GetMethod("PhysicsChangeKinematic");
			_physicsChangeDynamic = GetMethod("PhysicsChangeDynamic");

			_physicsApplyTransform = GetMethod("PhysicsApplyTransform");
			_physicsApplyLock = GetMethod("PhysicsApplyLock");
			_physicsAddForce = GetMethod("PhysicsAddForce");
			_physicsAddTorque = GetMethod("PhysicsAddTorque");
			_physicsSetVelocity = GetMethod("PhysicsSetVelocity");
			_physicsSetAngularVelocity = GetMethod("PhysicsSetAngularVelocity");

			_physicsGetVelocity = GetMethod("PhysicsGetVelocity");
			_physicsGetAngularVelocity = GetMethod("PhysicsGetAngularVelocity");
		}

		public static string PhysicsCreateCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			return (string)(_physicsCreateCube?.Invoke(null, new object[] { position, rotation, scale }));
		}

		public static string PhysicsCreateSphere(Vector3 position, Quaternion rotation, float radius)
		{
			return (string)(_physicsCreateSphere?.Invoke(null, new object[] { position, rotation, radius }));
		}

		public static string PhysicsCreatePlane(Vector3 position, Quaternion rotation, Vector2 size)
		{
			return (string)(_physicsCreatePlane?.Invoke(null, new object[] { position, rotation, size }));
		}

		public static string PhysicsCreateCapsule(Vector3 position, Quaternion rotation, float halfHeight, float radius)
		{
			return (string)(_physicsCreateCapsule?.Invoke(null, new object[] { position, rotation, halfHeight, radius }));
		}

		public static PhysicsEntryData PhysicsGetActorData(string physicsID)
		{
			return (PhysicsEntryData)(_physicsGetActorData?.Invoke(null, new object[] { physicsID }));
		}

		public static void PhysicsUpdateActorSize(string physicsID, object[] data)
		{
			_physicsUpdateActorSize?.Invoke(null, new object[] { physicsID, data });
		}

		public static void PhysicsRemoveActor(string physicsID)
		{
			_physicsRemoveActor?.Invoke(null, new object[] { physicsID });
		}

		public static void PhysicsChangeKinematic(string physicsID, bool kinematic)
		{
			_physicsChangeKinematic?.Invoke(null, new object[] { physicsID, kinematic });
		}

		public static void PhysicsChangeDynamic(string physicsID, bool dynamic)
		{
			_physicsChangeDynamic?.Invoke(null, new object[] { physicsID, dynamic });
		}

		public static void PhysicsApplyTransform(string physicsID, Vector3 position, Quaternion rotation)
		{
			_physicsApplyTransform?.Invoke(null, new object[] { physicsID, position, rotation });
		}

		public static void PhysicsApplyLock(string physicsID, Vector3Bool linearLock, Vector3Bool angularLock)
		{
			_physicsApplyLock?.Invoke(null, new object[] { physicsID, linearLock, angularLock });
		}

		public static void PhysicsAddForce(string physicsID, Vector3 force, ForceMode mode)
		{
			_physicsAddForce?.Invoke(null, new object[] { physicsID, force, mode });
		}

		public static void PhysicsAddTorque(string physicsID, Vector3 torque, ForceMode mode)
		{
			_physicsAddTorque?.Invoke(null, new object[] { physicsID, torque, mode });
		}

		public static void PhysicsSetVelocity(string physicsID, Vector3 velocity)
		{
			_physicsSetVelocity?.Invoke(null, new object[] { physicsID, velocity });
		}

		public static void PhysicsSetAngularVelocity(string physicsID, Vector3 velocity)
		{
			_physicsSetAngularVelocity?.Invoke(null, new object[] { physicsID, velocity });
		}

		public static Vector3 PhysicsGetVelocity(string physicsID)
		{
			return (Vector3)(_physicsGetVelocity?.Invoke(null, new object[] { physicsID }));
		}

		public static Vector3 PhysicsGetAngularVelocity(string physicsID)
		{
			return (Vector3)(_physicsGetAngularVelocity?.Invoke(null, new object[] { physicsID }));
		}

		#endregion

		#region Camera
		private static MethodInfo? _cameraCreateRequest;

		private static void LoadCameraAssembly()
		{
			_cameraCreateRequest = GetMethod("CameraCreateRequest");
		}

		public static void CameraCreateRequest(string GUID, uint priority, Vector3 position, Quaternion rotation)
		{
			_cameraCreateRequest?.Invoke(null, new object[] { GUID, priority, position, rotation });
		}
		#endregion

		#region Project
		private static MethodInfo? _projectIsEditor;
		private static MethodInfo? _projectIsRuntime;

		private static void LoadProjectAssembly()
		{
			_projectIsEditor = GetMethod("ProjectIsEditor");
			_projectIsRuntime = GetMethod("ProjectIsRuntime");
		}

		public static bool ProjectIsEditor()
		{
			return (bool)(_projectIsEditor?.Invoke(null, new object[] { }));
		}

		public static bool ProjectIsRuntime()
		{
			return (bool)(_projectIsRuntime?.Invoke(null, new object[] { }));
		}
		#endregion
	}
}
