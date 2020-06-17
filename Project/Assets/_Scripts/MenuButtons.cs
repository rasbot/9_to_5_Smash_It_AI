using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {

    public GameObject disclaimerText;
    public GameObject menu;
    public GameObject loading;

    // Use this for initialization
    void Start()
    {
        disclaimerText.SetActive(true);
        menu.SetActive(false);
        loading.SetActive(false);
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
	
    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Application has been terminated");
    }

    IEnumerator LoadMenu()
    {
        disclaimerText.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        menu.SetActive(true);
    }

    IEnumerator LoadingGameDelay()
    {
        menu.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        loading.SetActive(true);
		LoadGame();
    }

	IEnumerator LoadingTutorialDelay()
	{
		menu.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		loading.SetActive(true);
		LoadTutorial();
	}

    public void LoadingGame()
    {
        StartCoroutine(LoadingGameDelay());
    }

	public void LoadingTutorial()
	{
		StartCoroutine(LoadingTutorialDelay());
	}

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(LoadMenu());
        }

#elif UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            StartCoroutine(LoadMenu());
        }
#endif
    }
}
