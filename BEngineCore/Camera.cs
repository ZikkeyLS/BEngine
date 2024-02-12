using System.Numerics;

namespace BEngineCore
{
	internal class Camera
	{
		public Vector3 position = Vector3.Zero;
		public Vector3 rotation = Vector3.Zero;


		public Vector3 forward { get; private set; } = Vector3.UnitZ * -1f;
		public Vector3 up { get; } = Vector3.UnitY;

		public float fov = 45.0f;

		public void Recalculate()
		{
			if (rotation.Y > 89.0f)
				rotation.Y = 89.0f;
			if (rotation.Y < -89.0f)
				rotation.Y = -89.0f;

			forward = Vector3.Transform(Vector3.UnitZ, Quaternion.
				CreateFromYawPitchRoll(-rotation.X,
				rotation.Y, rotation.Z));
		}

		public Matrix4x4 CalculateViewMatrix()
		{
			return Matrix4x4.CreateLookAt(position, position + forward, up);
		}

		public Matrix4x4 CalculateProjectionMatrix(float width, float height)
		{
			return Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(fov),
					width / height, 0.1f, 100.0f);
		}
	}
}
