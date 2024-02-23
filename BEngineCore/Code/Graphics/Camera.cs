using System.Numerics;

namespace BEngineCore
{
	internal class Camera
	{
		// This looks like base for transform sort of things

		public Vector3 position = Vector3.Zero;
		public Vector3 rotation = Vector3.Zero;

		public Vector3 forward { get; private set; }
		public Vector3 up { get; private set; }
		public Vector3 right { get; private set; }

		public float fov = 60.0f;
		public float minRange = 0.01f;
		public float maxRange = 100f;

		public void Recalculate()
		{
			Quaternion resultRotation = Quaternion.CreateFromYawPitchRoll(
				float.DegreesToRadians(rotation.X), 
				float.DegreesToRadians(rotation.Y), 
				float.DegreesToRadians(rotation.Z));

			forward = Vector3.Transform(Vector3.UnitZ, resultRotation);
			up = Vector3.Transform(Vector3.UnitY, resultRotation); 
			right = Vector3.Transform(Vector3.UnitX, resultRotation);
		}

		public Matrix4x4 CalculateViewMatrix()
		{
			return Matrix4x4.CreateLookAt(position, position + forward, up);
		}

		public Matrix4x4 CalculateProjectionMatrix(float width, float height)
		{
			return Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(fov),
					width / height,minRange, maxRange);
		}
	}
}
