using NanoDI;

namespace GamePlay
{
	public class GamepadInput : IInputProvider, IInitializable
	{
		public void Initialize()
		{
		}

		public string GetLatestInput()
		{
			return "Gamepad input received";
		}
	}
}
