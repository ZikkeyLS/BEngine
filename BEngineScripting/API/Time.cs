
namespace BEngine
{
	public class Time
	{
		public static float Speed
		{
			get
			{
				return GetTimeSpeed();
			}
			set
			{
				SetTimeSpeed(value);
			}
		}

		public static float DeltaTime => GetDeltaTime();

		private static void SetTimeSpeed(float speed)
		{
			InternalCalls.SetTimeSpeed(speed);
		}

		private static float GetTimeSpeed()
		{
			return InternalCalls.GetTimeSpeed();
		}

		private static float GetDeltaTime()
		{
			return InternalCalls.GetDeltaTime();
		}

		public static float GetRawDeltaTime()
		{
			return InternalCalls.GetRawDeltaTime();
		}
	}
}
