using UnityEngine;

namespace GamePlay
{
	public class PlayerController
	{
		public PlayerController() {}

		public void SpawnPlayer(string playerName)
		{
			Debug.Log($"Spawning player: {playerName}");
		}

		public void MovePlayer(Vector3 direction)
		{
			Debug.Log($"Moving player in direction: {direction}");
		}

		public void KillPlayer()
		{
			Debug.Log("Player has been killed.");
		}
	}
}
