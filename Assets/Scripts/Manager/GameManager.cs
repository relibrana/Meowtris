using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[Header ("References")]
	public CameraController cameraController;

	[Header ("UI")]

	[Header ("Game Variables")]
	public float autoMoveCameraTime = 3;
	[HideInInspector] public float autoMoveCameraCurrentTime;
	public float autoMoveCameraSpeed = 0.2f;
	bool screenShakeTrigger=false;

	public enum GameState { Menu, Prepare, Started, Over }
	[Header ("Dont touch")]
	public GameState gameState = GameState.Menu;

	public PoolingManager blocksPool;

	public GameObject player1;
	public GameObject player2;
	bool gameStart=false;
	Camera mainCamera;
	public GameObject p1wins;
	public GameObject p2wins;
	public GameObject lose;

	void Start(){
		 mainCamera = CameraEffects.instance.mainCamera.GetComponent<Camera>();

	}
    void Awake()
	{
		if (instance == null)
			instance = this;
	}

    void Update()
    {
		if(!gameStart){
			if (player1.activeSelf && player2.activeSelf)
			{
				Debug.Log ("Game Start");
				gameStart=true;
				StartGame ();
			}
		}

		if (autoMoveCameraCurrentTime < 0)
			autoMoveCameraCurrentTime = 0;

		// GameState state machine
		switch (gameState)
		{
			case GameState.Menu:
				break;
			case GameState.Prepare:
				break;
			case GameState.Started:
                autoMoveCameraCurrentTime -= Time.deltaTime;

                if (autoMoveCameraCurrentTime <= 0)
                {
                    cameraController.maxHeightReached += autoMoveCameraSpeed * Time.deltaTime;
                }
                // Check player coordinates
                CheckPlayerCoordinates();
                break;
			case GameState.Over:
				break;
		}
    }

	void CheckPlayerCoordinates()
	{
		foreach (Transform playerTransform in players)
		{
			Vector3 playerViewportPos = mainCamera.WorldToViewportPoint(playerTransform.position);

			// Check if player is outside the viewport
			if (playerViewportPos.x < 0 || playerViewportPos.x > 1 || playerViewportPos.y < 0 || playerViewportPos.y > 1)
			{
				playerTransform.gameObject.SetActive(false);
			}
		}
		CheckForDeathPlayers();
	}



	void CheckForDeathPlayers()
    {
		if(!player1.activeSelf && !player2.activeSelf)
		{
			if(screenShakeTrigger==false){
				CameraEffects.instance.DoScreenShake(0.5f,0.2f);
				screenShakeTrigger=true;
			}
			GameOver ();
       		lose.SetActive(true);
			return;
		}

		if(!player1.activeSelf)
		{
			if(screenShakeTrigger==false){
				CameraEffects.instance.DoScreenShake(0.5f,0.2f);
				screenShakeTrigger=true;
			}
			GameOver ();
			p2wins.SetActive(true);
		}

		else if (!player2.activeSelf)
		{
			if(screenShakeTrigger==false)
			{
				CameraEffects.instance.DoScreenShake(0.5f,0.2f);
				screenShakeTrigger=true;
			}
			GameOver ();
			p1wins.SetActive(true);
		}
    }

	public void StartGame ()
	{
		Debug.Log ("Call Start Game");
		StartCoroutine (StartGameRoutine());
	}

	public void GameOver ()
	{
		Debug.Log ("Game Over");
		StartCoroutine (GameOverRoutine());
	}

	public void RestartScene ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}


	IEnumerator StartGameRoutine ()
	{
		gameState = GameState.Prepare;
		yield return new WaitForSeconds (4);
		// Start Game
		autoMoveCameraCurrentTime = 0;
		gameState = GameState.Started;
	}
	IEnumerator GameOverRoutine ()
	{
		gameState = GameState.Over;
		yield return new WaitForSeconds (3f);
	}
}
