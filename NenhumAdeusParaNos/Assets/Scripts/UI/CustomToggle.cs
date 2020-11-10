using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    public GameObject checkMark;
    Button button;

    [SerializeField] bool on = false;
    public bool On { get { return on; } set { on = value; } }

    [SerializeField] bool interactable = true;
    public bool Interactable { set { interactable = value; if (button == null) button = GetComponent<Button>(); button.interactable = value; } }

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Switch()
    {
        on = !on;
        checkMark.SetActive(on);

        Interactable = false;
    }

    public void Switch(bool value)
    {
        if (value) Interactable = false;

        on = value;
        checkMark.SetActive(value);
    }
}
