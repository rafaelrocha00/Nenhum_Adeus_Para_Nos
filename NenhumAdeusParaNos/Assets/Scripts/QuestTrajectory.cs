using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrajectory : MonoBehaviour
{
    [SerializeField] Quest quest = null;
    public Quest Quest { get { return quest; } }

    public GameObject route;

    //bool enteredARoute;
    //bool leftARoute;

    //Coroutine waitForExit;

    private void Start()
    {
        CustomEvents.instance.onQuestAccepted += CheckCondition;
        CustomEvents.instance.onQuestComplete += CheckCondition;

        CheckCondition(quest.Name);
    }

    private void OnDestroy()
    {
        CustomEvents.instance.onQuestAccepted -= CheckCondition;
        CustomEvents.instance.onQuestComplete -= CheckCondition;
    }

    public void CheckCondition(string quest_name)
    {
        if (!quest_name.Equals(quest.Name)) return;

        if (quest.Accepted && !quest.Completed) route.SetActive(true);
        else if (!quest.Accepted || quest.Completed) route.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            //StopCoroutine("ResetLeft");

            //if (leftARoute)
            //{
            //    ExitTrajectory();
            //    return;
            //}
            //leftARoute = true;
            //StartCoroutine("ResetLeft"); 
            Invoke("ExitTrajectory", 1.0f);
            Debug.Log("Saiu de um Rota");
            //if (!) ExitTrajectory();
            //StopCoroutine(ResetLeft());
            //leftARoute = true;
            //StartCoroutine(ResetLeft());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            //StopCoroutine("CheckDelayedExit");

            //enteredARoute = true;
            //if (waitForExit == null) waitForExit = StartCoroutine(WaitForExit());

            //StartCoroutine("CheckDelayedExit");
            //enteredARoute = true;
            //StartCoroutine(CheckDelayedExit());
            CancelInvoke();
            Debug.Log("Entrou em uma outra Rota");
        }
    }

    //IEnumerator WaitForExit()
    //{
    //    while (true)
    //    {
    //        if (!enteredARoute && leftARoute) ExitTrajectory();
    //        yield return new WaitForFixedUpdate();        
    //    }
    //}

    //IEnumerator CheckDelayedExit()
    //{
    //    yield return new WaitForSeconds(1.0f);
    //    enteredARoute = false;
    //}

    //IEnumerator ResetLeft()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    leftARoute = false;
    //}

    void ExitTrajectory()
    {
        //ResetDefault();
        Debug.Log("Saiu completamente da trajetória");
        CustomEvents.instance.OnExitTrajectory();
    }

    //void ResetDefault()
    //{
    //    StopAllCoroutines();
    //    leftARoute = false;
    //    enteredARoute = false;

    //    //waitForExit = null;
    //}
}
