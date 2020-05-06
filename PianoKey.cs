using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKey
{
    public GameObject gameObject { get; private set; }

    bool isPushed;

    public PianoKey(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
}
