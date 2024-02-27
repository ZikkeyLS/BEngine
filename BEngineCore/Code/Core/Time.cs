
namespace BEngineCore
{
	public class Time
	{
		public float DeltaTime => RawDeltaTime * Speed;
		public float RawDeltaTime = 0f;
		public float Speed = 1f;
	}
}
