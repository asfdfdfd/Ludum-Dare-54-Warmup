using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

public class PrintCsvInfo : MonoBehaviour
{
    
    struct BeatPart
    {
        public bool hasBeat;
    }
    
    private int msecInBeat;
    private int msecInPart;
    private float secInPart;

    public GameObject cube1;
    public GameObject cube2;

    public string url;

    public BeatCubeGenerator generator1;
    public BeatCubeGenerator generator2;

    private bool shouldGenerateCubes = false;

    public AudioSource music;

    private double _blinkCubeClock = 0.0f;

    private double _nextBlinkCubeTime = 0.0f;

    private double _dspTimePrev = Double.MinValue;
    
    private double _augmentedClock = 0.0f;

    private double _musicStartupTime = 0.0f;

    private int _track1BlinkIndex;
    private int _track2BlinkIndex;

    private double _nextPartTime;

    // Start is called before the first frame update
    void Start()
    {
        var csvTextAsset = Resources.Load<TextAsset>("track");
        LaunchGame(csvTextAsset.text);

        // StartCoroutine(LaunchGameCoroutine());
    }

    private IEnumerator LaunchGameCoroutine()
    {
        var request = UnityWebRequest.Get(url);
        yield return request.Send();
        LaunchGame(request.downloadHandler.text);
    }

    private void LaunchGame(string csvText)
    {
        var csvParser = new CsvParser(new StringReader(csvText), CultureInfo.InvariantCulture);
        var rows = new List<string[]>();
        while (csvParser.Read())
        {
            var row = csvParser.Record;
            if (row != null)
            {
                rows.Add(row);
            }
        }

        var bpm = int.Parse(rows[0][1]);
        var partsInBeat = int.Parse(rows[1][1]);
        
        Debug.Log(String.Format("BPM: {0}. Parts in beat: {1}", bpm, partsInBeat));
        
        var msecInMinute = 60 * 1000;
        
        msecInBeat = msecInMinute / bpm;
        msecInPart = msecInBeat / partsInBeat;
        secInPart = msecInPart / 1000.0f;
        
        var partsBeforeActualStart = partsInBeat * 2;
        var partsBeforeActualStartMsec = partsBeforeActualStart * msecInPart;
        var partsBeforeActualStartSec = partsBeforeActualStartMsec / 1000.0f;
        
        var startPosition = generator1.gameObject.transform.position;
        var endPosition = Vector3.zero;

        var partsBeforeActualStartDistance = Vector3.Distance(startPosition, endPosition);
        
        Debug.Log("Distance: " + partsBeforeActualStartDistance);
        Debug.Log("Parts: " + partsBeforeActualStart);
        Debug.Log("Time of all these parts: " + partsBeforeActualStartSec);

        var partSpeed = partsBeforeActualStartDistance / partsBeforeActualStartSec;

        Debug.Log("Part speed: " + partSpeed);
        
        var track1 = new List<BeatPart>();
        var track2 = new List<BeatPart>();
        
        for (int i = 3; i < rows.Count; i++)
        {
            var beatIndicator1 = rows[i][0];
            track1.Add(new BeatPart() { hasBeat = beatIndicator1.Length > 0});
            var beatIndicator2 = rows[i][2];
            track2.Add(new BeatPart() { hasBeat = beatIndicator2.Length > 0});            
        }
        
        StartCoroutine(GenerateCubesCoroutine(generator1, track1, partsBeforeActualStartSec));
        StartCoroutine(GenerateCubesCoroutine(generator2, track2, partsBeforeActualStartSec));

        StartCoroutine(BlinkCubesCoroutine(cube1, cube2, track1, track2));

        _musicStartupTime = AudioSettings.dspTime + partsBeforeActualStartSec;

        _track1BlinkIndex = 0;
        _track2BlinkIndex = 0;
        _nextPartTime = _musicStartupTime + secInPart;

        music.PlayScheduled(_musicStartupTime);
    }

    private IEnumerator GenerateCubesCoroutine(BeatCubeGenerator generator, List<BeatPart> parts, float partsBeforeActualStartSec)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            var part = parts[i];
            if (part.hasBeat)
            {
                generator.GenerateCube(partsBeforeActualStartSec);
            }
            
            yield return new WaitForSecondsRealtime(secInPart);            
        }
    }

    private IEnumerator BlinkCubesCoroutine(GameObject cube1, GameObject cube2, List<BeatPart> parts1, List<BeatPart> parts2)
    {
        var nextPartIndex = 0;
        
        while (true)
        {
            while (_nextPartTime > _augmentedClock)
            {
                yield return null;
            }

            BlinkCubeIfRequired(cube1, parts1, nextPartIndex);
            BlinkCubeIfRequired(cube2, parts2, nextPartIndex);

            nextPartIndex++;
            _nextPartTime += secInPart;

            if (nextPartIndex == parts1.Count)
            {
                break;
            }
        }
    }

    private void BlinkCubeIfRequired(GameObject cube, List<BeatPart> parts, int partIndex)
    {
        var part = parts[partIndex];
            
        if (part.hasBeat)
        {
            StartCoroutine(BlinkCube(cube));
        }
    }
    
    private IEnumerator BlinkCube(GameObject cube)
    {
        yield return cube.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.03f).WaitForCompletion();
        yield return cube.transform.DOScale(new Vector3(1, 1, 1), 0.03f).WaitForCompletion();
    }

    private void Update()
    {
        var augmentedTime = UpdateAugmentedClock();
    }

    private double UpdateAugmentedClock()
    {
        if (_dspTimePrev != AudioSettings.dspTime)
        {
            _dspTimePrev = AudioSettings.dspTime;
            _augmentedClock = _dspTimePrev;
        }
        else
        {
            _augmentedClock += Time.unscaledDeltaTime;
        }

        return _augmentedClock;
    }
}
