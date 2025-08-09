using NanoDI;
using UnityEngine;

namespace GamePlay
{
	[CreateAssetMenu(menuName = "GamePlay/GamePlayRefs")]
	public class GamePlayRefs : ReferenceBinder
	{
		public override void BindReferences(Container container)
		{
			Debug.Log("Binding GamePlay references...");
			container.BindNew<PlayerController>();
			container.BindNew<UIManager>();
			container.BindNew<GameManager>();
		}
	}
}
