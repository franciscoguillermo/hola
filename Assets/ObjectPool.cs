using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool _instance;

    public static ObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectPool>();
            }
            return _instance;
        }

        private set { }
    }

    Dictionary<GameObject, Stack<GameObject>> _objectPool = new Dictionary<GameObject, Stack<GameObject>>();

    /// <summary>
    /// Populate the Dictionary from the Resources/Pooled folder in project.
    /// </summary>
    void Awake()
    {
        foreach (GameObject prefab in Resources.LoadAll<GameObject>("Pooled"))
        {
            _objectPool.Add(prefab, new Stack<GameObject>());
        }

        foreach (KeyValuePair<GameObject, Stack<GameObject>> pair in _objectPool)
        {
            int poolSize = pair.Key.GetComponent<PooledObject>().DefaultPoolSize;

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newGameObject = GameObject.Instantiate(pair.Key) as GameObject;
                newGameObject.name = pair.Key.name;
                newGameObject.GetComponent<PooledObject>().Prefab = pair.Key;

                Push(newGameObject);
            }
        }
    }

    /// <summary>
    /// Pops an object from the ObjectPool.
    /// 
    /// The state of the object is unreliable. Should not make assumptions about
    /// its inital properties from the prefab, it's transform parent, or even the
    /// isActive state of the gameobject.
    /// </summary>
    public static GameObject Pop(GameObject prefab)
    {
        if (Instance._objectPool[prefab].Count > 0)
        {
            GameObject obj = Instance._objectPool[prefab].Pop();
            obj.GetComponent<PooledObject>().Pop();
            return obj;
        }
        else
        {
            GameObject newGameObject = Instantiate(prefab) as GameObject;
            newGameObject.name = prefab.name;
            newGameObject.GetComponent<PooledObject>().Prefab = prefab;
            newGameObject.GetComponent<PooledObject>().Pop();

            return newGameObject;
        }
    }

    /// <summary>
    /// Generic version of Pop, saves writing GetComponent on quick pool accesses.
    /// </summary>
    public static T Pop<T>(GameObject prefab)
    {
        return Pop(prefab).GetComponent<T>();
    }

    /// <summary>
    /// Pushes an object back to the Object Pool.
    /// 
    /// The Pool disables the object and parents it to it's transform in the scene.
    /// </summary>
    public static void Push(PooledObject pooled)
    {
        GameObject go = pooled.gameObject;

        Instance._objectPool[pooled.Prefab].Push(go);
        go.transform.SetParent(Instance.transform);
        go.SetActive(false);
    }

    /// <summary>
    /// Generic version of push, allows Push(this) on MonoBehaviours.
    /// </summary>
    public static void Push<T>(T component) where T : MonoBehaviour
    {
        Push(component.GetComponent<PooledObject>());
    }

    public static void Push(GameObject go)
    {
        Push(go.GetComponent<PooledObject>());
    }
}