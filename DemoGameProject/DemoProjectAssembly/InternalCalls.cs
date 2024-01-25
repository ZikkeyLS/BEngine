using BEngineScripting;

namespace DemoProjectAssembly
{
	public class InternalCalls
	{
		public void Test()
		{
			BEngine.InternalCalls.Log("Demo message from Scripting");
		}
	}

	public class TestScript : Script
	{

	}
}
