using System.Runtime.InteropServices;

namespace BEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        public readonly static Vector2 zero = new(0, 0);
        public readonly static Vector2 one = new(1, 1);

        public readonly static Vector2 Right = new(1, 0);
        public readonly static Vector2 Up = new(0, 1);

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

        public static Vector2 Abs(Vector2 value)
        {
            return System.Numerics.Vector2.Abs((System.Numerics.Vector2)value);
        }

        public static Vector2 Add(Vector2 value, Vector2 value2)
        {
            return System.Numerics.Vector2.Add((System.Numerics.Vector2)value, (System.Numerics.Vector2)value2);
        }

        public static Vector2 Clamp(Vector2 value, Vector2 value2, Vector2 value3)
        {
            return System.Numerics.Vector2.Clamp((System.Numerics.Vector2)value, (System.Numerics.Vector2)value2, (System.Numerics.Vector2)value3);
        }

        public static float Distance(Vector2 value, Vector2 value2)
        {
            return System.Numerics.Vector2.Distance((System.Numerics.Vector2)value, (System.Numerics.Vector2)value2);
        }

        public static float DistanceSquared(Vector2 value, Vector2 value2)
        {
            return System.Numerics.Vector2.DistanceSquared((System.Numerics.Vector2)value, (System.Numerics.Vector2)value2);
        }

        public static Vector2 Divide(Vector2 left, float divisor)
        {
            return System.Numerics.Vector2.Divide((System.Numerics.Vector2)left, divisor);
        }

        public static float Dot(Vector2 value, Vector2 value2)
        {
            return System.Numerics.Vector2.Dot((System.Numerics.Vector2)value, (System.Numerics.Vector2)value2);
        }

        public static float Length(Vector2 value)
        {
            return ((System.Numerics.Vector2)value).Length();
        }

        public static float LengthSquared(Vector2 value)
        {
            return ((System.Numerics.Vector2)value).LengthSquared();
        }

        public static Vector2 Lerp(Vector2 value, Vector2 value2, float amount)
        {
            return System.Numerics.Vector2.Lerp((System.Numerics.Vector2)value, (System.Numerics.Vector2)value2, amount);
        }

        public static Vector2 Max(Vector2 value, Vector2 value2)
        {   
            return System.Numerics.Vector2.Max((System.Numerics.Vector2)value, (System.Numerics.Vector2)value2);
        }

        public static Vector2 Min(Vector2 value, Vector2 value2)
        {
            return System.Numerics.Vector2.Min((System.Numerics.Vector2)value, (System.Numerics.Vector2)value2);
        }

        public static Vector2 Multiply(Vector2 left, Vector2 right)
        {
            return System.Numerics.Vector2.Multiply((System.Numerics.Vector2)left, (System.Numerics.Vector2)right);
        }

        public static Vector2 Multiply(Vector2 left, float right)
        {
            return System.Numerics.Vector2.Multiply((System.Numerics.Vector2)left, right);
        }

        public static Vector2 Multiply(float left, Vector2 right)
        {
            return System.Numerics.Vector2.Multiply(left, (System.Numerics.Vector2)right);
        }

        public static Vector2 Negate(Vector2 value)
        {
            return System.Numerics.Vector2.Negate((System.Numerics.Vector2)value);
        }
        public static Vector2 Normalize(Vector2 value)
        {
            return System.Numerics.Vector2.Normalize((System.Numerics.Vector2)value);
        }

        public static Vector2 Reflect(Vector2 value, Vector2 normal)
        {
            return System.Numerics.Vector2.Reflect((System.Numerics.Vector2)value, (System.Numerics.Vector2)normal);
        }

        public static Vector2 SquareRoot(Vector2 value)
        {
            return System.Numerics.Vector2.SquareRoot((System.Numerics.Vector2)value);
        }

        public static Vector2 Substract(Vector2 left, Vector2 right)
        {
            return System.Numerics.Vector2.Subtract((System.Numerics.Vector2)left, (System.Numerics.Vector2)right);
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

        public static implicit operator System.Numerics.Vector2(Vector2 v)
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
            float sinRollOver2 = Math.Sin(rollOver2);
            float cosRollOver2 = Math.Cos(rollOver2);
            float pitchOver2 = x * 0.5f;
            float sinPitchOver2 = Math.Sin(pitchOver2);
            float cosPitchOver2 = Math.Cos(pitchOver2);
            float yawOver2 = y * 0.5f;
            float sinYawOver2 = Math.Sin(yawOver2);
            float cosYawOver2 = Math.Cos(yawOver2);

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
                v.y = 2f * Math.Atan2(q.y, q.x);
                v.x = Math.PI / 2;
                v.z = 0;

                v.x = float.RadiansToDegrees(v.x);
                v.y = float.RadiansToDegrees(v.y);
                v.z = float.RadiansToDegrees(v.z);
                return NormalizeAngles(v);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.y = -2f * Math.Atan2(q.y, q.x);
                v.x = -Math.PI / 2;
                v.z = 0;

                v.x = float.RadiansToDegrees(v.x);
                v.y = float.RadiansToDegrees(v.y);
                v.z = float.RadiansToDegrees(v.z);
                return NormalizeAngles(v);
            }

            Quaternion q1 = new Quaternion(q.w, q.z, q.x, q.y);
            v.y = Math.Atan2(2f * q1.x * q1.w + 2f * q1.y * q1.z, 1 - 2f * (q1.z * q1.z + q1.w * q1.w));     // Yaw
            v.x = Math.Asin(2f * (q1.x * q1.z - q1.w * q1.y));                             // Pitch
            v.z = Math.Atan2(2f * q1.x * q1.y + 2f * q1.z * q1.w, 1 - 2f * (q1.y * q1.y + q1.z * q1.z));      // Roll

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
    }
    public static class Math
    {
        public const float E = 2.718281828f;
        public const float PI = 3.141592653f;
        public const float Tau = 6.283185307f;

        public static int Abs(int a)
        {
            return System.Math.Abs(a);
        }

        public static float Abs(float a)
        {
            return System.Math.Abs(a);
        }

        public static float Acos(float a)
        {
            return (float)System.Math.Acos(a);
        }

        public static float Acosh(float a)
        {
            return (float)System.Math.Acosh(a);
        }

        public static float Asin(float a)
        {
            return (float)System.Math.Asin(a);
        }

        public static float Asinh(float a)
        {
            return (float)System.Math.Asinh(a);
        }

        public static float Atan(float a)
        {
            return (float)System.Math.Atan(a);
        }

        public static float Atan2(float a, float b)
        {
            return (float)System.Math.Atan2(a, b);
        }

        public static float Atanh(float a)
        {
            return (float)System.Math.Atanh(a);
        }

        public static float Cbrt(float a)
        {
            return (float)System.Math.Cbrt(a);
        }

        public static float Celing(float a)
        {
            return (float)System.Math.Ceiling(a);
        }

        public static float Clamp(float a, float b, float c)
        {
            return System.Math.Clamp(a, b, c);
        }

        public static int Clamp(int a, int b, int c)
        {
            return System.Math.Clamp(a, b, c);
        }

        public static float CopySing(float a, float b)
        {
            return (float)System.Math.CopySign(a, b);
        }

        public static float Cos(float a)
        {
            return (float)System.Math.Cos(a);
        }

        public static float Cosh(float a)
        {
            return (float)System.Math.Cosh(a);
        }

        public static float Exp(float a)
        {
            return (float)System.Math.Exp(a);
        }

        public static float Floor(float a)
        {
            return (float)System.Math.Floor(a);
        }

        public static float FusedMultiplyAdd(float a, float b, float c)
        {
            return (float)System.Math.FusedMultiplyAdd(a, b, c);
        }

        public static float IEEERemainder(float a, float b)
        {
            return (float)System.Math.IEEERemainder(a, b);
        }

        public static int ILogB(float a)
        {
            return System.Math.ILogB(a);
        }

        public static float Log(float a)
        {
            return (float)System.Math.Log(a);
        }

        public static float Log10(float a)
        {
            return (float)System.Math.Log10(a);
        }

        public static float Log2(float a)
        {
            return (float)System.Math.Log2(a);
        }

        public static float Max(float a, float b)
        {
            return System.Math.Max(a, b);
        }

        public static int Max(int a, int b)
        {
            return System.Math.Max(a, b);
        }

        public static float MaxMagnitude(float a, float b)
        {
            return (float)System.Math.MaxMagnitude(a, b);
        }

        public static float Min(float a, float b)
        {
            return System.Math.Min(a, b);
        }

        public static int Min(int a, int b)
        {
            return System.Math.Min(a, b);
        }

        public static float MinMagnitude(float a, float b)
        {
            return (float)System.Math.MinMagnitude(a, b);
        }

        public static float Pow(float a, float b)
        {
            return (float)System.Math.Pow(a, b);
        }

        public static float ReciprocalEstimate(float a)
        {
            return (float)System.Math.ReciprocalEstimate(a);
        }

        public static float ReciprocalSqrtEstimate(float a)
        {
            return (float)System.Math.ReciprocalSqrtEstimate(a);
        }

        public static float Round(float a)
        {
            return (float)System.Math.Round(a);
        }

        public static float Round(float value, int digits)
        {
            return (float)System.Math.Round(value, digits);
        }

        public static float Round(float value, int digits, MidpointRounding mode)
        {
            return (float)System.Math.Round(value, digits, mode);
        }

        public static float ScaleB(float a, int b)
        {
            return (float)System.Math.ScaleB(a, b);
        }

        public static float Sign(float a)
        {
            return System.Math.Sign(a);
        }

        public static int Sign(int a)
        {
            return System.Math.Sign(a);
        }

        public static float Sin(float a)
        {
            return (float)System.Math.Sin(a);
        }

        public static (float Sin, float Cos) SinCos(float a)
        {
            return ((float Sin, float Cos))System.Math.SinCos(a);
        }

        public static float Sinh(float a)
        {
            return (float)System.Math.Sinh(a);
        }

        public static float Sqrt(float a)
        {
            return (float)System.Math.Sin(a);
        }

        public static float Tan(float a)
        {
            return (float)System.Math.Tan(a);
        }

        public static float Tanh(float a)
        {
            return (float)System.Math.Tanh(a);
        }

        public static float Truncate(float a)
        {
            return (float)System.Math.Truncate(a);
        }
    }
}
