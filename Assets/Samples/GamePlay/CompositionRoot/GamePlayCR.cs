using NanoDI;
using UnityEngine;

namespace GamePlay
{
	[CreateAssetMenu(menuName = "GamePlay/Composition Root")]
	public class GamePlayCR : CompositionRoot
	{
		[SerializeField] private Player playerPrefab;
		[SerializeField] private Weapon weaponPrefab;

		public override void Compose(Container container)
		{
			container.CreateFactory(playerPrefab);
			container.CreateFactory(weaponPrefab);


			container.BindNew<PlayerStats>();
			container.BindNew<WeaponStats>();
			container.BindNew<UIManager>();
			container.BindNew<GameManager>();
		}
	}
}
