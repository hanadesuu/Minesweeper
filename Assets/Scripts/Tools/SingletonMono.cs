using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;
    public static T GetT()
    {
        return instance;
    }
    protected virtual void Awake()
    {
        instance = this as T;
    }


}
