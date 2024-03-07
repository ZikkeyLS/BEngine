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
		private static MethodInfo? _physicsCreateDynamicCube;
		private static MethodInfo? _physicsCreateStaticCube;

		private static MethodInfo? _physicsGetDynamicData;
		private static MethodInfo? _physicsGetStaticData;

		private static MethodInfo? _physicsUpdateStaticScale;
		private static MethodInfo? _physicsUpdateDynamicScale;

		private static MethodInfo? _physicsRemoveStatic;
		private static MethodInfo? _physicsRemoveDynamic;


		private static void LoadPhysicsAssembly()
		{
			_physicsCreateStaticCube = GetMethod("PhysicsCreateStaticCube");
			_physicsCreateDynamicCube = GetMethod("PhysicsCreateDynamicCube");

			_physicsGetStaticData = GetMethod("PhysicsGetStaticData");
			_physicsGetDynamicData = GetMethod("PhysicsGetDynamicData");

			_physicsUpdateStaticScale = GetMethod("PhysicsUpdateStaticScale");
			_physicsUpdateDynamicScale = GetMethod("PhysicsUpdateDynamicScale");

			_physicsRemoveStatic = GetMethod("PhysicsRemoveStatic");
			_physicsRemoveDynamic = GetMethod("PhysicsRemoveDynamic");
		}

		public static string PhysicsCreateStaticCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			return (string)(_physicsCreateStaticCube?.Invoke(null, new object[] { position, rotation, scale }));
		}

		public static string PhysicsCreateDynamicCube(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			return (string)(_physicsCreateDynamicCube?.Invoke(null, new object[] { position, rotation, scale }));
		}

		public static PhysicsEntryData PhysicsGetStaticData(string physicsID)
		{
			return (PhysicsEntryData)(_physicsGetStaticData?.Invoke(null, new object[] { physicsID }));
		}

		public static PhysicsEntryData PhysicsGetDynamicData(string physicsID)
		{
			return (PhysicsEntryData)(_physicsGetDynamicData?.Invoke(null, new object[] { physicsID }));
		}

		public static void PhysicsUpdateStaticScale(string physicsID, Vector3 scale)
		{
			_physicsUpdateStaticScale?.Invoke(null, new object[] { physicsID, scale });
		}

		public static void PhysicsUpdateDynamicScale(string physicsID, Vector3 scale)
		{
			_physicsUpdateDynamicScale?.Invoke(null, new object[] { physicsID, scale });
		}

		public static void PhysicsRemoveStatic(string physicsID)
		{
			_physicsRemoveStatic?.Invoke(null, new object[] { physicsID });
		}

		public static void PhysicsRemoveDynamic(string physicsID)
		{
			_physicsRemoveDynamic?.Invoke(null, new object[] { physicsID });
		}

		#endregion
	}
}
