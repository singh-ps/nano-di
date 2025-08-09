using NanoDI;

namespace GamePlay
{
	public class GameManager: IInitializable
	{
		[Inject] private readonly PlayerController playerController;
		[Inject] private readonly UIManager uiManager;

		public void Initialize()
		{
			uiManager.HideUI("MainMenu");
			playerController.SpawnPlayer("Player One");
			uiManager.ShowUI("GameHUD");
		}
	}
}
