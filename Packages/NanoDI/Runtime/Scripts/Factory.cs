namespace NanoDI
{
	public sealed class Factory<T> : IFactory<T> where T : IInitializable, new()
	{
		private readonly Container container;

		public Factory(Container container)
		{
			this.container = container;
		}

		public T Create()
		{
			T obj = new T();
			container.Inject(obj);
			obj.Initialize();
			return obj;
		}
	}
}
