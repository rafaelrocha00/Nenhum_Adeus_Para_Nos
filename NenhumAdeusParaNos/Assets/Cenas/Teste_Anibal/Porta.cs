using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porta : MonoBehaviour
{
        void Update()
    {
        GetComponent<Rigidbody>().WakeUp();
    }
}
