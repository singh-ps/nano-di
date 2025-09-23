using UnityEngine;

namespace NanoDI
{
	[ExecuteAlways]
	public class SceneDIContext : MonoBehaviour
	{
		[SerializeField] private DIContext context;

		void Awake()
		{
			if (context != null)
			{
				context.Create();
			}
		}
	}
}
