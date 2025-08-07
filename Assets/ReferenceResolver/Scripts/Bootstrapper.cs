using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ReferenceResolver
{
	[CreateAssetMenu(menuName = "Reference Resolver/Bootstrapper")]
	public class Bootstrapper: ScriptableObject
	{
		[SerializeField] private List<ReferenceBinder> definitions = new();
		[SerializeField] private bool enabled = false;

		private Container container;

		public void Bootstrap()
		{
			if (!enabled)
				return;

			container = new Container();

			for (int i = 0; i < definitions.Count; i++)
			{
				definitions[i].BindReferences(container);
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Initialize()
		{
			Bootstrapper[] bootstrappers = Resources.LoadAll<Bootstrapper>("");
			if (bootstrappers == null || bootstrappers.Length < 1)
			{
				throw new System.Exception("[Bootstrapper] No bootstrapper found");
			}

			Bootstrapper[] enabledBootstrappers = bootstrappers.Where(b => b.enabled).ToArray();

			if (enabledBootstrappers.Length == 0)
			{
				Debug.LogWarning("[Bootstrapper] No enabled bootstrappers found.");
				return;
			}

			if (enabledBootstrappers.Length > 1)
			{
				string names = string.Join(", ", enabledBootstrappers.Select(b => b.name));
				throw new System.Exception($"[Bootstrapper] Multiple enabled bootstrappers found: {names}. Only one is allowed.");
			}

			enabledBootstrappers[0].Bootstrap();
		}
	}
}