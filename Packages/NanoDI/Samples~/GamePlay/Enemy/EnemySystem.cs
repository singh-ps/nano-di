using NanoDI;
using UnityEngine;
using System.Collections.Generic;

namespace GamePlay
{
	public class EnemySystem: IInitializable
	{
		private int meleeEnemyCount = 5;
		[Inject] private PrefabFactory<MeleeEnemy> meleeEnemyFactory;

		private int rangedEnemyCount = 3;
		[Inject] private PrefabFactory<RangedEnemy> rangedEnemyFactory;

		private List<BaseEnemy> enemies = new List<BaseEnemy>();

		public void Initialize()
		{
			for (int i = 0; i < meleeEnemyCount; i++)
			{
				Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
				MeleeEnemy enemy = meleeEnemyFactory.Create(null, $"MeleeEnemy[{i}]", randomPos, Quaternion.identity);
				enemies.Add(enemy);

			}
			for (int i = 0; i < rangedEnemyCount; i++)
			{
				Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
				RangedEnemy enemy = rangedEnemyFactory.Create(null, $"RangedEnemy[{i}]", randomPos, Quaternion.identity);
				enemies.Add(enemy);
			}
		}

		public void TickEnemies()
		{
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Attack();
			}
		}
	}
}
