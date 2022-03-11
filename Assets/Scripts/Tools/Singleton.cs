public class Singleton<T> where T : new()
{
	private static T instance;
	public static T GetT()
	{

		if (instance == null)
		{
			instance = new T();
		}
		return instance;

	}

}
