using UnityEngine;

namespace NanoDI
{
	public abstract class ReferenceBinder: ScriptableObject
	{
		public abstract void BindReferences(Container container);
	}
}