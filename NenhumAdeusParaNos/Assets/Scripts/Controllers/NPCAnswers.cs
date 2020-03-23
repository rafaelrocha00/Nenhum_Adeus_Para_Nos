using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnswers : MonoBehaviour
{
    #region DereDere
    public Dialogue[] failDereDere_seduce = new Dialogue[2];
    public Dialogue[] failDereDere_calmdown = new Dialogue[2];
    public Dialogue[] failDereDere_hurt = new Dialogue[2];
    public Dialogue[] failDereDere_mock = new Dialogue[2];
    public Dialogue[] failDereDere_cheerup = new Dialogue[2];
    #endregion
    #region Yandere
    public Dialogue[] failYandere_seduce = new Dialogue[2];
    public Dialogue[] failYandere_calmdown = new Dialogue[2];
    public Dialogue[] failYandere_hurt = new Dialogue[2];
    public Dialogue[] failYandere_mock = new Dialogue[2];
    public Dialogue[] failYandere_cheerup = new Dialogue[2];
    #endregion
    #region Tsundere
    public Dialogue[] failTsundere_seduce = new Dialogue[2];
    public Dialogue[] failTsundere_calmdown = new Dialogue[2];
    public Dialogue[] failTsundere_hurt = new Dialogue[2];
    public Dialogue[] failTsundere_mock = new Dialogue[2];
    public Dialogue[] failTsundere_cheerup = new Dialogue[2];
    #endregion
    #region Kuudere
    public Dialogue[] failKuudere_seduce = new Dialogue[2];
    public Dialogue[] failKuudere_calmdown = new Dialogue[2];
    public Dialogue[] failKuudere_hurt = new Dialogue[2];
    public Dialogue[] failKuudere_mock = new Dialogue[2];
    public Dialogue[] failKuudere_cheerup = new Dialogue[2];
    #endregion

    Dialogue[][] allDereDereFailAnswers = new Dialogue[5][];
    Dialogue[][] allYandereFailAnswers = new Dialogue[5][];
    Dialogue[][] allTsundereFailAnswers = new Dialogue[5][];
    Dialogue[][] allKuudereFailAnswers = new Dialogue[5][];

    private void Start()
    {
        #region Set_Dialogues
        allDereDereFailAnswers[0] = failDereDere_seduce;
        allDereDereFailAnswers[1] = failDereDere_calmdown;
        allDereDereFailAnswers[2] = failDereDere_hurt;
        allDereDereFailAnswers[3] = failDereDere_mock;
        allDereDereFailAnswers[4] = failDereDere_cheerup;

        allKuudereFailAnswers[0] = failKuudere_seduce;
        allKuudereFailAnswers[1] = failKuudere_calmdown;
        allKuudereFailAnswers[2] = failKuudere_hurt;
        allKuudereFailAnswers[3] = failKuudere_mock;
        allKuudereFailAnswers[4] = failKuudere_cheerup;

        allTsundereFailAnswers[0] = failTsundere_seduce;
        allTsundereFailAnswers[1] = failTsundere_calmdown;
        allTsundereFailAnswers[2] = failTsundere_hurt;
        allTsundereFailAnswers[3] = failTsundere_mock;
        allTsundereFailAnswers[4] = failTsundere_cheerup;

        allYandereFailAnswers[0] = failYandere_seduce;
        allYandereFailAnswers[1] = failYandere_calmdown;
        allYandereFailAnswers[2] = failYandere_hurt;
        allYandereFailAnswers[3] = failYandere_mock;
        allYandereFailAnswers[4] = failYandere_cheerup;
        #endregion
    }

    public Dialogue GetFailAnswer(INPC.Personalities personality, DialogueBattle.ApproachType apType, int rIdx)
    {
        switch (personality)
        {
            case INPC.Personalities.DereDere:
                return allDereDereFailAnswers[(int)apType][rIdx];
            case INPC.Personalities.Kuudere:
                return allKuudereFailAnswers[(int)apType][rIdx];
            case INPC.Personalities.Tsundere:
                return allTsundereFailAnswers[(int)apType][rIdx];
            case INPC.Personalities.Yandere:
                return allYandereFailAnswers[(int)apType][rIdx];
            default:
                return null;
        }
    }
}
