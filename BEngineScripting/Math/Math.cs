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

		public static bool operator ==(Vector2 a, Vector2 b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vector2 a, Vector2 b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public static Vector2 operator +(Vector2 a, Vector2 b) 
		{
			return new Vector2(a.x + b.x, a.y + b.y);
		}

		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x - b.x, a.y - b.y);
		}

		public static Vector2 operator *(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x, a.y * b.y);
		}

		public static Vector2 operator /(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x / b.x, a.y / b.y);
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

		public static bool operator ==(Vector3 a, Vector3 b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(Vector3 a, Vector3 b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z;
		}

		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3 operator *(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3 operator /(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
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

		public static bool operator ==(Vector4 a, Vector4 b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		public static bool operator !=(Vector4 a, Vector4 b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
		}

		public static Vector4 operator +(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vector4 operator -(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Vector4 operator *(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		public static Vector4 operator /(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
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
			x = float.DegreesToRadians(x);
			y = float.DegreesToRadians(y);
			z = float.DegreesToRadians(z);

			float rollOver2 = z * 0.5f;
			float sinRollOver2 = (float)Math.Sin((double)rollOver2);
			float cosRollOver2 = (float)Math.Cos((double)rollOver2);
			float pitchOver2 = x * 0.5f;
			float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
			float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
			float yawOver2 = y * 0.5f;
			float sinYawOver2 = (float)Math.Sin((double)yawOver2);
			float cosYawOver2 = (float)Math.Cos((double)yawOver2);

			Quaternion result = identity;
			result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
			result.x = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
			result.y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
			result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;


			return new Quaternion(result.x, result.y, result.z, result.w);
		}

		public static Quaternion FromEuler(Vector3 euler)
		{
			return FromEuler(euler.x, euler.y, euler.z);
		}

		public static Vector3 ToEuler(Quaternion q)
		{
			float sqw = q.w * q.w;
			float sqx = q.x * q.x;
			float sqy = q.y * q.y;
			float sqz = q.z * q.z;
			float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
			float test = q.x * q.w - q.y * q.z;
			Vector3 v = Vector3.zero;

			if (test > 0.4995f * unit)
			{ // singularity at north pole
				v.y = (float)(2f * Math.Atan2(q.y, q.x));
				v.x = (float)(Math.PI / 2);
				v.z = 0;

				v.x = float.RadiansToDegrees(v.x);
				v.y = float.RadiansToDegrees(v.y);
				v.z = float.RadiansToDegrees(v.z);
				return NormalizeAngles(v);
			}
			if (test < -0.4995f * unit)
			{ // singularity at south pole
				v.y = (float)(-2f * Math.Atan2(q.y, q.x));
				v.x = (float)(-Math.PI / 2);
				v.z = 0;

				v.x = float.RadiansToDegrees(v.x);
				v.y = float.RadiansToDegrees(v.y);
				v.z = float.RadiansToDegrees(v.z);
				return NormalizeAngles(v);
			}

			Quaternion q1 = new Quaternion(q.w, q.z, q.x, q.y);
			v.y = (float)Math.Atan2(2f * q1.x * q1.w + 2f * q1.y * q1.z, 1 - 2f * (q1.z * q1.z + q1.w * q1.w));     // Yaw
			v.x = (float)Math.Asin(2f * (q1.x * q1.z - q1.w * q1.y));                             // Pitch
			v.z = (float)Math.Atan2(2f * q1.x * q1.y + 2f * q1.z * q1.w, 1 - 2f * (q1.y * q1.y + q1.z * q1.z));      // Roll

			v.x = float.RadiansToDegrees(v.x);
			v.y = float.RadiansToDegrees(v.y);
			v.z = float.RadiansToDegrees(v.z);
			return NormalizeAngles(v);
		}

		static Vector3 NormalizeAngles(Vector3 angles)
		{
			angles.x = NormalizeAngle(angles.x);
			angles.y = NormalizeAngle(angles.y);
			angles.z = NormalizeAngle(angles.z);
			return angles;
		}

		static float NormalizeAngle(float angle)
		{
			while (angle > 360)
				angle -= 360;
			while (angle < 0)
				angle += 360;
			return angle;
		}

		public override string ToString()
		{
			return $"Quaternion ({x};{y};{z};{w})";
		}

		public static bool operator ==(Quaternion a, Quaternion b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		public static bool operator !=(Quaternion a, Quaternion b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
		}

		public static Quaternion operator +(Quaternion a, Quaternion b)
		{
			return new Quaternion(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Quaternion operator -(Quaternion a, Quaternion b)
		{
			return new Quaternion(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Quaternion operator *(Quaternion a, Quaternion b)
		{
			return new Quaternion(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}
		public static Quaternion operator /(Quaternion a, Quaternion b)
		{
			return new Quaternion(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}


		public static implicit operator Quaternion(System.Numerics.Quaternion q)
		{
			return new(q.X, q.Y, q.Z, q.W);
		}

		public static explicit operator System.Numerics.Quaternion(Quaternion q)
		{
			return new(q.x, q.y, q.z, q.w);
		}

		public static explicit operator System.Numerics.Vector3(Quaternion v)
		{
			throw new NotImplementedException();
		}
	}
}
