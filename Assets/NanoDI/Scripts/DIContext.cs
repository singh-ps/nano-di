using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

			for (int i = 0; i < compositions.Count; i++)
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

			DIContext[] enabledCtx = ctx.Where(b => b.enabled).ToArray();

			if (enabledCtx.Length == 0)
			{
				Debug.LogWarning("[GlobalDIContext] Disabled global DI context found.");
				return;
			}

			if (enabledCtx.Length > 1)
			{
				string names = string.Join(", ", enabledCtx.Select(b => b.name));
				throw new System.Exception($"[DIContext] Multiple enabled global DI contexts found: {names}. Only one is allowed.");
			}

			enabledCtx[0].Create();
		}
	}
}