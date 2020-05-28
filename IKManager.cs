using RootMotion.FinalIK;
using UnityEngine;

/// <summary>
/// 主にIKの処理順を管理するマネージャクラス
/// </summary>
public class IKManager : MonoBehaviour
{
    public GameObject[] HandIKList { get; set; }     // 手と指のIKターゲットのGameObjectリスト

    [SerializeField]
    public IK[] componentList;                       // IKのコンポーネントリスト
    private bool updateFrame;                        // フレーム内でFixedUpdateを使っているか判断

    void Awake()
    {
        HandIKList = new GameObject[12];
        //  子要素に揃っているIKのGameObjectを取得
        for (int fingerCount = 0; fingerCount < 12; fingerCount++)
        {
            HandIKList[fingerCount] = transform.GetChild(fingerCount).gameObject;
        }

        FixFingers();
    }

    void FixFingers()
    {
        GameObject tmpObj;
        for (int fingerCount = 0; fingerCount < 5; fingerCount++)
        {
            tmpObj = HandIKList[fingerCount];
            HandIKList[fingerCount] = HandIKList[9 - fingerCount];
            HandIKList[9 - fingerCount] = tmpObj;
        }
    }

    void Start()
    {
        //  IKの処理順を変える為、一度Disableする
        foreach (IK component in componentList)
            component.Disable();
    }
    void FixedUpdate()
    {
        updateFrame = true;     
    }
    void LateUpdate()
    {
        updateFrame = false;

        // IKをリスト順にUpdate処理
        foreach (IK component in componentList)
            component.GetIKSolver().Update();
    }
}
