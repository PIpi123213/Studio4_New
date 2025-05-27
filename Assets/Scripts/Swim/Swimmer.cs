using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Swimmer : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Values")]
    [SerializeField] float swimForce = 2f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float minForce;
    [SerializeField] float minTimeBetweenStrokes;

    [Header("Reference")]
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference leftControllerVelocity;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] InputActionReference rightControllerVelocity;

    [SerializeField] Transform trackingReference;
    Rigidbody _rigidbody;
    float _cooldownTimer;

    [Header("Gravity")]
    public bool _useGravity = false;
    public float gravityMultiplier = 0.01f;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
    private void FixedUpdate()
    {
        _cooldownTimer += Time.fixedDeltaTime;
        if (_cooldownTimer > minTimeBetweenStrokes
            && (leftControllerSwimReference.action.IsPressed()
            || rightControllerSwimReference.action.IsPressed()))
        {
            var leftHandVelocity = leftControllerVelocity.action.ReadValue<Vector3>();
            var rightHandVelocity = rightControllerVelocity.action.ReadValue<Vector3>();
            Vector3 loaclVelocity = leftHandVelocity + rightHandVelocity;
            loaclVelocity *= -1;

            if (loaclVelocity.sqrMagnitude > minForce * minForce)
            {
                Vector3 worldVelocity = trackingReference.TransformDirection(loaclVelocity);
                _rigidbody.AddForce(worldVelocity * swimForce, ForceMode.Acceleration);
                _cooldownTimer = 0f;
            }
        }


      /*  if (_cooldownTimer > minTimeBetweenStrokes)
        {
            var leftHandVelocity = leftControllerVelocity.action.ReadValue<Vector3>();
            var rightHandVelocity = rightControllerVelocity.action.ReadValue<Vector3>();
            Vector3 loaclVelocity = leftHandVelocity + rightHandVelocity;
            loaclVelocity *= -1;

            if (loaclVelocity.sqrMagnitude > minForce * minForce)
            {
                Vector3 worldVelocity = trackingReference.TransformDirection(loaclVelocity);
                _rigidbody.AddForce(worldVelocity * swimForce, ForceMode.Acceleration);
                _cooldownTimer = 0f;
            }
        }*/
        if (_rigidbody.velocity.sqrMagnitude > 0.01f)
        {
            _rigidbody.AddForce(-_rigidbody.velocity*dragForce,ForceMode.Acceleration);
        }
        if (_useGravity)
        {
            _rigidbody.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }

    }
    

}
