using UnityEngine;

namespace ReferenceResolver
{
	public abstract class ReferenceBinder: ScriptableObject
	{
		public abstract void BindReferences(Container container);
	}
}