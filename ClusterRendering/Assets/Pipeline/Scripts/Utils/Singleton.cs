using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class SingletonBase
    {
        protected static List<SingletonBase> s_lstSingle = new List<SingletonBase>();
        public static void DestroyAll()
        {
            for (int i = s_lstSingle.Count - 1; i >= 0; --i)
            {
                SingletonBase s = s_lstSingle[i];
                s.Destroy();
            }
            if (s_lstSingle.Count != 0)
            {
                Debug.LogError("s_lst.Count != 0");
            }
            s_lstSingle.Clear();
        }

        virtual public void Init()
        {

        }

        virtual protected void OnDestroy()
        {

        }

        virtual public void Destroy()
        {
        }

    }

    public abstract class Singleton<T>:SingletonBase where T : SingletonBase, new()
    {
        private static T _instance = null;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.Init();
                    s_lstSingle.Add(_instance);
                }
                return _instance;
            }
        }

        override public void Destroy()
        {
            OnDestroy();
            s_lstSingle.Remove(_instance);
            Debug.Log("--------------- " + _instance.GetType().ToString());
            _instance = null;
        }
    }
}