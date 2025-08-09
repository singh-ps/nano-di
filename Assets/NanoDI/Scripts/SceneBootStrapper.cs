using UnityEngine;

namespace NanoDI
{
	[ExecuteAlways]
	public class SceneBootStrapper : MonoBehaviour
	{
		[SerializeField] private Bootstrapper bootstrapper;

		void Awake()
		{
			if (bootstrapper != null)
			{
				bootstrapper.Bootstrap();
			}
		}
	}
}
