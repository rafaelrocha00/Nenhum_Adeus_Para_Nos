using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [HideInInspector] public AudioController audioController;
    [HideInInspector] public DialogueController dialogueController;
    [HideInInspector] public BattleController battleController;
    [HideInInspector] public TimeController timeController;
    [HideInInspector] public InventoryController inventoryController;
    [HideInInspector] public ObjectPlacer objectPlacer;
    [HideInInspector] public QuestController questController;
    [HideInInspector] public QuestGenerator questGenerator;
    [HideInInspector] public CalendarController calendarController;
    [HideInInspector] public CompanyController companyController;
    [HideInInspector] public RepairController repairController;
    [HideInInspector] public ItemsSaver itemsSaver;

    [HideInInspector] MainHud mainHud;
    public MainHud MainHud { get { return mainHud; } set { mainHud = value; } }
    [HideInInspector] CamMove mainCamera;
    public CamMove MainCamera { get { if (mainCamera == null) { mainCamera = Camera.main.GetComponent<CamMove>(); return mainCamera; } else return mainCamera; } }

    //public float[][] personalities = new float[4][];
    [SerializeField] int spawnpointID = -1;
    public int SpawnpointID { get { return spawnpointID; } set { spawnpointID = value; } }

    public string PlayerName = "guest";

    [HideInInspector] bool newGame = true;
    public bool NewGame { get { return newGame; } set { newGame = value; } }

    [SerializeField] bool battleUnlocked = false;
    public bool BattleUnlocked { get { return battleUnlocked; } set { battleUnlocked = value; } }

    [HideInInspector] List<GameObject> playerCompanionsPref = new List<GameObject>();
    public List<GameObject> PlayerCompanionsPref { get { return playerCompanionsPref; } }

    SceneStateManager currentSceneStateManager;

    public List<Quest> questsToSkip = new List<Quest>();
    int id = 0;
    bool accepting = false;

    public Image black_screen;
    public Text title_txt;


    private void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }

        //mainHud = GameObject.Find("/MainHud").GetComponent<MainHud>();
        audioController = GetComponent<AudioController>();
        dialogueController = GetComponent<DialogueController>();
        battleController = GetComponent<BattleController>();
        timeController = GetComponent<TimeController>();
        inventoryController = GetComponent<InventoryController>();
        objectPlacer = GetComponent<ObjectPlacer>();
        questController = GetComponent<QuestController>();
        questGenerator = GetComponent<QuestGenerator>();
        calendarController = GetComponent<CalendarController>();
        companyController = GetComponent<CompanyController>();
        repairController = GetComponent<RepairController>();
        itemsSaver = GetComponent<ItemsSaver>();

        GameManager.gameManager.repairController.OnLoadScene();
        GameManager.gameManager.questController.SpawnAllQuestMarks();
        GameManager.gameManager.LockUnlockPlayerBattle();
        GameManager.gameManager.battleController.BattleUnlocked = battleUnlocked;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                battleController.ActiveBattle = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                dialogueController.SetCam();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {               
                if (accepting)
                {
                    if (!questsToSkip[id].Accepted) questsToSkip[id].AcceptQuest();
                    else Debug.Log("Ja aceitou");
                    accepting = false;
                }
                else
                {
                    if (!questsToSkip[id].Completed) questsToSkip[id].Complete();
                    id++;
                    accepting = true;
                }

                gameManager.questController.questsCompleted = 0;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                ShowTitle();
            }
        }

        //if (Input.GetKeyDown(KeyCode.O)) Client_UDP.Singleton.SendToServer("Jogador: " + PlayerName + " | Maior Recompensa: 500,68(s)");
    }

    public void LockUnlockPlayerBattle()
    {
        GameManager.gameManager.battleController.MainCharacter.BattleUnlocked = BattleUnlocked;
    }

    public void ChangeCurrentSceneState(SceneStateConditions sceneState)
    {
        try
        {
            if (currentSceneStateManager == null)
            {
                GameObject go = GameObject.Find("SceneStateManager");
                currentSceneStateManager = go.GetComponent<SceneStateManager>();
            }
            //GameObject go = GameObject.Find("SceneStateManager");
            currentSceneStateManager.ChangeSceneState(sceneState.ChangingState, sceneState.MovePlayer, sceneState.FadeInOut);
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public void ChangeScene(string sceneName, int id)
    {
        GameManager.gameManager.SpawnpointID = id;

        GameManager.gameManager.inventoryController.Inventory.SaveItems();
        GameManager.gameManager.itemsSaver.BlockChestGen();

        SceneManager.LoadScene(sceneName);
    }

    public void ShowTitle(string sceneName = "")
    {
        black_screen.gameObject.SetActive(true);
        StartCoroutine(FadeIn(0.26f, 3.0f, sceneName));
        StartCoroutine(FadeInTitle(0.26f, 3.5f));

        if (!sceneName.Equals(""))
        {
            HideTitle(6.0f);
        }

    }

    public void HideTitle(float delay)
    {
        black_screen.gameObject.SetActive(true);
        StartCoroutine(FadeOutTitle(1.26f, delay));
        StartCoroutine(FadeOut(1.5f, delay + 1.75f));
    }

    IEnumerator FadeIn(float t, float delay, string sn = "")
    {
        yield return new WaitForSeconds(delay);

        float timer = 0.0f;

        while (timer <= t)
        {
            black_screen.color = new Color(0, 0, 0, timer / t);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (!sn.Equals("")) GameManager.gameManager.ChangeScene(sn, -1);
    }

    IEnumerator FadeInTitle(float t, float delay)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0.0f;

        while (timer <= t)
        {
            title_txt.color = new Color(1, 1, 1, timer / t);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator FadeOut(float t, float delay)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0.0f;

        while (timer <= t)
        {
            black_screen.color = new Color(0, 0, 0, 1 - timer / t);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        black_screen.gameObject.SetActive(false);
    }

    IEnumerator FadeOutTitle(float t, float delay)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0.0f;

        while (timer <= t)
        {
            title_txt.color = new Color(1, 1, 1, 1 - timer / t);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
