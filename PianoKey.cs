using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ピアノの鍵盤を管理する
/// </summary>
public class PianoKey
{
    public GameObject gameObject { get; private set; }

    bool isPushed;

    public PianoKey(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
}
