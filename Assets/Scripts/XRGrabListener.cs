using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabListener : MonoBehaviour
{
    public XRGrabInteractable grabInteractable; // ��Ҫ������ XRGrabInteractable ���
    public bool isTwoHandGrab = false;


    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (grabInteractable.interactorsSelecting.Count == 2)
        {
            isTwoHandGrab = true;

            Debug.Log("�����������屻˫��ץס��");
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (grabInteractable.interactorsSelecting.Count < 2)
        {
            isTwoHandGrab = false;
            Debug.Log("�����������屻���ֻ��ɿ���");
        }
    }
}