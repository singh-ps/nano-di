using UnityEngine;

namespace NanoDI
{
	public interface IPrefabFactory<T>
	{
		T Create(Transform parent, string name, Vector3 position, Quaternion rotation);
	}
}
