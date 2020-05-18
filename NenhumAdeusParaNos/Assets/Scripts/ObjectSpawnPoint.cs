using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnPoint : MonoBehaviour
{
    [SerializeField] bool used = false;
    public bool Used { get { return used; } }

    public void SpawnObject(GameObject obj)
    {
        used = true;
        Instantiate(obj, transform.position, transform.rotation);
    }
}
