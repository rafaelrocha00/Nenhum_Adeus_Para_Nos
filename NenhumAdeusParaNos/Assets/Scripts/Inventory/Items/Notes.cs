using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Notes", menuName = "Item/Notes")]
public class Notes : QuickUseItem
{
    [TextArea] public string[] texts = new string[10];
    //public string[] Texts { get { return texts; } set { texts = value; } }
    public int maxChar = 340;

    public override bool Effect()
    {        
        GameManager.gameManager.MainHud.OpenCloseNotesMenu();
        WriteText();
        return false;
    }

    public void WriteText()
    {
        GameManager.gameManager.MainHud.WriteNotes(texts);
    }

    public void AddNote(string n)
    {
        int page = 0;
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].Length + n.Length < maxChar)
            {
                page = i;
                break;
            }
        }
        texts[page] += "- " + n + "\n\n";

        //WriteText();
    }

    private void OnDisable()
    {
        texts = new string[10];
    }
}
