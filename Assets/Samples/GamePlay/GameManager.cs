using NanoDI;
using UnityEngine;

namespace GamePlay
{
	public class GameManager: IInitializable
	{
		[Inject] private readonly UIManager uiManager;
		[Inject] readonly private PrefabFactory<Player> playerFactory;

		public void Initialize()
		{
			uiManager.HideUI("MainMenu");
			uiManager.ShowUI("GameHUD");
			
			Player player = playerFactory.Create(null, "Player", Vector3.zero, Quaternion.identity);
			player.TakeDamage(10);
			player.FireWeapon(); 
		}
	}
}
