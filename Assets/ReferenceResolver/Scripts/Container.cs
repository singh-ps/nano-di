using System.Collections.Generic;
using System;

namespace ReferenceResolver
{
	public class Container
	{
		private readonly Dictionary<Type, object> references = new();

		public void BindNew<T>() where T : new()
		{
			object newRef = new T();
			references.Add(typeof(T), newRef);
		}

		public void Bind<T>(T instance)
		{
			references.Add(typeof(T), instance);
		}

		public T Get<T>() => (T)references[typeof(T)];

		public void ClearBindings()
		{
			references.Clear();
		}
	}
}