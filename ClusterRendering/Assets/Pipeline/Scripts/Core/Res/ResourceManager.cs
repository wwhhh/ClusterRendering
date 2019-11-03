using Framework;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{

    public T Load<T>(string name) where T : Object
    {
        T t = Resources.Load(name) as T;
        return t;
    }

}