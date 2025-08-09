using UnityEngine;

namespace NanoDI
{
	public abstract class CompositionRoot: ScriptableObject
	{
		public abstract void Compose(Container container);
	}
}