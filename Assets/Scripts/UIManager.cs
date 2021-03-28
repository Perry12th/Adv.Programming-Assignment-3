using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public GameObject gameOverPanel;
	public GameObject optionsPanel;
	public GameObject gamePlayPanel;
	public TextMeshProUGUI ResultsTxt;
		   
	public TextMeshProUGUI scoreTxt;
	public TextMeshProUGUI requiredScoreTxt;
	public TextMeshProUGUI TimeCounterTxt;
	public TextMeshProUGUI ModeTxt;

	public TextMeshProUGUI timeGivenTxt;
	public TextMeshProUGUI scoreRequiredTxt;

	private int score;
	private int requiredScore = 4000;
	private float timeCounter = 90;
	private int startingTime = 90;

	public bool gameOver = false;

	public int Score
    {
		get { return score; }

		set 
		{ 
			score = value; 
			scoreTxt.text = score.ToString(); 

			if (score >= requiredScore)
            {
				timeCounter = 0;
				StartCoroutine(WaitForShifting());
			}
		}
	}

	public int StartingTime
    { 
		get { return startingTime; }

		set
        {
			startingTime = value;

			timeGivenTxt.text = "Time Given: " + startingTime;
        }
	}


	public int RequiredScore
    {
		get { return requiredScore; }

        set
        {
			requiredScore = value;
			requiredScoreTxt.text = requiredScore.ToString();
			scoreRequiredTxt.text = "RequiredScore : " + requiredScore;
        }
	}


	public float TimeCounter
	{
		get { return timeCounter; }

		set
		{
			timeCounter = value;
			TimeCounterTxt.text = ((int)timeCounter).ToString();
			if (timeCounter <= 0)
            {
				timeCounter = 0;
				StartCoroutine(WaitForShifting());
            }
		}
	}

	void Awake()
	{
		Score = 0;
		TimeCounterTxt.text = timeCounter.ToString(); 
		instance = GetComponent<UIManager>();
	}

    private void Update()
    {
		if (!gameOver)
		{
			TimeCounter -= Time.deltaTime;
		}
    }

    private void Start()
    {
		TimeCounter = startingTime;
	}

    // Show the game over panel
    public void GameOver()
	{
		gameOver = true;

		gameOverPanel.SetActive(true);
		gamePlayPanel.SetActive(false);
		MatchBoardManager.instance.GameOver();

		if (score >= requiredScore)
		{
			ResultsTxt.text = "You managed to get more than " + requiredScore + " points!, Well Done";
		}
		else
		{
			ResultsTxt.text = "You didn't managed to get more than " + requiredScore + " points this time";
		}

	}

	public void StartGame()
    {
		gamePlayPanel.SetActive(true);
		MatchBoardManager.instance.StartGame();
		TimeCounter = startingTime;
		gameOver = false;
	}

	private IEnumerator WaitForShifting()
    {
		yield return new WaitUntil(() => !MatchBoardManager.instance.IsShifting);
		yield return new WaitForSeconds(.25f);
		GameOver();
    }


	public void SetGameMode(int diffculty)
    {
		MatchBoardManager.instance.GameMode = (Mode)diffculty;

		if (diffculty == (int)Mode.Easy)
        {
			ModeTxt.text = "Match 3 of the same to gain points";
        }

		if (diffculty == (int)Mode.Medium)
		{
			ModeTxt.text = "WATCH OUT FOR THE BEES, YOU CANT TOUCH THEM!!";
		}

		if (diffculty == (int)Mode.Hard)
		{
			ModeTxt.text = "Oh Great now you need to match 4 instead of 3, also BEES!!";
		}
	}
}
