using NanoDI;
using UnityEngine;

namespace GamePlay
{
	[CreateAssetMenu(menuName = "GamePlay/Composition Root")]
	public class GamePlayCR : CompositionRoot
	{
		[SerializeField] private bool useGamePad;

		[SerializeField] private MeleeEnemy meleeEnemyPrefab;
		[SerializeField] private RangedEnemy rangedEnemyPrefab;

		[SerializeField] private Player playerPrefab;
		[SerializeField] private Weapon weaponPrefab;

		public override void Compose(Container container)
		{
			container.BindNew<GameAnalytics>();
			
			container.BindNew<EnemyStats>();
			container.BindFactory(meleeEnemyPrefab);
			container.BindFactory(rangedEnemyPrefab);
			container.BindFactory<EnemySystem>();

			IInputProvider inputProvider = useGamePad? new GamepadInput() : new KeyboardInput();
			container.Bind(inputProvider);

			container.BindFactory(playerPrefab);
			container.BindFactory(weaponPrefab);


			container.BindNew<PlayerStats>();
			container.BindNew<WeaponStats>();
			container.BindNew<UIManager>();
			container.BindNew<GameManager>();

			// DEMO: This would throw - duplicate binding (uncomment to test)
			// container.BindNew<PlayerStats>();
			// container.BindNew<PlayerStats>(); // InvalidOperationException
		}
	}
}
