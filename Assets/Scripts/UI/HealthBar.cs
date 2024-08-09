using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    GameManager gameManager;
    PlayerController playerController;

    public Slider sliderP1;
    public Slider sliderP2;

    [SerializeField] TextMeshProUGUI FPSText;

    Vector3 sliderP1Scale;
    Vector3 sliderP2Scale;

    Vector3 recover = new Vector3(0.05f, 0.05f, 0.05f);
    float enlargement = 1.2f;

    [SerializeField] private RawImage rocketP1;
    [SerializeField] private RawImage rocketP2;

    //Score
    int player1Score;
    int player2Score;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    private void Start()
    {
        player1Score = 0;
        player2Score = 0;
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
        gameManager = FindObjectOfType<GameManager>();
        playerController = FindAnyObjectByType<PlayerController>();
        sliderP1Scale = transform.localScale;
        sliderP2Scale = transform.localScale;
        rocketP1.enabled = false;
        rocketP2.enabled = false;
        gameManager.GetFPSText(FPSText);
        playerController.Player1GO.GetComponent<PlayerController>().GetHealth();
        playerController.Player2GO.GetComponent<PlayerController>().GetHealth();
    }

    private void FixedUpdate()
    {
        if (sliderP1.transform.localScale.x > sliderP1Scale.x || sliderP1.transform.localScale.y > sliderP1Scale.y || sliderP1.transform.localScale.z > sliderP1Scale.z)
            sliderP1.transform.localScale -= recover;
        else
            sliderP1.transform.localScale = sliderP1Scale;

        if (sliderP2.transform.localScale.x > sliderP2Scale.x || sliderP2.transform.localScale.y > sliderP2Scale.y || sliderP2.transform.localScale.z > sliderP2Scale.z)
            sliderP2.transform.localScale -= recover;
        else
            sliderP2.transform.localScale = sliderP2Scale;
    }

    public void SetMaxHealth(float health, GameObject player)
    {
        if (player == playerController.Player1GO)
        {
            sliderP1.maxValue = health;
            sliderP1.value = health;
        }
        else if (player == playerController.Player2GO)
        {
            sliderP2.maxValue = health;
            sliderP2.value = health;
        }

    }

    public void SetHealth(float health, GameObject player)
    {
        if (player == playerController.Player1GO)
        {
            sliderP1.value = health;
            sliderP1.transform.localScale = sliderP1.transform.localScale * enlargement;
        }   
        else if (player == playerController.Player2GO)
        {
            sliderP2.value = health;
            sliderP2.transform.localScale = sliderP2.transform.localScale * enlargement;
        }   
    }

    public void UpdateRocketIcon(GameObject player)
    {
        if (player == playerController.Player1GO)
            rocketP1.enabled = true;
        else if (player == playerController.Player2GO)
            rocketP2.enabled = true;
    }

    public void HideRocketIcon(GameObject player)
    {
        if (player == playerController.Player1GO)
            rocketP1.enabled = false;
        else if (player == playerController.Player2GO)
            rocketP2.enabled = false;
    }

    public void UpdateScore(GameObject player)
    {
        if (player == playerController.Player1GO)
        {
            player2Score++;
            player2ScoreText.text = player2Score.ToString();
        }
        else if (player == playerController.Player2GO)
        {
            player1Score++;
            player1ScoreText.text = player1Score.ToString();
        }

    }
}
