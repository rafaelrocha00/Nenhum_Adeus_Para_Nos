using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotepadNotes : MonoBehaviour
{
    Text note;
    Text erasure;

    [HideInInspector] bool written = false;
    public bool Written { get { return written; } }

    [HideInInspector] Quest quest;
    public Quest Quest { get { return quest; } set { quest = value; } }

    public void Write(string txt, Quest q)
    {
        GetTexts();

        note.text = txt;
        quest = q;

        written = true;
    }

    public void PutErasure()
    {
        erasure.text = new string('_', note.text.Length + Mathf.Clamp(note.text.Length, 0, 1) * 5);
    }

    void GetTexts()
    {
        if (note != null) return;

        note = GetComponent<Text>();
        erasure = transform.GetChild(0).GetComponent<Text>();
    }

    public void EraseAll()
    {
        GetTexts();

        note.text = "";
        erasure.text = "";
    }
}
