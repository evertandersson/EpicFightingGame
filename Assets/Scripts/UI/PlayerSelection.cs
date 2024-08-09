using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelection : MonoBehaviour
{
    public static PlayerSelection Instance;

    public GameObject Player1SelectedCharacter = null;
    public GameObject Player2SelectedCharacter = null;

    private bool player1Ready = false;
    private bool player2Ready = false;

    private string levelSelect;
    public GameObject levelScreen;
    public GameObject selectPlayerScreen;

    [HideInInspector]
    public GameObject Player1GO = null;
    [HideInInspector]
    public GameObject Player2GO = null;
    [HideInInspector]
    public GameObject PlayerGUIGO = null;

    [SerializeField] private GameObject PlayerGUI;

    public RectTransform Player1Waypoint;
    public RectTransform Player2Waypoint;

    private void Awake()
    {
        levelScreen.SetActive(true);
        selectPlayerScreen.SetActive(false);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "CharacterSelection")
        {
            Debug.Log("Scene loaded: Level01");
            GetComponent<Canvas>().enabled = false;

            // Perform actions specific to the Level01 scene
            Player1GO = Instantiate(Player1SelectedCharacter, new Vector3(-6, 8, 0), Quaternion.identity);
            Player2GO = Instantiate(Player2SelectedCharacter, new Vector3(6, 8, 0), Quaternion.identity);
            PlayerGUIGO = Instantiate(PlayerGUI);
        }
    }

    public void SelectLevel(string level)
    {
        levelSelect = level;
        levelScreen.SetActive(false);
        selectPlayerScreen.SetActive(true);
    }

    public void SelectCharacterForPlayer1(GameObject character)
    {
        Player1SelectedCharacter = character;
    }

    public void SelectCharacterForPlayer2(GameObject character)
    {
        Player2SelectedCharacter = character;
    }

    public void ReadyPlayer1()
    {
        player1Ready = true;
        NextScene();
    }

    public void ReadyPlayer2()
    {
        player2Ready = true;
        NextScene();
    }

    private void NextScene()
    {
        if (player1Ready && player2Ready)
        {
            SceneManager.LoadScene(levelSelect);
        }
    }
}