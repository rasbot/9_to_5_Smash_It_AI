using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;



public static class mgExtensions{


    public static Vector3 apparentVel(this Rigidbody rb, Vector3 prevPos) {
        return rb.position-prevPos;
    }



    ////PHP calls WWW, UnityWebRequest
	public static bool isFinished(this WWW www) {
		return www.isDone && string.IsNullOrEmpty(www.error); 
	}
	public static bool hasError(this WWW www) {
		return (!string.IsNullOrEmpty(www.error) || www.text().Contains("dberror"));
	}
    public static string text(this WWW www)
    {
        return www.text;
    }
    //
    public static bool hasError(this UnityWebRequest www)
    {
        #if UNITY_EDITOR
        Debug.Log("www.downloadHandler.text="+www.downloadHandler.text);
        #endif

        return (!string.IsNullOrEmpty(www.error) || www.downloadHandler.text.Contains("dberror"));
    }
    public static string text(this UnityWebRequest www)
    {
        return www.downloadHandler.text;
    }
    ////



	public static GameObject getClosestGameObj(this Vector3 pos, GameObject[] objs) {
		float closestDist=Mathf.Infinity;
		GameObject closestObj=null;
		
		for(int c=0; c<objs.Length; c++) {
			float sqrDist=(pos-objs[c].transform.position).sqrMagnitude;
			if( sqrDist<closestDist){
				closestDist=sqrDist;
				closestObj=objs[c];
			}
		}

		return closestObj;
	}



	public static void DestroyChildren(this Transform root) {
		for(int c=0; c<root.childCount; c++) GameObject.Destroy(root.GetChild(c).gameObject);
	}

	public static void HideChildren(this Transform root) {
		for(int c=0; c<root.childCount; c++) root.GetChild(c).gameObject.SetActive(false);
	}
	public static void HideChildren(this GameObject root) {
		for(int c=0; c<root.transform.childCount; c++) root.transform.GetChild(c).gameObject.SetActive(false);
	}

	public static List<GameObject> GetAllChildren(this GameObject parentObj) {
		List<GameObject> objs=new List<GameObject>(); 
		foreach(Transform tf in parentObj.transform) objs.Add(tf.gameObject);
		return objs;
	}
	public static List<GameObject> GetAllChildren(this Transform parent) {
		List<GameObject> objs=new List<GameObject>(); 
		foreach(Transform tf in parent) objs.Add(tf.gameObject);
		return objs;
	}

	public static Transform Copy(this Transform origTF){
		Transform t=new GameObject("Transform Copy").transform;
		t.position=origTF.position;
		t.rotation=origTF.rotation;
		t.localScale=origTF.localScale;
		return t;
	}


	public static string commas(this int i){
		if(i==0) return "0";
		else return i.ToString("#,#");
	}


	public static Vector3 rnd(this Vector3 v, float range, bool clampY=false){
		return new Vector3( Random.value*range-range/2f, clampY ? 0f : Random.value*range-range/2f, Random.value*range-range/2f);
	}


	public static Vector3 RoundToInts(this Vector3 v) {
		return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
	}

	public static Vector3 ClampY0(this Vector3 v){
		return new Vector3(v.x,0,v.z); 
	}

	public static Vector3 add(this Vector3 v, Vector3 addv){
		return new Vector3(v.x+addv.x, v.y+addv.y, v.z+addv.z); 
	}

	public static Vector3 add(this Vector3 v, float x, float y, float z){
		return new Vector3(v.x+x, v.y+y, v.z+z); 
	}

    public static Vector3 addX(this Vector3 v, float x)
    {
        return new Vector3(v.x + x, v.y, v.z); 
    }
    public static Vector3 add(this Vector3 v, float y)
    {
        return new Vector3(v.x, v.y + y, v.z); 
    }
    public static Vector3 addZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, v.z + z); 
    }


    public static Vector3 newX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z); 
    }
    public static Vector3 newY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z); 
    }
    public static Vector3 newZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z); 
    }



}
