namespace NanoDI
{
	public interface IFactory<T> where T : IInitializable, new()
	{
		T Create();
	}
}
