using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PianoController : MonoBehaviour
{
    public Dictionary<int, PianoKey> keyMap { get; private set; }     // intはノード番号、GameObjectは鍵盤のGameObjectを格納


    public Dictionary<int, Vector3> defaultKeyPos { get; private set; }

    int midiDeltaTime;
    int beforeMidiDeltaTime;

    void Awake()
    {
        keyMap = new Dictionary<int, PianoKey>();
        defaultKeyPos = new Dictionary<int, Vector3>();

        foreach (Transform childTransform in gameObject.transform)
        {
            keyMap.Add(ConvertStringkeyToNodenumber(childTransform.name), new PianoKey(childTransform.gameObject));
            defaultKeyPos.Add(ConvertStringkeyToNodenumber(childTransform.name), childTransform.position);
        }
    }

    // String型のノード情報をint型ノード番号に変換
    int ConvertStringkeyToNodenumber(string key)
    {
        int result = 0;

        key = key.Substring(4, key.Length - 4);

        switch (key[0])
        {
            case 'C':
                result += 0;
                break;
            case 'D':
                result += 2;
                break;
            case 'E':
                result += 4;
                break;
            case 'F':
                result += 5;
                break;
            case 'G':
                result += 7;
                break;
            case 'A':
                result += 9;
                break;
            case 'B':
                result += 11;
                break;
            default:
                Debug.Log("Keyの入力エラー");
                return -1;
        }
        result += 12;
        result += int.Parse(key[1].ToString()) * 12;

        if (key.Length == 3)
            result += 1;

        return result;
    }


    public void Play(int nodeNum, float playTime)
    {
        Debug.Log(nodeNum + "/" + playTime);
        Observable.FromCoroutine(() => KeyDownCoroutine(nodeNum, playTime)).Subscribe();

    }

    IEnumerator KeyDownCoroutine(int nodeNum, float playTime)
    {
        int useFlame = (int)(playTime / 0.0166) / 3;

        for (int downCount = 0; downCount < useFlame; downCount++)
        {
            keyMap[nodeNum].gameObject.transform.position = keyMap[nodeNum].gameObject.transform.position + Vector3.down * 0.0150f / useFlame;
            yield return null;
        }

        for (int holdCount = 0; holdCount < useFlame; holdCount++)
        {
            yield return null;
        }

        for (int upCount = 0; upCount < useFlame; upCount++)
        {
            keyMap[nodeNum].gameObject.transform.position = keyMap[nodeNum].gameObject.transform.position + Vector3.up * 0.0150f / useFlame;
            yield return null;
        }
        keyMap[nodeNum].gameObject.transform.position = defaultKeyPos[nodeNum];
    }
}
