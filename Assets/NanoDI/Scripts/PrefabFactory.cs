using UnityEngine;
using System.Collections.Generic;

namespace NanoDI
{
	public sealed class PrefabFactory<T> : IPrefabFactory<T> where T : Component, IInitializable
	{
		private readonly Container container;
		private readonly T prefab;

		private List<MonoBehaviour> buffer = new List<MonoBehaviour>(128);

		public PrefabFactory(Container container, T prefab)
		{
			this.container = container;
			this.prefab = prefab;
		}

		public T Create(Transform parent, string name, Vector3 position, Quaternion rotation)
		{
			if (prefab == null)
			{
				Debug.LogError("Prefab is null");
				return null;
			}
			
			GameObject go = Object.Instantiate(prefab.gameObject, position, rotation, parent);
			if (!string.IsNullOrEmpty(name))
			{
				go.name = name;
			}
			T component = go.GetComponent<T>();

			buffer.Clear();
			go.GetComponentsInChildren(true, buffer);
			for (int i = 0; i < buffer.Count; i++)
			{
				MonoBehaviour mb = buffer[i];
				if (mb == null)
				{
					continue;
				}
				container.Inject(mb);
				if (mb is IInitializable init)
				{
					init.Initialize();
				}
			} 
			return component;
		}
	}
}
