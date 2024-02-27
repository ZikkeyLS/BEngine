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
	}
}
