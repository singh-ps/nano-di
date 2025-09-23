using NanoDI;
using UnityEngine;

namespace GamePlay
{
	public class Player : MonoBehaviour, IInitializable
	{
		[Inject] private PlayerStats playerStats;
		[Inject] private PrefabFactory<Weapon> weaponFactory;
		[Inject] private IInputProvider inputProvider;

		private Weapon weapon;

		public void Initialize()
		{
			Debug.Log("Initializing Player");
			weapon = weaponFactory.Create(null, "Weapon", Vector3.zero, Quaternion.identity);

			inputProvider.GetLatestInput();
		}

		public void TakeDamage(int damage)
		{
			Debug.Log($"Player took {damage} damage. Current health: {playerStats.Health}");
		}

		public void FireWeapon()
		{
			weapon.FireWeapon();
		}
	}
}
