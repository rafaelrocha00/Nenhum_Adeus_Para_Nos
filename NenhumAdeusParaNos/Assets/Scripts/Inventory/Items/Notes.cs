using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Notes", menuName = "Item/Notes")]
public class Notes : QuickUseItem
{
    public override bool Effect()
    {
        GameManager.gameManager.MainHud.OpenCloseNotesMenu();
        return false;
    }
}
