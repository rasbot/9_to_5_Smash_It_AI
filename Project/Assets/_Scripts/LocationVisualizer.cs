using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationVisualizer : MonoBehaviour {

    public bool showLocations;          // toggle to show locations
    public GameObject locationVisual;   // prefab sphere

    private List<Vector3> visualLocations = new List<Vector3>();        // list of spawn locations
    private List<GameObject> locationVisuals = new List<GameObject>();  // list of spheres
    private GameObject temp;
    private bool spheresMade;

    // Use this for initialization
    void Start ()
    {
        showLocations = false;
        spheresMade = false;
    }
	
    void MakeLocationsVisible()
    {
        print("spheres made = " + spheresMade);
        for (int i = 0; i < transform.childCount; ++i)
        {
            visualLocations.Add(transform.GetChild(i).transform.position); // list of positions
        }
        //for (int i = 0; i <= PlayerController.spawnLocations.Count - 1; i++)
        //{
        //    visualLocations.Add(PlayerController.spawnLocations[i].position);
        //}

        for (int j = 0; j < visualLocations.Count; j++) // make spheres at each location
        {
            temp = Instantiate(locationVisual, visualLocations[j], Quaternion.identity);
            locationVisuals.Add(temp);

        }   
    }

	// Update is called once per frame
	void Update ()
    {
		if (showLocations)
        {
            if (!spheresMade)
            {
                MakeLocationsVisible();
                spheresMade = true;
            }

        }
        else
        {
            foreach (GameObject go in locationVisuals)
            {
                Destroy(go);
            }
            visualLocations = new List<Vector3>();
            locationVisuals = new List<GameObject>();
            //foreach(Vector3 vect in visualLocations)
            //{
            //    Destroy(vect);
            //}
            spheresMade = false;
        }
	}
}
