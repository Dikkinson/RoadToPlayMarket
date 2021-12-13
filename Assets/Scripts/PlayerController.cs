using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed;
    int VelocityHash;
    private void Start()
    {
        VelocityHash = Animator.StringToHash("Velocity");
    }
    private void FixedUpdate()
    {
        Vector3 inputVelocity = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical);
        _characterController.Move((inputVelocity * _moveSpeed) * Time.fixedDeltaTime);

        _animator.SetFloat(VelocityHash, inputVelocity.magnitude);

        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(inputVelocity);
        }
    }
}
