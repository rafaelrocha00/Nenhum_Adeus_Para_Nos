using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSave : MonoBehaviour
{
    [SerializeField]GameObject FootCollisor;
    Vector3 lastPosition;
    int layerMask = 0;
    int water = 0;

    private void Start()
    {
        layerMask =~ LayerMask.GetMask("ColisorAgua");
        water = LayerMask.GetMask("ColisorAgua");
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f, layerMask))
        {
            lastPosition = transform.position;
        }
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f, water))
        {
            transform.position = lastPosition;
        }
    }

    public void Voltar()
    {
        transform.position = lastPosition;
    }

}
