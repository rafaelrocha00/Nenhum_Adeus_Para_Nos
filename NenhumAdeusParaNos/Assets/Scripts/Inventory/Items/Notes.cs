using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Notes", menuName = "Item/Notes")]
public class Notes : QuickUseItem
{
    //[TextArea] public string[] texts = new string[10];
    //public string[] strokes = new string[10];
    //public string[] Texts { get { return texts; } set { texts = value; } }
    //public int maxChar = 340;

    [SerializeField] bool main = false;
    public bool Main { get { return main; } set { main = value; } }

    public List<string> texts = new List<string>();
    public List<Quest> quests = new List<Quest>();
    //public List<string> strokes = new List<string>();

    public int maxNotes = 100;

    public override bool Effect()
    {        
        GameManager.gameManager.MainHud.OpenCloseNotesMenu();
        WriteText();
        return false;
    }

    public void WriteText()
    {
        GameManager.gameManager.MainHud.WriteNotes(texts, quests, main, this);
    }

    public void AddNote(string n, Quest q)
    {
        //int page = 0;
        //for (int i = 0; i < texts.Length; i++)
        //{
        //    if (texts[i].Length + n.Length < maxChar)
        //    {
        //        page = i;
        //        break;
        //    }
        //}
        ////strokes[page] += new string('_', texts[page].Length);
        //texts[page] += "- " + n + "\n\n";

        //WriteText();

        if (texts.Count >= maxNotes) return;

        texts.Add(n);
        quests.Add(q);
    }

    private void OnDisable()
    {
        //texts = new string[10];
        //strokes = new string[10];
        texts = new List<string>();
        quests = new List<Quest>();
        main = false;
    }
}
