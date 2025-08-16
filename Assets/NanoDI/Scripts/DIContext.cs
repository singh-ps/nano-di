using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NanoDI
{
	[CreateAssetMenu(menuName = "NanoDI/DIContext")]
	public class DIContext: ScriptableObject
	{
		[SerializeField] private List<CompositionRoot> compositions = new();
		[SerializeField] private bool enabled = false;

		private Container container;

		public void Create()
		{
			if (!enabled)
				return;

			container = new Container();

			int count = compositions.Count;
			for (int i = 0; i < count; i++)
			{
				compositions[i].Compose(container);
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void InitializeGlobalContexts()
		{
			DIContext[] ctx = Resources.LoadAll<DIContext>("");
			if (ctx == null || ctx.Length < 1)
			{
				throw new System.Exception("[DIContext] No global DI context found");
			}

			DIContext enabledContext = null;
			int enabledCount = 0;

			for (int i = 0; i < ctx.Length; i++)
			{
				if (ctx[i].enabled)
				{
					enabledCount++;
					if (enabledContext == null)
						enabledContext = ctx[i];
				}
			}

			if (enabledCount == 0)
			{
				Debug.LogWarning("[GlobalDIContext] Disabled global DI context found.");
				return;
			}

			if (enabledCount > 1)
			{
				StringBuilder errorMsg = new StringBuilder("[DIContext] Multiple enabled global DI contexts found: ");
				bool first = true;
				for (int i = 0; i < ctx.Length; i++)
				{
					if (ctx[i].enabled)
					{
						if (!first) errorMsg.Append(", ");
						errorMsg.Append(ctx[i].name);
						first = false;
					}
				}
				errorMsg.Append(". Only one is allowed.");
				throw new System.Exception(errorMsg.ToString());
			}

			enabledContext.Create();
		}
	}
}