
using System.Runtime.InteropServices;

namespace BEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2
	{
		public readonly static Vector2 zero = new(0, 0);
		public readonly static Vector2 one = new(1, 1);

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

		public static implicit operator Vector2(System.Numerics.Vector2 v)
		{
			return new(v.X, v.Y);
		}

		public static explicit operator System.Numerics.Vector2(Vector2 v)
		{
			return new(v.x, v.y);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3
	{
		public readonly static Vector3 zero = new(0, 0, 0);
		public readonly static Vector3 one = new(1, 1, 1);

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

		public static implicit operator Vector3(System.Numerics.Vector3 v)
		{
			return new(v.X, v.Y, v.Z);
		}

		public static explicit operator System.Numerics.Vector3(Vector3 v)
		{
			return new(v.x, v.y, v.z);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4
	{
		public readonly static Vector4 zero = new(0, 0, 0, 0);
		public readonly static Vector4 one = new(1, 1, 1, 1);

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

		public static implicit operator Vector4(System.Numerics.Vector4 v)
		{
			return new(v.X, v.Y, v.Z, v.W);
		}

		public static explicit operator System.Numerics.Vector4(Vector4 v)
		{
			return new(v.x, v.y, v.z, v.w);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Quaternion
	{
		public readonly static Quaternion identity = new(0, 0, 0, 1);
		public readonly static Quaternion zero = new(0, 0, 0, 0);
		public readonly static Quaternion one = new(1, 1, 1, 1);

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

		public static implicit operator Quaternion(System.Numerics.Quaternion q)
		{
			return new(q.X, q.Y, q.Z, q.W);
		}

		public static explicit operator System.Numerics.Quaternion(Quaternion q)
		{
			return new(q.x, q.y, q.z, q.w);
		}
	}
}
