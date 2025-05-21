using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabListener : XRGrabInteractable
{
    public Transform rightHandPos;
    public Transform leftHandPos;
    public override Transform GetAttachTransform(IXRInteractor interactor)
    {
        handDataPose handData = interactor.transform.GetComponentInChildren<handDataPose>();
        if (handData.type == handDataPose.HandModelType.Right&& rightHandPos!=null)
        {
            Transform _transform = rightHandPos;
            //_transform.rotation = Quaternion.identity;
            return _transform;
        }
        else if (handData.type == handDataPose.HandModelType.Left && leftHandPos != null)
        {
            Transform _transform = leftHandPos;
            //_transform.rotation = Quaternion.identity;
            return _transform;
        }
        else
        {
            return this.transform;
        }
    }
}