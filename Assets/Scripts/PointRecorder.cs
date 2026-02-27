using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
using TNRD.Utilities;
#endif
[Serializable]
public class Recording
{
    public List<FrameData> frames = new List<FrameData>();
}
[Serializable]
public class FrameData
{
    public Vector3[] positions;
}
public class PointRecorder : MonoBehaviour
{
    [SerializeField] private WebCam webCam;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float maxRecordTime = 60f;
    private bool isRecording = false;
    private float startTime;
    private List<FrameData> recordedFrames = new List<FrameData>();
    private Coroutine statusCoroutine;
    private void Start()
    {
        if (statusCoroutine == null)
        {
            statusCoroutine = StartCoroutine(StatusLogCoroutine());
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isRecording)
        {
            StartRecording();
        }
        if (Input.GetKeyDown(KeyCode.W) && isRecording)
        {
            StopRecording();
        }
    }
    private void StartRecording()
    {
        isRecording = true;
        startTime = Time.time;
        recordedFrames.Clear();
        Debug2.Log("Recording started");
        StartCoroutine(RecordCoroutine());
    }
    private IEnumerator RecordCoroutine()
    {
        while (isRecording && (Time.time - startTime) < maxRecordTime)
        {
            Vector3[] positions = webCam.GetAllPointPositions();
            if (positions != null)
            {
                recordedFrames.Add(new FrameData { positions = positions });
            }
            yield return new WaitForSeconds(recordInterval);
        }
        if (isRecording)
        {
            StopRecording();
        }
    }
    private void StopRecording()
    {
        isRecording = false;
        if (recordedFrames.Count > 0)
        {
            Recording recording = new Recording { frames = recordedFrames };
            string json = JsonUtility.ToJson(recording);
            PlayerPrefs2.SetString("PointRecording", json);
            Debug2.Log("Recording saved to PlayerPrefs2.");
        }
        else
        {
            Debug2.Log("No frames recorded.");
        }
        Debug2.Log("Recording stopped");
    }
    private IEnumerator StatusLogCoroutine()
    {
        while (true)
        {
            if (isRecording)
            {
                float elapsed = Time.time - startTime;
                Debug2.Log($"Recording time: {Mathf.FloorToInt(elapsed)} seconds");
            }
            yield return new WaitForSeconds(1f);
        }
    }
}