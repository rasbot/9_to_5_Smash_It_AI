using UnityEngine;

public class TimeManager : MonoBehaviour {

#if UNITY_EDITOR

    public static TimeManager s;

	void Awake ()
    {
        s = this;	
	}

	void Start()
	{
	//Time.timeScale = 1; 
	}

	public void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
        print("time scale changed to [" + speed + "]");
    }

    public void PauseUnity()
    {
        UnityEditor.EditorApplication.isPaused = true;
        print("Unity editor is paused");
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Time.timeScale = 12f;
            PlayerController.s.waveTotal = 1;
			print("debug mode activated - one wave only");
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Time.timeScale = 8f;
        }

        for (int c = 1; c <= 7; c++) { 
            if (Input.GetKeyDown("" + c))
            {
                Time.timeScale = c / 5.0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Time.timeScale = (Time.timeScale==0) ? 1f : 0f;

        }
	}

#endif

}
