
namespace BEngine
{
	public static class Project
	{
		public static bool IsEditor => InternalCalls.ProjectIsEditor();
		public static bool IsRuntime => InternalCalls.ProjectIsRuntime();
	}
}
