using NanoDI;
using UnityEngine;

namespace GamePlay
{
	[CreateAssetMenu(menuName = "GamePlay/Composition Root")]
	public class GamePlayCR : CompositionRoot
	{
		public override void Compose(Container container)
		{
			container.BindNew<PlayerController>();
			container.BindNew<UIManager>();
			container.BindNew<GameManager>();
		}
	}
}
