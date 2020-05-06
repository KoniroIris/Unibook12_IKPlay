using RootMotion.FinalIK;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    public GameObject[] inverseKinematicsArray { get; set; }
    [SerializeField]
    public IK[] components;
    private bool updateFrame;
    void Awake()
    {
        inverseKinematicsArray = new GameObject[12];
        for (int fingerCount = 0; fingerCount < 12; fingerCount++)
        {
            inverseKinematicsArray[fingerCount] = transform.GetChild(fingerCount).gameObject;
        }
    }

    void Start()
    {
        foreach (IK component in components) component.Disable();
    }
    void FixedUpdate()
    {
        updateFrame = true;
    }
    void LateUpdate()
    {
        updateFrame = false;
        foreach (IK component in components) component.GetIKSolver().Update();
    }
}
