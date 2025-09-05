using NanoDI;
using UnityEngine;

namespace GamePlay
{
	public class BaseEnemy : MonoBehaviour, IInitializable
	{
		[Inject] protected EnemyStats stats;
		[Inject] private readonly GameAnalytics analytics;

		public void Initialize()
		{
			
		}

		public virtual void Attack()
		{
			analytics.LogEvent($"{gameObject.name}");
		}
	}
}
