using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<string, Queue<GameObject>> objectsPool = new Dictionary<string, Queue<GameObject>>();
    public static ObjectPool instance { get; private set; }

    private const int maxPoolSize = 30;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this);
    }


    public GameObject GetObject(GameObject obj, bool active = false, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        if (objectsPool.TryGetValue(obj.name, out Queue<GameObject> objectList))
        {
            if (objectList.Count == 0)
                return CreateNewObject(obj, active, position, rotation, parent);

            else
            {
                GameObject requestedObject = objectList.Dequeue();
                requestedObject.SetActive(active);
                requestedObject.transform.position = position;
                requestedObject.transform.rotation = rotation;
                requestedObject.transform.parent = parent;
                return requestedObject;
            }
        }

        else
            return CreateNewObject(obj, active, position, rotation, parent);
    }

    GameObject CreateNewObject(GameObject obj, bool active = false, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
    {
        GameObject newObject = Instantiate(obj, position, rotation, parent);
        newObject.name = obj.name;
        newObject.SetActive(active);
        return newObject;
    }

    public void ReturnToPool(GameObject obj, float killTime = 0, System.Action Reset = null)
    {
        Reset?.Invoke();
        if (killTime == 0)
        {
            HideAndStoreObject(obj);
        }
        else
            StartCoroutine(DelayDestroy(obj, killTime));

    }

    IEnumerator DelayDestroy(GameObject obj, float killTime)
    {
        yield return new WaitForSeconds(killTime);

        HideAndStoreObject(obj);

    }
    private void HideAndStoreObject(GameObject obj)
    {
        if (obj == null)
            return;

        obj.SetActive(false);
        obj.transform.SetParent(null);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        if (objectsPool.TryGetValue(obj.name, out Queue<GameObject> objectList) && objectList.Count < maxPoolSize)
        {
            objectList.Enqueue(obj);
        }

        else
        {
            Queue<GameObject> newObjectQueue = new Queue<GameObject>();
            newObjectQueue.Enqueue(obj);
            objectsPool.Add(obj.name, newObjectQueue);
        }
    }
}
