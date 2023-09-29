using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class BeatCubeGenerator : MonoBehaviour
{
    public void GenerateCube(float partsBeforeActualStartSec)
    {
        var gameObjectCube = BeatCubePool.Instance.AcquireCube(gameObject);
        gameObjectCube.GetComponent<Cube>().Launch(partsBeforeActualStartSec);
    }
}
