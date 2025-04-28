using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    public Transform otherObject;
    public Transform target;// ��һ������
    public bool isCurrentParent = true; // ��ǰ�Ƿ��Ǹ�����

    private Collider myCollider;
    private Rigidbody myRigidbody;

    // ���ڱ���ԭʼ��Ա任
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Quaternion initialRotationDifference;
    public static AttachAnchor attachAnchor = null;
    public Vector3 positionOffset = Vector3.zero;

    void Start()
    {
        if (otherObject == null)
        {
            Debug.LogError("Other object not assigned!");
            return;
        }
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();
        // ��¼��ʼ����Ա任
        originalLocalPosition = otherObject.localPosition;
        originalLocalRotation = otherObject.localRotation;
        originalLocalScale = otherObject.localScale;

        Vector3 localOffset = target.InverseTransformPoint(transform.position);
        initialLocalPosition = new Vector3(localOffset.x, 0, localOffset.z);

        // ��¼��ʼ��ת����
        initialRotationDifference = Quaternion.Inverse(target.rotation) * transform.rotation;
        UpdatePhysicsState();
    }

    public void ToggleParenting()
    {
        if (isCurrentParent)
        {
            // �� A�Ǹ�B���� �� �л�Ϊ B�Ǹ�A����
            // 1. ���浱ǰ����任
            Vector3 aWorldPos = transform.position;
            Quaternion aWorldRot = transform.rotation;
            Vector3 aWorldScale = transform.lossyScale;

            Vector3 bWorldPos = otherObject.position;
            Quaternion bWorldRot = otherObject.rotation;
            Vector3 bWorldScale = otherObject.lossyScale;

            // 2. �����ǰ���ӹ�ϵ
            otherObject.SetParent(null);

            // 3. �����µĸ��ӹ�ϵ (A��ΪB��������)


            // 4. �ָ�����任
            otherObject.position = bWorldPos;
            otherObject.rotation = bWorldRot;
            SetLossyScale(otherObject, bWorldScale);
            transform.position = aWorldPos;
            transform.rotation = aWorldRot;
            SetLossyScale(transform, aWorldScale);

            transform.SetParent(otherObject);

        }
        else
        {
            // �� B�Ǹ� �� �л��� A�Ǹ�

            // 1. ���浱ǰ����任
            Vector3 aWorldPos = transform.position;
            Quaternion aWorldRot = transform.rotation;
            Vector3 aWorldScale = transform.lossyScale;

            Vector3 bWorldPos = otherObject.position;
            Quaternion bWorldRot = otherObject.rotation;
            Vector3 bWorldScale = otherObject.lossyScale;

            // 2. �����ǰ���ӹ�ϵ
            transform.SetParent(null);

            // 3. �ָ�ԭʼ���ӹ�ϵ (B��ΪA��������)


            // 4. �ָ�����任
            transform.position = aWorldPos;
            transform.rotation = aWorldRot;
            SetLossyScale(transform, aWorldScale);

            otherObject.position = bWorldPos;
            otherObject.rotation = bWorldRot;
            SetLossyScale(otherObject, bWorldScale);

            otherObject.SetParent(transform);
        }


        isCurrentParent = !isCurrentParent;
        UpdatePhysicsState();
    }


    // �л����ӹ�ϵ
    public void ToggleParentingA()
    {
        
       
            // �� B�Ǹ� �� �л��� A�Ǹ�

            // 1. ���浱ǰ����任
            Vector3 aWorldPos = transform.position;
            Quaternion aWorldRot = transform.rotation;
            Vector3 aWorldScale = transform.lossyScale;

            Vector3 bWorldPos = otherObject.position;
            Quaternion bWorldRot = otherObject.rotation;
            Vector3 bWorldScale = otherObject.lossyScale;

            // 2. �����ǰ���ӹ�ϵ
            transform.SetParent(null);

            // 3. �ָ�ԭʼ���ӹ�ϵ (B��ΪA��������)
          

            // 4. �ָ�����任
            transform.position = aWorldPos;
            transform.rotation = aWorldRot;
            SetLossyScale(transform, aWorldScale);

            otherObject.position = bWorldPos;
            otherObject.rotation = bWorldRot;
            SetLossyScale(otherObject, bWorldScale);

            otherObject.SetParent(transform);
        

        isCurrentParent = !isCurrentParent;
        UpdatePhysicsState();
    }
    public void ToggleParentingB()
    {
        
            // �� A�Ǹ�B���� �� �л�Ϊ B�Ǹ�A����

            // 1. ���浱ǰ����任
            Vector3 aWorldPos = transform.position;
            Quaternion aWorldRot = transform.rotation;
            Vector3 aWorldScale = transform.lossyScale;

            Vector3 bWorldPos = otherObject.position;
            Quaternion bWorldRot = otherObject.rotation;
            Vector3 bWorldScale = otherObject.lossyScale;

            // 2. �����ǰ���ӹ�ϵ
            otherObject.SetParent(null);

            // 3. �����µĸ��ӹ�ϵ (A��ΪB��������)


            // 4. �ָ�����任
            otherObject.position = bWorldPos;
            otherObject.rotation = bWorldRot;
            SetLossyScale(otherObject, bWorldScale);
            transform.position = aWorldPos;
            transform.rotation = aWorldRot;
            SetLossyScale(transform, aWorldScale);

            transform.SetParent(otherObject);
        
       

        isCurrentParent = !isCurrentParent;
        UpdatePhysicsState();
    }





    private void SetLossyScale(Transform target, Vector3 worldScale)
    {
        if (target.parent == null)
        {
            target.localScale = worldScale;
        }
        else
        {
            Vector3 parentScale = target.parent.lossyScale;
            target.localScale = new Vector3(
                worldScale.x / parentScale.x,
                worldScale.y / parentScale.y,
                worldScale.z / parentScale.z
            );
        }
    }
    private void UpdatePhysicsState()
    {
        if (myCollider != null)
            myCollider.isTrigger = !isCurrentParent;

        if (myRigidbody != null)
        {
            RigidbodyConstraints rotationConstraints =
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;

            RigidbodyConstraints positionConstraints;

            if (!isCurrentParent)
            {
                // ����λ��
                positionConstraints =
                    RigidbodyConstraints.FreezePositionX |
                    RigidbodyConstraints.FreezePositionY |
                    RigidbodyConstraints.FreezePositionZ;
            }
            else
            {
                // ����λ�ã������ƶ���
                positionConstraints = RigidbodyConstraints.None;
            }

            // ʼ������ת��λ�ø��ݵ�ǰ״̬����
            myRigidbody.constraints = positionConstraints | rotationConstraints;
        }
    }
    // ʾ�������ո���л�
    void Update()
    {
        if (!CharacterClimb.isStart) return;


        if (CharacterClimb.isClimbing)
        {
            if (isCurrentParent)
            {
                ToggleParentingB();
                isCurrentParent = false;
            }
         
          

        }
        else
        {
            if (!isCurrentParent)
            {
              
                ToggleParentingA();
                isCurrentParent = true;
            }
        
        }




        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleParenting();
        }
    }


    void LateUpdate()
    {
        if (!isCurrentParent)
        {
            // ����Ŀ��� Y ����ת
            float targetYRotation = target.eulerAngles.y;

            // ���������� Y ����ת����Ԫ��
            Quaternion rotationY = Quaternion.Euler(0, targetYRotation, 0);

            // �����µ�λ�ã����� Y �᲻�䣩
            Vector3 newPosition = target.position + rotationY * (initialLocalPosition+ positionOffset);
            newPosition.y = transform.position.y; // ����ԭʼ Y ��λ��

            // Ӧ���µ�λ��
            transform.position = newPosition;

            // Ӧ�ó�ʼ��ת����
            transform.rotation = rotationY * initialRotationDifference;
        }
        // ����λ��
       
    }
}
