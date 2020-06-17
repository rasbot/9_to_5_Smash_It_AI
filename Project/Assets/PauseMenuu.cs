using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuu : MonoBehaviour {

    public GameObject pauseMenuCanvas;
    public GameObject pauseButton;

    public static PauseMenuu s;
 

    void Awake ()
    {
        s = this;
		pauseMenuCanvas.SetActive(false);
		pauseButton.SetActive(false);
		print("pause menu set to inactive");
	}

	//void Start()
	//{		
 //       pauseMenuCanvas.SetActive(false);   
 //       pauseButton.SetActive(false);
	//	print("pause menu set to inactive");
	//}

	public void LoadMainMenu()
    {
        SceneManager.LoadScene(0); // 0 should be the build index for the menu scene
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pauseMenuCanvas.SetActive(true);

        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }
    
    public void QuitGame()
    {
        Application.Quit();
        print("Application has been terminated");
    }
	
}
