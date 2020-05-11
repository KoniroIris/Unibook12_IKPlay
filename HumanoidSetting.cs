using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class HumanoidSetting : MonoBehaviour
{
    [SerializeField]
    GameObject targetModel;
    Animator _targetAnim;
    GameObject[] fingertip;     // 指先を格納　左手小指[0]から右に向かって数が増える
    GameObject[] iKTargetList;  // IKTargetを格納する0に手首のFullBodyBipedIK、1~10が指先のCCDIK 
    PianoController pianoController;

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
        Initialize();    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            fingerMove(76);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            fingerMove(77);
        if (Input.GetKeyDown(KeyCode.W))
            fingerMove(78);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            fingerMove(79);
        if (Input.GetKeyDown(KeyCode.E))
            fingerMove(80);
        if (Input.GetKeyDown(KeyCode.R))
            fingerMove(81);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            fingerMove(82);
        if (Input.GetKeyDown(KeyCode.T))
            fingerMove(83);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            fingerMove(84);
        if (Input.GetKeyDown(KeyCode.Y))
            fingerMove(85);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            fingerMove(86);
        if (Input.GetKeyDown(KeyCode.U))
            fingerMove(87);
        if (Input.GetKeyDown(KeyCode.I))
            fingerMove(88);
    }

    public void Initialize()
    {
        iKTargetList = GameObject.Find("IKManager").GetComponent<IKManager>().inverseKinematicsArray;

        pianoController = GameObject.Find("Piano").GetComponent<PianoController>();
        _targetAnim = targetModel.GetComponent<Animator>();

        fingertip = new GameObject[12];
        fingertip[0] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.LeftLittleDistal).gameObject);
        fingertip[1] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.LeftRingDistal).gameObject);
        fingertip[2] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).gameObject);
        fingertip[3] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.LeftIndexDistal).gameObject);
        fingertip[4] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.LeftThumbDistal).gameObject);
        fingertip[5] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.RightThumbDistal).gameObject);
        fingertip[6] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.RightIndexDistal).gameObject);
        fingertip[7] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.RightMiddleDistal).gameObject);
        fingertip[8] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.RightRingDistal).gameObject);
        fingertip[9] = SeachFingertips(_targetAnim.GetBoneTransform(HumanBodyBones.RightLittleDistal).gameObject);
        fingertip[10] = _targetAnim.GetBoneTransform(HumanBodyBones.LeftHand).gameObject;
        fingertip[11] = _targetAnim.GetBoneTransform(HumanBodyBones.RightHand).gameObject;

        PlayHandInitialize();
    }

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

    public Vector3 RelativePositionFromKey(PlayFinger playFinger, int fingerNum)
    {
        if (fingerNum < 5)
        {
            return playPosList[playFinger][fingerNum] - pianoController.keyMap[84].gameObject.transform.position;
        }
        if (fingerNum < 10)
        {
            return playPosList[playFinger][fingerNum] - pianoController.keyMap[96].gameObject.transform.position;
        }
        if (fingerNum == 10)
        {
            return playPosList[playFinger][fingerNum] - pianoController.keyMap[84].gameObject.transform.position;
        }
        if (fingerNum == 11)
        {
            return playPosList[playFinger][fingerNum] - pianoController.keyMap[96].gameObject.transform.position;
        }

        return Vector3.zero;
    }

    // 指のIKTargetオブジェクトをモデルの指先に、手首のIKTargetオブジェクトをモデルの手首に移動
    public void OpenHandInitialize()
    {
        Vector3[] playPos = new Vector3[12];
        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            iKTargetList[fingerNum].transform.position = fingertip[fingerNum].transform.position;
            playPos[fingerNum] = fingertip[fingerNum].transform.position;
        }

        playPos[10] = _targetAnim.GetBoneTransform(HumanBodyBones.LeftHand).position;
        playPos[11] = _targetAnim.GetBoneTransform(HumanBodyBones.RightHand).position;
    }

    public void PlayHandInitialize()
    {
        Observable.FromCoroutine(() => PlayHandPosCoroutine()).Subscribe();
    }

    IEnumerator PlayHandPosCoroutine()
    {
        Vector3[][] playPos = new Vector3[50][];

        Vector3 openPos = _targetAnim.GetBoneTransform(HumanBodyBones.LeftIndexProximal).transform.position - Vector3.up * 0.025f;

        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            Vector3 changePos = Vector3.zero;

            changePos = fingertip[fingerNum].transform.position;

            changePos.y = openPos.y;

            iKTargetList[fingerNum].transform.position = changePos;
        }

        yield return new WaitForSeconds(1.5f);

        playPos[(int)PlayFinger.Open] = new Vector3[12];
        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            iKTargetList[fingerNum].transform.position = fingertip[fingerNum].transform.position;
            playPos[(int)PlayFinger.Open][fingerNum] = fingertip[fingerNum].transform.position;
        }

        playPos[(int)PlayFinger.Open][10] = _targetAnim.GetBoneTransform(HumanBodyBones.LeftHand).position;
        playPos[(int)PlayFinger.Open][11] = _targetAnim.GetBoneTransform(HumanBodyBones.RightHand).position;

        playPosList.Add(PlayFinger.Open, playPos[(int)PlayFinger.Open]);

        yield return new WaitForSeconds(1.5f);

        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            Vector3 changePos = Vector3.zero;

            changePos = fingertip[fingerNum].transform.position;

            changePos.y -= 0.1f;

            iKTargetList[fingerNum].transform.position = changePos;
        }

        yield return new WaitForSeconds(1.5f);

        playPos[(int)PlayFinger.Close] = new Vector3[12];
        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            iKTargetList[fingerNum].transform.position = fingertip[fingerNum].transform.position;
            playPos[(int)PlayFinger.Close][fingerNum] = fingertip[fingerNum].transform.position;
        }

        playPos[(int)PlayFinger.Close][10] = _targetAnim.GetBoneTransform(HumanBodyBones.LeftHand).position;
        playPos[(int)PlayFinger.Close][11] = _targetAnim.GetBoneTransform(HumanBodyBones.RightHand).position;

        playPosList.Add(PlayFinger.Close, playPos[(int)PlayFinger.Close]);

        yield return new WaitForSeconds(1.5f);

        for (int poseNum = 0; poseNum < 20; poseNum++)
        {
            playPos[poseNum] = new Vector3[12];

            if (poseNum % 10 < 5)
                iKTargetList[10].transform.position = pianoController.keyMap[84].gameObject.transform.position - RelativePositionFromHand(PlayFinger.Close, poseNum % 10);
            else
                iKTargetList[11].transform.position = pianoController.keyMap[96].gameObject.transform.position - RelativePositionFromHand(PlayFinger.Close, poseNum % 10);

            for (int fingerNum = 0; fingerNum < 10; fingerNum++)
            {
                if (poseNum == fingerNum && poseNum < 10)
                {
                    if (fingerNum < 5)
                        iKTargetList[fingerNum].transform.position = iKTargetList[10].transform.position + RelativePositionFromHand(PlayFinger.Close, fingerNum);
                    else
                        iKTargetList[fingerNum].transform.position = iKTargetList[11].transform.position + RelativePositionFromHand(PlayFinger.Close, fingerNum);
                }
                else
                {
                    if (fingerNum < 5)
                        iKTargetList[fingerNum].transform.position = iKTargetList[10].transform.position + RelativePositionFromHand(PlayFinger.Open, fingerNum);
                    else
                        iKTargetList[fingerNum].transform.position = iKTargetList[11].transform.position + RelativePositionFromHand(PlayFinger.Open, fingerNum);
                }
            }

            yield return new WaitForSeconds(0.1f);

            for (int fingerNum = 0; fingerNum < 12; fingerNum++)
            {
                playPos[poseNum][fingerNum] = iKTargetList[fingerNum].transform.position;
            }
        }

        for (int fingerNum = 0; fingerNum < 20; fingerNum++)
        {
            playPosList.Add(HumanoidSetting.PlayFinger.L_Pinky + fingerNum, playPos[fingerNum]);
        }
    }

    GameObject SeachFingertips(GameObject fingerObj)
    {
        if (fingerObj.transform.childCount > 0)
        {
            return fingerObj.transform.GetChild(0).gameObject;
        }
        return fingerObj;
    }

    public void fingerMove(int poseNum)
    {

        if (poseNum % 10 < 5)
            iKTargetList[10].transform.position = pianoController.keyMap[84].gameObject.transform.position - RelativePositionFromHand(PlayFinger.Close, poseNum % 10);
        else
            iKTargetList[11].transform.position = pianoController.keyMap[96].gameObject.transform.position - RelativePositionFromHand(PlayFinger.Close, poseNum % 10);

        for (int fingerNum = 0; fingerNum < 10; fingerNum++)
        {
            if (poseNum == fingerNum && poseNum < 10)
            {
                if (fingerNum < 5)
                    iKTargetList[fingerNum].transform.position = iKTargetList[10].transform.position + RelativePositionFromHand(PlayFinger.Close, fingerNum);
                else
                    iKTargetList[fingerNum].transform.position = iKTargetList[11].transform.position + RelativePositionFromHand(PlayFinger.Close, fingerNum);
            }
            else
            {
                if (fingerNum < 5)
                    iKTargetList[fingerNum].transform.position = iKTargetList[10].transform.position + RelativePositionFromHand(PlayFinger.Open, fingerNum);
                else
                    iKTargetList[fingerNum].transform.position = iKTargetList[11].transform.position + RelativePositionFromHand(PlayFinger.Open, fingerNum);
            }
        }
        return;
    }

}
