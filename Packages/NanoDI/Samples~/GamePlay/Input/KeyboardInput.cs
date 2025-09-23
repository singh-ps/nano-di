using NanoDI;

namespace GamePlay
{
	public class KeyboardInput : IInputProvider, IInitializable
	{
		public void Initialize()
		{
		}

		public string GetLatestInput()
		{
			return "Keyboard input received";
		}
	}
}
