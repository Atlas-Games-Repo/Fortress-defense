using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public enum Direction
{
    Top,
    Bottom,
    Left,
    Right,
    UpperLeft,
    UpperRight,
    BottomLeft,
    BottomRight
}

public enum DialogContent
{
    Text,Video,Image
}

public enum TutorialPlacing
{
    Menu,Game   
}
public enum TipType
{
    Dialog,
    Hint,
    Task
}

[System.Serializable]
public class Tip 
{
    public Direction tipDirection;
    public DialogContent dialogContentType;
    public TipType tipType = TipType.Dialog;
    public Direction pointerDirection;
    public float delay;
    public bool isLastDialog;
    public string uiPartName;
    public float circleMaskScale;
    public bool isUiInteractible;
    public bool pauseGame;
    public string tipText;
    public string tipTitle;
    public Sprite dialogImage;
    public VideoClip dialogVideo;
    public Direction hintDirection;
}
