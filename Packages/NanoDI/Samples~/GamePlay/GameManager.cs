using NanoDI;
using UnityEngine;

namespace GamePlay
{
	public class GameManager: IInitializable
	{
		[Inject] private readonly UIManager uiManager;
		[Inject] readonly private PrefabFactory<Player> playerFactory;
		[Inject] private Factory<EnemySystem> enemySystemFactory;

		private EnemySystem enemySystem;

		public void Initialize()
		{
			uiManager.HideUI("MainMenu");
			uiManager.ShowUI("GameHUD");
			
			enemySystem = enemySystemFactory.Create();

			Player player = playerFactory.Create(null, "Player", Vector3.zero, Quaternion.identity);
			player.TakeDamage(10);
			player.FireWeapon(); 

			enemySystem.TickEnemies();
		}
	}
}
