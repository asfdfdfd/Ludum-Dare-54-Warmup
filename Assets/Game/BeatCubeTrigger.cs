using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCubeTrigger : MonoBehaviour
{
    private HashSet<GameObject> _gameObjectsEntered = new();

    public string buttonName;

    private Collider _collider;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (Input.GetButtonDown(buttonName))
        {
            foreach (var cubeGameObject in _gameObjectsEntered)
            {
                if (cubeGameObject != null)
                {
                    var cube = cubeGameObject.GetComponent<Cube>();
                    if (cube != null && !cube.isReleased)
                    {
                        cube.Intercept();
                    }
                }
            }

            _gameObjectsEntered.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == this.gameObject.layer)
        {
            _gameObjectsEntered.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit!");
        _gameObjectsEntered.Remove(other.gameObject);
    }
}
