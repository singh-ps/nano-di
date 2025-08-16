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

		const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

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
			FieldInfo[] fields = GetInjectFields(type);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo f = fields[i];
				Type depType = f.FieldType;
				if (!instances.TryGetValue(depType, out object dep))
					throw new InvalidOperationException($"[Container] Missing binding for dependency {depType} required by {type}.{f.Name}");
				f.SetValue(instance, dep);
			}
		}

		private FieldInfo[] GetInjectFields(Type type)
		{
			if (injFieldCache.TryGetValue(type, out FieldInfo[] cached))
				return cached;

			
			FieldInfo[] all = type.GetFields(FLAGS);
			List<FieldInfo> list = new List<FieldInfo>(all.Length);
			for (int i = 0; i < all.Length; i++)
			{
				FieldInfo f = all[i];
				if (f.IsDefined(typeof(InjectAttribute), inherit: true))
					list.Add(f);
			}

			var arr = list.Count == 0 ? Array.Empty<FieldInfo>() : list.ToArray();
			injFieldCache[type] = arr;
			return arr;
		}

		public PrefabFactory<T> CreateFactory<T>(T prefab) where T : Component, IInitializable
		{
			PrefabFactory <T> factory = new PrefabFactory<T>(this, prefab);
			instances[typeof(PrefabFactory<T>)] = factory;
			return factory;
		}

		public Factory<T> CreateFactory<T>() where T : IInitializable, new()
		{
			Factory<T> factory = new Factory<T>(this);
			instances[typeof(Factory<T>)] = factory;
			return factory;
		}

		public void ClearBindings()
		{
			instances.Clear();
		}

		/// <summary>
		/// Inject FIELD dependencies for all MonoBehaviours in a GameObject hierarchy.
		/// Calls Initialize() on components that implement IInitializable.
		/// Throws if any dependency is missing.
		/// </summary>
		internal void InjectGameObject(GameObject go, bool callInitialize = true)
		{
			if (go == null) return;
			var behaviours = go.GetComponentsInChildren<MonoBehaviour>(true);
			for (int i = 0; i < behaviours.Length; i++)
			{
				var mb = behaviours[i];
				if (mb == null) continue; // In case of missing scripts
				Inject(mb);
				if (callInitialize && mb is IInitializable init) init.Initialize();
			}
		}
	}
}