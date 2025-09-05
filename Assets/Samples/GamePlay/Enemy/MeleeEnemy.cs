using UnityEngine;

namespace GamePlay
{
	public class MeleeEnemy : BaseEnemy
	{
		[SerializeField] private int attackMultiplier = 2;

		public override void Attack()
		{
			int damage = stats.BaseDamage * attackMultiplier;
			Debug.Log($"MeleeEnemy attacks for {damage} damage!");
		}
	}
}
