using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug=UnityEngine.Debug;


static public class db {

	static int cCube=0;
	public static float timeOut=2f;
	
	public static Transform dbTF;


	#region benchmarking
		static Stopwatch benchWatch;

		static public void benchStart(){
			if(benchWatch==null) benchWatch=new Stopwatch();
			benchWatch.Start();
		}
		static public void benchStop(string msg=""){
			benchWatch.Stop();
			Debug.Log("___ "+msg+": "+benchWatch.ElapsedMilliseconds);
		}
	#endregion



	static public GameObject label(Vector3 pos, string txt, float sc=1f, float timeout=0){

		GameObject txtLabelObj=new GameObject();
		txtLabelObj.transform.position=pos;
		txtLabelObj.name=pos+" : "+txt;
		txtLabelObj.transform.localScale=Vector3.one*sc;
		txtLabelObj.transform.LookAt(Camera.main.transform.position);

		txtLabelObj.AddComponent<MeshRenderer>();

		TextMesh textMesh=txtLabelObj.AddComponent<TextMesh>();
		textMesh.fontSize=24;
		textMesh.text=txt;

		if(dbTF==null) dbTF=new GameObject("dbTF").transform;
		txtLabelObj.transform.parent=dbTF;
		txtLabelObj.transform.localScale=new Vector3(-sc, sc, sc);

		if(timeout>0) GameObject.Destroy(txtLabelObj, timeout);

		return txtLabelObj;
	}


	static public void labelFollow(int id, Transform parentTF, string txt, float sc=1f, float yOffset=1f, bool incrementYwithID=false){
		Vector3 offset=Vector3.up*yOffset;
		if(incrementYwithID) offset.y+=(id-1)*4f;

		Transform labelTF=parentTF.Find("Label "+id);
		GameObject fLabelObj=null;

		if(labelTF) fLabelObj=labelTF.gameObject;

		if(fLabelObj==null) {
			fLabelObj=label(parentTF.position+offset, txt, sc);
			fLabelObj.name="Label "+id;
			dbLookAt lookAt=fLabelObj.AddComponent<dbLookAt>();
			lookAt.lookAtTF=Camera.main.transform;
			fLabelObj.transform.localScale=new Vector3(-sc, sc, sc);
		}
		
		fLabelObj.transform.parent=parentTF;
		fLabelObj.GetComponent<TextMesh>().text=txt;

	}



	static public GameObject line(Vector3 posFrom, Vector3 posTo, float thickness=0.1f, string name="", Color clr=default(Color)){
		GameObject obj=line(posFrom, Vector3.Distance(posFrom,posTo), (posTo-posFrom).normalized, thickness, name, clr);
		
		return obj;
	}
	static public GameObject line(Vector3 pos, float length=1.2f, Vector3 dir=default(Vector3), float thickness=0.1f, string name="", Color clr=default(Color)){
		if(dir==Vector3.zero){dir=Vector3.up;}
		GameObject cube=GameObject.CreatePrimitive(PrimitiveType.Cube);
		if(clr!=default(Color)){
			cube.GetComponent<Renderer>().material.color=clr;
		}
		cube.GetComponent<Collider>().enabled=false;
		cube.transform.position=pos;
		cube.transform.localScale=new Vector3(thickness, thickness, length);
		cube.transform.LookAt(pos+dir);
		cube.transform.Translate(0,0,length/2f);

		cCube++;
		if(name.Length>0) cube.name=name;
		else cube.name=cCube+"  "+(int)cube.transform.position.x+"_"+(int)cube.transform.position.y+"_"+(int)cube.transform.position.z;

		if(timeOut>0) GameObject.Destroy(cube, timeOut);
		//cube.transform.parent=Game.dbTF;
		return cube;
	}

	static public void lineFollow(int id, Vector3 posFrom, Vector3 posTo, float thickness=0.1f, string name="", Color clr=default(Color)){
		GameObject fLine=GameObject.Find("lineFollow "+id);
		if(fLine) GameObject.Destroy(fLine);
		line(posFrom, posTo, thickness, "lineFollow "+id, clr);
	}


	static public void cubeFollow(int id, Vector3 pos, float sc=1.02f, Color clr=default(Color)){
		GameObject fCube=GameObject.Find("cubeFollow "+id);
		if(fCube==null) fCube=cube(pos, sc, "cubeFollow "+id);
		fCube.transform.position=pos;
		fCube.transform.localScale=Vector3.one*sc;
        fCube.GetComponent<Renderer>().material.color=clr;
	}


	static public GameObject cube(Transform copyTF, float sc=1.02f, string name=""){
		GameObject obj=cube(copyTF.position,copyTF.transform.localScale.magnitude,copyTF.name+" (clone)");
		obj.transform.localRotation=copyTF.localRotation;
		obj.transform.localScale=new Vector3(sc,sc,sc);
		if(name.Length>0){
			obj.name=name;
		}
		
		return obj;
	}
	static public GameObject cube(Vector3 pos, Color clr, float sc=1.02f){
		GameObject obj=cube(pos,sc);
		obj.GetComponent<Renderer>().material.color=clr;
		
		return obj;
	}
	static public GameObject cube(Vector3 pos, float sc=1.02f, string name=""){
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		obj.GetComponent<Collider>().enabled=false;
		obj.transform.position=pos;
		cCube++;

		if(name.Length>0) obj.name=name;
		else obj.name=cCube+"  "+(int)obj.transform.position.x+"_"+(int)obj.transform.position.y+"_"+(int)obj.transform.position.z;
		obj.transform.localScale=new Vector3(sc,sc,sc);

		if(timeOut>0) GameObject.Destroy(obj,timeOut);
		

		return obj;
	}

	static Text dbTxt;

    static public void warn(float txt) { warn(txt.ToString()); }
    static public void warn(int txt) { warn(txt.ToString()); }
	static public void warn(string txt){
		if(dbTxt==null){
			dbTxt=GameObject.Find("dbTxt-Warn").GetComponent<Text>();
		}
		dbTxt.text=txt;
		//Debug.Log(txt);
	}

	static public void p(string t){Debug.Log(t);}//print

    static public void pause()
    {
#if UNITY_EDITOR
        System.Console.WriteLine("Unity Editor Paused!");
        UnityEditor.EditorApplication.isPaused=true;
        #endif
    }

    static public void unpause()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused=false;
        #endif
    }
}





public class dbLookAt : MonoBehaviour {
	public Transform lookAtTF;

	void Update () {
		transform.LookAt(lookAtTF);
	}
}