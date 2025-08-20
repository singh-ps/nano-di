using UnityEngine;

namespace NanoDI
{
	public interface IPrefabFactory<T> where T : Component, IInitializable
	{
		T Create(Transform parent, string name, Vector3 position, Quaternion rotation);
	}
}
