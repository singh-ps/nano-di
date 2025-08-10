using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NanoDI
{
	public class Container
	{
		private readonly Dictionary<Type, object> references = new();

		public void BindNew<T>() where T : new()
		{
			object instance = (T)Resolve(typeof(T));
			references[typeof(T)] = instance;
		}

		public void Bind<T>(T instance)
		{
			references.Add(typeof(T), instance);
		}

		public T Get<T>() => (T)Resolve(typeof(T));

		public PrefabFactory<T> CreateFactory<T>(T prefab) where T : Component, IInitializable
		{
			PrefabFactory <T> factory = new PrefabFactory<T>(this, prefab);
			references[typeof(PrefabFactory<T>)] = factory;
			return factory;
		}

		public void ClearBindings()
		{
			references.Clear();
		}

		private object Resolve(Type instanceType)
		{
			if (references.TryGetValue(instanceType, out object instance))
				return instance;

			Type implType = instanceType.IsInterface || instanceType.IsAbstract
				? throw new InvalidOperationException($"No binding for {instanceType}")
				: instanceType;

			ConstructorInfo ctor = implType.GetConstructors()
				.FirstOrDefault(c => c.IsDefined(typeof(InjectAttribute), true))
				??
				implType.GetConstructors()
					.OrderByDescending(c => c.GetParameters().Length)
					.First();

			object[] args = ctor.GetParameters()
						   .Select(p => Resolve(p.ParameterType))
						   .ToArray();

			instance = ctor.Invoke(args);

			InjectMembers(instance);

			if (instance is IInitializable init)
			{
				init.Initialize();
			}

			references[instanceType] = instance;

			return instance;
		}

		private void InjectMembers(object instance)
		{
			Type type = instance.GetType();

			foreach (FieldInfo field in type
					 .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					 .Where(f => f.IsDefined(typeof(InjectAttribute), true)))
			{
				object value = Resolve(field.FieldType);
				field.SetValue(instance, value);
			}

			foreach (PropertyInfo prop in type
					 .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					 .Where(p => p.IsDefined(typeof(InjectAttribute), true)
								 && p.CanWrite))
			{
				object value = Resolve(prop.PropertyType);
				prop.SetValue(instance, value);
			}
		}

		public void InjectGameObject(GameObject go)
		{
			foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
			{
				InjectMembers(mb);
				if (mb is IInitializable init) init.Initialize();
			}
		}
	}
}