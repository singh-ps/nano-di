using NanoDI;
using UnityEngine;

namespace GamePlay
{
	public class Weapon : MonoBehaviour, IInitializable
	{
		[Inject] private WeaponStats weaponStats;

		public void Initialize() { }

		public void FireWeapon()
		{
			Debug.Log($"Firing weapon with stats {weaponStats}");
		}
	}
}
