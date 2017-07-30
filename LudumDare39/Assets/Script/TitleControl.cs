using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour
{
	public Animator animatorStart;
	public GameObject drinkingAnimation;
	public GameObject text;
	public GameObject button1;
	public GameObject button2;

    public AudioSource audioSourceMusic;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void ButtonPlay()
    {
		Debug.Log ("Pressed");
		//animatorStart.Play ("startGame");
		drinkingAnimation.SetActive(true);
		text.SetActive(false);
		button1.SetActive(false);
		button2.SetActive(false);
        audioSourceMusic.Stop();
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
		yield return new WaitForSeconds (5.3f);
		SceneManager.LoadScene("game");
	}
}
