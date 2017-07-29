using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour
{
	public Animator animatorStart;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void ButtonPlay()
    {
		Debug.Log ("Pressed");
		animatorStart.Play ("startGame");
		StartCoroutine ("EnteringGameDelay");
        //StartGame();
    }

    public void ButtonExit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
		
    }

	IEnumerator EnteringGameDelay ()
	{
		yield return new WaitForSeconds (7);
		SceneManager.LoadScene("game");
	}
}
