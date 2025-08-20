using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NanoDI
{
	public sealed class Container
	{
		private readonly Dictionary<Type, object> instances = new();
		private readonly Dictionary<Type, FieldInfo[]> injFieldCache = new();
		private readonly Dictionary<Type, Action<object, Container>> compiledInjectors = new();

		const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Bind an existing, fully constructed instance. Does NOT inject or initialize.
		/// Duplicate binds are illegal and throw.
		/// </summary>
		public void Bind<T>(T instance)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			Type key = typeof(T);
			if (instances.ContainsKey(key))
				throw new InvalidOperationException($"[Container] Duplicate bind for {key}. This is likely a mistake.");
			instances[key] = instance!;
		}

		/// <summary>
		/// Create a new instance via parameterless constructor (new T()), then perform FIELD injection,
		/// ALWAYS call Initialize() if implemented, and finally store it.
		/// Throws if a dependency is missing or T already bound.
		/// </summary>
		public T BindNew<T>() where T : new()
		{
			Type key = typeof(T);
			if (instances.ContainsKey(key))
				throw new InvalidOperationException($"[Container] Duplicate bind for {key}. This is likely a mistake.");

			T instance = new T();
			Inject(instance); // throws if missing deps
			if (instance is IInitializable init) init.Initialize();
			instances[key] = instance;
			return instance;
		}

		/// <summary>
		/// Inject FIELD dependencies into an arbitrary object. Throws if any dependency is missing.
		/// </summary>
		internal void Inject(object instance)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			Type type = instance.GetType();
			
			// Try compiled injector first (fastest path)
			if (compiledInjectors.TryGetValue(type, out Action<object, Container> compiledInjector))
			{
				compiledInjector(instance, this);
				return;
			}
			
			FieldInfo[] fields = GetInjectFields(type);

			// Create compiled injector for future use
			if (fields.Length > 0)
			{
				Action<object, Container> injector = CreateCompiledInjector(type, fields);
				compiledInjectors[type] = injector;
				injector(instance, this);
			}
		}

		private FieldInfo[] GetInjectFields(Type type)
		{
			if (injFieldCache.TryGetValue(type, out var cached))
				return cached;

			List<FieldInfo> list = new List<FieldInfo>(8);
			Type t = type;
			while (t != null && t != typeof(object))
			{
				FieldInfo[] fields = t.GetFields(FLAGS);
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo f = fields[i];
					if (Attribute.IsDefined(f, typeof(InjectAttribute), inherit: true))
						list.Add(f);
				}
				t = t.BaseType;
			}

			FieldInfo[] arr = list.Count == 0 ? Array.Empty<FieldInfo>() : list.ToArray();
			injFieldCache[type] = arr;
			return arr;
		}

		public void BindFactory<T>(T prefab) where T : Component, IInitializable
		{
			if (instances.ContainsKey(typeof(PrefabFactory<T>)))
				throw new InvalidOperationException($"[Container] Duplicate bind for PrefabFactory<{typeof(T)}>.");

			PrefabFactory<T> factory = new PrefabFactory<T>(this, prefab);
			instances[typeof(PrefabFactory<T>)] = factory;
		}

		public void BindFactory<T>() where T : IInitializable, new()
		{
			if (instances.ContainsKey(typeof(Factory<T>)))
				throw new InvalidOperationException($"[Container] Duplicate bind for Factory<{typeof(T)}>.");

			Factory<T> factory = new Factory<T>(this);
			instances[typeof(Factory<T>)] = factory;
		}

		public void ClearBindings()
		{
			instances.Clear();
			injFieldCache.Clear();
			compiledInjectors.Clear();
		}

		private Action<object, Container> CreateCompiledInjector(Type type, FieldInfo[] fields)
		{
			// Simple delegate compilation for better performance
			return (instance, container) =>
			{
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo f = fields[i];
					Type depType = f.FieldType;
					if (!container.instances.TryGetValue(depType, out object dep))
						throw new InvalidOperationException($"[Container] Missing binding for dependency {depType} required by {type}.{f.Name}");
					f.SetValue(instance, dep);
				}
			};
		}
	}
}