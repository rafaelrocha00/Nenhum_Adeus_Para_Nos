using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInsideRoom : MonoBehaviour
{

    [SerializeField] public GameObject thisRoom;
    [SerializeField] public GameObject oldRoom;
    void OnTriggerEnter(Collider other)
    {
        oldRoom.SetActive(false);
        thisRoom.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!thisRoom.activeInHierarchy)
        {
            thisRoom.SetActive(true);
          
        }
    }

  
}
