using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BeatCubePool : MonoBehaviour
{
    public GameObject cubePrefab;
    
    private List<GameObject> predefinedCubes = new List<GameObject>();

    public static BeatCubePool Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            var gameObjectCube = Instantiate(cubePrefab, transform);
            gameObjectCube.SetActive(false);
            predefinedCubes.Add(gameObjectCube);
        }
    }

    public GameObject AcquireCube(GameObject parent)
    {
        var cube = predefinedCubes[0];
        cube.layer = parent.layer;
        predefinedCubes.RemoveAt(0);
        cube.SetActive(true);
        cube.transform.parent = parent.transform;
        cube.transform.localPosition = Vector3.zero;
        // cube.GetComponent<Collider>().enabled = true;
        // cube.GetComponent<Cube>().isReleased = false;
        return cube;
    }

    public void ReleaseCube(GameObject gameObject)
    {
        // gameObject.SetActive(false);
        // gameObject.transform.parent = this.gameObject.transform;
        // gameObject.transform.localPosition = Vector3.zero;
        // gameObject.GetComponent<Cube>().StopAnything();
        // predefinedCubes.Add(gameObject);
        Destroy(gameObject);
    }
}
