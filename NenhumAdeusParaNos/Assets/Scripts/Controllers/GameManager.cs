using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [HideInInspector] List<GameObject> playerCompanionsPref = new List<GameObject>();
    public List<GameObject> PlayerCompanionsPref { get { return playerCompanionsPref; } }

    SceneStateManager currentSceneStateManager;

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
        }

        //if (Input.GetKeyDown(KeyCode.O)) Client_UDP.Singleton.SendToServer("Jogador: " + PlayerName + " | Maior Recompensa: 500,68(s)");
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
}
