
namespace BEngine
{
	public static class Project
	{
		public static bool IsEditor => InternalCalls.ProjectIsRuntime();
		public static bool IsRuntime => InternalCalls.ProjectIsEditor();
	}
}
