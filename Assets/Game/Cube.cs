using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private bool isLaunched = false;
    
    private float partSpeed = 0.0f;

    private Tween _tween;

    public bool isReleased = false;
    
    private void Update()
    {
        if (!isLaunched)
        {
            return;
        }

        // transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, partSpeed * Time.deltaTime);
    }

    public void Launch(float partsBeforeActualStartSec)
    {
        this.partSpeed = partsBeforeActualStartSec;
        this.isLaunched = true;

        _tween = transform
            .DOMove(Vector3.zero, partsBeforeActualStartSec)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                BeatCubePool.Instance.ReleaseCube(gameObject);
                isReleased = true;
                _tween = null;
            });
    }

    public void Intercept()
    {
        StopAnything();

        if (isReleased)
        {
            return;
        }
        
        gameObject.GetComponent<Collider>().enabled = false;

        _tween = transform.DOMoveY(transform.position.y + 5, 0.3f).OnComplete(() =>
        {
            BeatCubePool.Instance.ReleaseCube(gameObject);
            isReleased = true;
            _tween = null;
        });
    }

    public void StopAnything()
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }
    }
}
