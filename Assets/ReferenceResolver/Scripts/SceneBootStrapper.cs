using UnityEngine;

namespace ReferenceResolver
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
