using UnityEngine;

namespace GamePlay
{
	public class RangedEnemy : BaseEnemy
	{
		[SerializeField] private int rangeMultiplier = 2;

		private void ScanEnemy()
		{
			Debug.Log($"RangedEnemy scans for enemies within {stats.AttackRange * rangeMultiplier} units!");
		}

		public override void Attack()
		{
			ScanEnemy();
			int damage = stats.BaseDamage;
			Debug.Log($"RangedEnemy attacks for {damage} damage from a distance!");
			base.Attack();
		}	
	}
}
