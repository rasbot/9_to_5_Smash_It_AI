
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class FPS : MonoBehaviour {

	public Text fpsText;

	public static float fpsAvg = 60;
	public static float fps = 60;

	int fpsFrames=0;
	
	
	float[] fpsHistory;
	int cfpsHistory=0;

	float updateInterval = 0.2f;
	float fpsLastInterval; 

	void Start () {
		fpsHistory=new float[20];
		for(int c=0; c<fpsHistory.Length; c++) fpsHistory[c]=60f;
		fpsText.text="";
	}
	

	void Update () {
		fpsFrames++;

		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > fpsLastInterval + updateInterval){
			fps = Mathf.FloorToInt(fpsFrames / (timeNow - fpsLastInterval));

			fpsFrames = 0;
			fpsLastInterval = timeNow;

			if(cfpsHistory>=fpsHistory.Length) cfpsHistory=0;
			fpsHistory[cfpsHistory]=fps;
			cfpsHistory++;

			fpsAvg=fpsHistory.Average();
			if(fpsText) fpsText.text=""+Mathf.RoundToInt(fps);
		}

	}


}
