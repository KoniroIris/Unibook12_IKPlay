using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class HumanoidSetting : MonoBehaviour
{
    [SerializeField]
    GameObject _targetModel;     // 今回演奏するキャラクターオブジェクト
    Animator _targetAnim;       // キャラクターオブジェクトのアニメーター
    GameObject[] _fingertip;     // 指先を格納　左手小指[0]から右に向かって数が増える
    GameObject[] _iKTargetList;  // IKTargetを格納する0に手首のFullBodyBipedIK、1~10が指先のCCDIK 
    PianoController _pianoController;

    public enum PlayFinger
    {
        Open,
        Close,
        L_Pinky,
        L_Ring,
        L_Middle,
        L_Index,
        L_Thumb,
        R_Thumb,
        R_Index,
        R_Middle,
        R_Ring,
        R_Pinky,
        L_Pinky_Wait,
        L_Ring_Wait,
        L_Middle_Wait,
        L_Index_Wait,
        L_Thumb_Wait,
        R_Thumb_Wait,
        R_Index_Wait,
        R_Middle_Wait,
        R_Ring_Wait,
        R_Pinky_Wait,
    }

    public IDictionary<PlayFinger, Vector3[]> playPosList = new Dictionary<PlayFinger, Vector3[]>();

    void Start()
    {
        _iKTargetList = GameObject.Find("IKManager").GetComponent<IKManager>().HandIKList;
        _pianoController = GameObject.Find("Piano").GetComponent<PianoController>();
        _targetAnim = _targetModel.GetComponent<Animator>();

        //  指先のGameObjectをHumanBodyBonesから取得
        _fingertip = new GameObject[12];
        _fingertip[0] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.LeftLittleDistal).gameObject);
        _fingertip[1] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.LeftRingDistal).gameObject);
        _fingertip[2] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).gameObject);
        _fingertip[3] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.LeftIndexDistal).gameObject);
        _fingertip[4] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.LeftThumbDistal).gameObject);
        _fingertip[5] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.RightThumbDistal).gameObject);
        _fingertip[6] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.RightIndexDistal).gameObject);
        _fingertip[7] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.RightMiddleDistal).gameObject);
        _fingertip[8] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.RightRingDistal).gameObject);
        _fingertip[9] = GetDeepestChildren(_targetAnim.GetBoneTransform(HumanBodyBones.RightLittleDistal).gameObject);

        //  手首のGameObjectをHumanBodyBonesから取得
        _fingertip[10] = _targetAnim.GetBoneTransform(HumanBodyBones.LeftHand).gameObject;
        _fingertip[11] = _targetAnim.GetBoneTransform(HumanBodyBones.RightHand).gameObject;

        //  キャラクター演奏アニメーションの初期設定
        PlayHandInitialize();
    }

    void Update()
    {
        //  入力キーから再生する演奏アニメーションを選択
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _pianoController.Play(84, 0.5f);
            HandIKMove(84, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _pianoController.Play(85, 0.5f);
            HandIKMove(85, 1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _pianoController.Play(86, 0.5f);
            HandIKMove(86, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _pianoController.Play(87, 0.5f);
            HandIKMove(87, 3);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _pianoController.Play(88, 0.5f);
            HandIKMove(88, 4);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _pianoController.Play(89, 0.5f);
            HandIKMove(89, 5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _pianoController.Play(90, 0.5f);
            HandIKMove(90, 6);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            _pianoController.Play(91, 0.5f);
            HandIKMove(91, 6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _pianoController.Play(92, 0.5f);
            HandIKMove(92, 7);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _pianoController.Play(93, 0.5f);
            HandIKMove(93, 7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _pianoController.Play(94, 0.5f);
            HandIKMove(94, 9);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            _pianoController.Play(95, 0.5f);
            HandIKMove(95, 8);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            _pianoController.Play(96, 0.5f);
            HandIKMove(96, 9);
        }
    }

    /// <summary>
    /// ポーズ毎の手首から指の相対距離を返す
    /// fingerNumが0～4なら左手首の処理
    /// fingerNumが5～9なら右手首の処理
    /// </summary>
    /// <param name="playFinger">ポーズの指定</param>
    /// <param name="fingerNum">指の指定</param>
    /// <returns></returns>
    public Vector3 RelativePositionFromHand(PlayFinger playFinger, int fingerNum)
    {
        if (fingerNum < 5)
        {
            return playPosList[playFinger][fingerNum] - playPosList[playFinger][10];
        }
        if (fingerNum < 10)
        {
            return playPosList[playFinger][fingerNum] - playPosList[playFinger][11];
        }

        return Vector3.zero;
    }

    public void PlayHandInitialize()
    {
        Observable.FromCoroutine(() => PlayHandPosInitCoroutine()).Subscribe();
    }

    /// <summary>
    /// 手首と指のIKターゲットが演奏する時のポーズを設定する
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayHandPosInitCoroutine()
    {
        Vector3[][] playPos = new Vector3[50][];

        Vector3 changePos = Vector3.zero;

        // 手を開いている状態のポーズを設定
        /*
        Vector3 openPos = _targetAnim.GetBoneTransform(HumanBodyBones.LeftIndexProximal).transform.position - Vector3.up * 0.025f;

        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            Vector3 changePos = Vector3.zero;

            changePos = _fingertip[fingerNum].transform.position;

            changePos.z = openPos.z + 1;

            _iKTargetList[fingerNum].transform.position = changePos;
        }

        yield return new WaitForSeconds(1.5f);
        */

        ///////////////////////////////////////////////////////////////////////////////////


        yield return new WaitForSeconds(0.3f);

        playPos[(int)PlayFinger.Open] = new Vector3[12];

        // 手を開いている状態のポーズを設定
        changePos = Vector3.zero;
        changePos.y += 0.05f;
        changePos.z += 1;
        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            _iKTargetList[fingerNum].transform.position = _fingertip[fingerNum].transform.position + changePos;
            playPos[(int)PlayFinger.Open][fingerNum] = _fingertip[fingerNum].transform.position;
        }
        _iKTargetList[10].transform.position = _pianoController.keyMap[84].gameObject.transform.position + Vector3.up * 0.05f;
        _iKTargetList[11].transform.position = _pianoController.keyMap[96].gameObject.transform.position + Vector3.up * 0.05f;

        yield return new WaitForSeconds(0.3f);

        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            playPos[(int)PlayFinger.Open][fingerNum] = _fingertip[fingerNum].transform.position;
        }
        playPos[(int)PlayFinger.Open][10] = _targetAnim.GetBoneTransform(HumanBodyBones.LeftHand).position;
        playPos[(int)PlayFinger.Open][11] = _targetAnim.GetBoneTransform(HumanBodyBones.RightHand).position;

        playPosList.Add(PlayFinger.Open, playPos[(int)PlayFinger.Open]);

        yield return new WaitForSeconds(0.3f);

        ///////////////////////////////////////////////////////////////////////////////////

        // 10本全ての打鍵時の指のポーズを設定
        playPos[(int)PlayFinger.Close] = new Vector3[12];
        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            _iKTargetList[fingerNum].transform.position = _fingertip[fingerNum].transform.position + Vector3.down;
            playPos[(int)PlayFinger.Close][fingerNum] = _fingertip[fingerNum].transform.position;
        }

        yield return new WaitForSeconds(0.3f);

        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            playPos[(int)PlayFinger.Close][fingerNum] = _fingertip[fingerNum].transform.position;
        }
        playPos[(int)PlayFinger.Close][10] = _targetAnim.GetBoneTransform(HumanBodyBones.LeftHand).position;
        playPos[(int)PlayFinger.Close][11] = _targetAnim.GetBoneTransform(HumanBodyBones.RightHand).position;

        playPosList.Add(PlayFinger.Close, playPos[(int)PlayFinger.Close]);

        yield return new WaitForSeconds(0.3f);
        
        ///////////////////////////////////////////////////////////////////////////////////

        // 個別の打鍵ポーズを設定
        for (int poseNum = 0; poseNum < 20; poseNum++)
        {
            playPos[poseNum] = new Vector3[12];

            if (poseNum % 10 < 5)
                _iKTargetList[10].transform.position = _pianoController.keyMap[84].gameObject.transform.position - RelativePositionFromHand(PlayFinger.Close, poseNum % 10);
            else
                _iKTargetList[11].transform.position = _pianoController.keyMap[96].gameObject.transform.position - RelativePositionFromHand(PlayFinger.Close, poseNum % 10);

            for (int fingerNum = 0; fingerNum < 10; fingerNum++)
            {
                if (poseNum == fingerNum && poseNum < 10)
                {
                    if (fingerNum < 5)
                        _iKTargetList[fingerNum].transform.position = _iKTargetList[10].transform.position + RelativePositionFromHand(PlayFinger.Close, fingerNum);
                    else
                        _iKTargetList[fingerNum].transform.position = _iKTargetList[11].transform.position + RelativePositionFromHand(PlayFinger.Close, fingerNum);
                }
                else
                {
                    if (fingerNum < 5)
                        _iKTargetList[fingerNum].transform.position = _iKTargetList[10].transform.position + RelativePositionFromHand(PlayFinger.Open, fingerNum);
                    else
                        _iKTargetList[fingerNum].transform.position = _iKTargetList[11].transform.position + RelativePositionFromHand(PlayFinger.Open, fingerNum);
                }
            }

            yield return new WaitForSeconds(0.2f);

            for (int fingerNum = 0; fingerNum < 12; fingerNum++)
            {
                playPos[poseNum][fingerNum] = _iKTargetList[fingerNum].transform.position;
            }
        }

        for (int fingerNum = 0; fingerNum < 20; fingerNum++)
        {
            playPosList.Add(HumanoidSetting.PlayFinger.L_Pinky + fingerNum, playPos[fingerNum]);
        }
    }

    /// <summary>
    /// GameObjectの一番下の子供のオブジェクトを返す
    /// </summary>
    /// <param name="parentObj"></param>
    /// <returns></returns>
    GameObject GetDeepestChildren(GameObject parentObj)
    {
        if (parentObj.transform.childCount > 0)
        {
            return parentObj.transform.GetChild(0).gameObject;
        }
        return parentObj;
    }

    /// <summary>
    /// 指と手首ののIK位置を動かす
    /// </summary>
    /// <param name="nodeNum">演奏するノード番号</param>
    public void HandIKMove(int nodeNum, int useFingerNum)
    {
        // 手首のIKターゲットの位置設定
        if(useFingerNum < 5)
            _iKTargetList[10].transform.position = _pianoController.keyMap[nodeNum].gameObject.transform.position - RelativePositionFromHand(PlayFinger.Close, useFingerNum);
        else
            _iKTargetList[11].transform.position = _pianoController.keyMap[nodeNum].gameObject.transform.position - RelativePositionFromHand(PlayFinger.Close, useFingerNum);


        // 各指のIKターゲットの位置設定
        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            if (useFingerNum == fingerNum)
            {
                if (fingerNum < 5)
                    _iKTargetList[fingerNum].transform.position = _iKTargetList[10].transform.position + RelativePositionFromHand(PlayFinger.Close, fingerNum);
                else
                    _iKTargetList[fingerNum].transform.position = _iKTargetList[11].transform.position + RelativePositionFromHand(PlayFinger.Close, fingerNum);
            }
            else
            {
                if (fingerNum < 5)
                    _iKTargetList[fingerNum].transform.position = _iKTargetList[10].transform.position + RelativePositionFromHand(PlayFinger.Open, fingerNum);
                else
                    _iKTargetList[fingerNum].transform.position = _iKTargetList[11].transform.position + RelativePositionFromHand(PlayFinger.Open, fingerNum);
            }
        }
        return;
    }

}
