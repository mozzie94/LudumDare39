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
		animatorStart.Play ("startGame");
        //StartGame();
    }

    public void ButtonExit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }
}
