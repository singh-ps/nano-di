using UnityEngine;

namespace GamePlay
{
	public class GameAnalytics
	{
		public void LogEvent(string eventName)
		{
			Debug.Log($"Event: {eventName}");
		}
	}
}
