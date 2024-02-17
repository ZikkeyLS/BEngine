
namespace BEngine
{
	public class Vector2
	{
		public float x { get; set; } = 0;
		public float y { get; set; } = 0;

		public Vector2()
		{

		}

		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public class Vector3
	{
		public float x { get; set; } = 0;
		public float y { get; set; } = 0;
		public float z { get; set; } = 0;

		public Vector3()
		{

		}

		public Vector3(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	public class Vector4
	{
		public float x { get; set; } = 0;
		public float y { get; set; } = 0;
		public float z { get; set; } = 0;
		public float w { get; set; } = 0;

		public Vector4()
		{

		}

		public Vector4(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector4(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector4(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
	}

	public class Quaternion
	{
		public float x { get; set; } = 0;
		public float y { get; set; } = 0;
		public float z { get; set; } = 0;
		public float w { get; set; } = 0;

		public Quaternion()
		{

		}

		public Quaternion(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public Quaternion(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Quaternion(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static Quaternion FromEuler(float x, float y, float z)
		{
			System.Numerics.Quaternion result = System.Numerics.Quaternion.CreateFromYawPitchRoll(x, y, z);
			return new Quaternion(result.X, result.Y, result.Z, result.W);
		}
	}
}
