using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSave : MonoBehaviour
{
    [SerializeField]GameObject FootCollisor;
    Vector3 lastPosition;
    int layerMask = 0;
    int water = 0;
    bool emTerra = true;
    Player player;

    private void Start()
    {
        layerMask =~ LayerMask.GetMask("ColisorAgua");
        water = LayerMask.GetMask("ColisorAgua");
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f, layerMask))
        {
            lastPosition = transform.position;
            if (!emTerra)
            {
                emTerra = true;
                player.CanMove = true;
            }
        }
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f, water))
        {
            transform.position = lastPosition;
            emTerra = false;
        }

        if (!emTerra)
        {
            player.CanMove = false;
        }
    }

    public void Voltar()
    {
        transform.position = lastPosition;
    }

}
