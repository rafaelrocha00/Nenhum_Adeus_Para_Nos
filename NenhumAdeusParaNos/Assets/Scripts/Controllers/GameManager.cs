using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [HideInInspector] public DialogueController dialogueController;
    [HideInInspector] public BattleController battleController;
    [HideInInspector] public TimeController timeController;
    [HideInInspector] public InventoryController inventoryController;
    [HideInInspector] public ObjectPlacer objectPlacer;
    [HideInInspector] public QuestController questController;
    [HideInInspector] public QuestGenerator questGenerator;
    [HideInInspector] public CalendarController calendarController;
    [HideInInspector] public CompanyController companyController;
    //[HideInInspector] public NPCAnswers npcAnswers;

    [HideInInspector] MainHud mainHud;
    public MainHud MainHud { get { return mainHud; } set { mainHud = value; } }
    [HideInInspector] CamMove mainCamera;
    public CamMove MainCamera { get { if (mainCamera == null) { mainCamera = Camera.main.GetComponent<CamMove>(); return mainCamera; } else return mainCamera; } }

    //[HideInInspector] Player mainCharacter;
    //public Player MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }

    //public float[] DereDereP = new float[5];
    //public float[] KuudereP = new float[5];
    //public float[] TsundereP = new float[5];
    //public float[] YandereP = new float[5];

    public float[][] personalities = new float[4][];

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
        dialogueController = GetComponent<DialogueController>();
        battleController = GetComponent<BattleController>();
        timeController = GetComponent<TimeController>();
        inventoryController = GetComponent<InventoryController>();
        objectPlacer = GetComponent<ObjectPlacer>();
        questController = GetComponent<QuestController>();
        questGenerator = GetComponent<QuestGenerator>();
        calendarController = GetComponent<CalendarController>();
        companyController = GetComponent<CompanyController>();
        //npcAnswers = GetComponent<NPCAnswers>();

        //personalities[0] = DereDereP;
        //personalities[1] = KuudereP;
        //personalities[2] = TsundereP;
        //personalities[3] = YandereP;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                battleController.ActiveBattle = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
                dialogueController.SetCam();
            }
        }
    }
}
