using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

public class MoveManager : MonoBehaviour
{
    [SerializeField] private MoveInSpace _movementData;
    public Transform TrackingObject;
    private Vector3 localPosition = Vector3.zero;
    XROrigin xrOrigin;
    Vector3 lastEyeLocalPos;
    public static MoveManager Instance { get; private set; }
    public Vector3 CurrentWorldPosition = Vector3.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  // ��һ�� Awake ʱ��ֵ
        }
        else
        {
            Destroy(gameObject);  // ��ֹ�ظ�����
        }
        _movementData.InitializeSystem();
        CurrentWorldPosition = this.transform.position;
        xrOrigin = GetComponent<XROrigin>();
    }

    void Start()
    {
        lastEyeLocalPos = TrackingObject.localPosition;
    }
    private void OnApplicationQuit()
    {
        _movementData.InitializeSystem();
    }


    void Update()
    {
        if (TryGetDevicePosition(out Vector3 currentPos))
        {
            localPosition = TrackingObject.localPosition;
            _movementData.UpdateOffset(localPosition);
        }
    }
    private bool TryGetDevicePosition(out Vector3 position)
    {
        var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
        {
            return true;
        }

        position = Vector3.zero; // ȷ������·�����и�ֵ
        return false;
    }

    void LateUpdate()
    {
        // ��ǰ CenterEye �ڱ��ؿռ��λ��
        // ��ǰ֡ͷ���ڱ��ؿռ��λ��
        Vector3 currentEyeLocalPos = TrackingObject.localPosition;

        // �����������һ֡��������ֻ�� XZ ƽ�棩
        Vector3 delta = currentEyeLocalPos- lastEyeLocalPos   ;
        Vector3 deltaXZ = new Vector3(delta.x, 0f, delta.z);

        Vector3 deltaWorld = TrackingObject.parent.TransformVector(deltaXZ);
        // ���������Ӧ�õ� XROrigin ��
        //xrOrigin.transform.position += deltaWorld;

        // ���� lastEyeLocalPos��Ϊ��һ֡��׼��
        lastEyeLocalPos = currentEyeLocalPos;

        // ��󣬰� centerEye.localPosition ����Ϊ (0,0,0) ��������Ҫ�ĳ�ʼƫ��
        // ������ XR Rig �����ʱ centerEye �������겢�� (0,0,0)��
        // ���������һ�иĳ���ĳ�ʼ�������ꡣ
        //TrackingObject.localPosition = Vector3.zero;

      

    }
    public void OnSceneIn()
    {
        CurrentWorldPosition = this.transform.position;
        Debug.Log(this.transform.position);


    }

    public void OnSceneOut()
    {
        //CurrentWorldPosition = CurrentWorldPosition + localPosition;
        this.transform.position = CurrentWorldPosition;
        Debug.Log(this.transform.position);


    }
}
