using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Line linePrefab;

    private AudioSource audioSource;

    private MapDesc mapDesc;

    private int timeLineClip = 0;
    private int lineClip = 0;

    void Start()
    {
        // TODO 这个放在整个游戏初始化阶段
        Input.multiTouchEnabled = true;
        string basePath = "beatmaps/Chrono Diver -PENDULUMs- (USAO remix)/";
        var mapDescAssert = Resources.Load<TextAsset>(basePath + "beatmap");
        mapDesc = JsonUtility.FromJson<MapDesc>(mapDescAssert.text);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>(basePath + mapDesc.music);
        audioSource.Play();
    }

    void Update()
    {
        HandleTouch();
        HandleClick();
        if (audioSource.isPlaying)
        {
            var audioTime = audioSource.time;
            var timeLines = mapDesc.timeLines;
            if (timeLineClip + 1 >= timeLines.Length
                || timeLines[timeLineClip + 1].offset > audioTime)
            {
                CheckAndGenLine(timeLines[timeLineClip].lines, audioTime);
            }
            else
            {
                timeLineClip += 1;
                lineClip = 0;
            }
        }
        else
        {
            // TODO 音乐放完了，游戏结束
        }
    }

    void CheckAndGenLine(LineItem[] lines, float audioTime)
    {
        if (lineClip >= 0 && lineClip < lines.Length)
        {
            var currentLine = lines[lineClip];
            var lineStart = currentLine.offset;
            var lineEnd = lineStart + currentLine.duration;
            if (audioTime >= lineStart - Line.leadTime && audioTime < lineEnd)
            {
                Line lineObject = Instantiate(linePrefab);
                lineObject.lineInfo = currentLine;
                lineObject.audioSource = audioSource;
                lineObject.transform.parent = transform;
                lineClip++;
                CheckAndGenLine(lines, audioTime);
            }
        }
    }

    void HandleTouch()
    {
        if (Input.touchSupported)
        {
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    OnTouchDown(Camera.main.ScreenToWorldPoint(touch.position));
                }
            }
        }
    }

    void OnTouchDown(Vector2 position)
    {
        float min = Line.triggerGapY;
        Transform nearlyLine = null;
        foreach (Transform child in transform)
        {
            if ("Line(Clone)" == child.name)
            {
                float gap = System.Math.Abs(position.y - child.position.y);
                if (gap < min)
                {
                    min = gap;
                    nearlyLine = child;
                }
            }
        }
        if (nearlyLine != null)
        {
            nearlyLine.GetComponent<Line>().OnTouchDown(position);
        }
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnTouchDown(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}

[Serializable]
public class MapDesc
{
    public string name;
    public string artist;
    public string creator;
    public string music;
    public string backgroundImage;
    public string skin;
    public Preview preview;
    public TimeItem[] timeLines;
}

[Serializable]
public class Preview
{
    public float offset;
    public float duration;
}

[Serializable]
public class TimeItem
{
    public float offset;
    public string beat;
    public float bpm;
    public LineItem[] lines;
}

[Serializable]
public class LineItem
{
    public float offset;
    public float duration;
    public int y;
    public EventItem[] events;
}

[Serializable]
public class EventItem
{
    public float offset;
    public string type;
    public int x;
    public float duration;
}