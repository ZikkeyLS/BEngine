
using System.Runtime.InteropServices;

namespace BEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2
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

		public override string ToString()
		{
			return $"Vector2 ({x};{y})";
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3
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

		public override string ToString()
		{
			return $"Vector3 ({x};{y};{z})";
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4
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

		public override string ToString()
		{
			return $"Vector4 ({x};{y};{z};{w})";
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Quaternion
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

		public override string ToString()
		{
			return $"Quaternion ({x};{y};{z};{w})";
		}
	}
}
