using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType {Tree, Rock}
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float gravity = -10;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed;
    private int _velocityAnimHash;
    private int _isMiningAnimHash;
    private bool _isMining;
    [SerializeField] private GameObject pickaxe;
    public delegate void InteractionEnter(InteractionType interactionType);

    public delegate void InteractionExit();

    public event InteractionEnter OnInteractionEnter;
    public event InteractionExit OnInteractionExit;

    public static PlayerController Singleton { get; private set; }

    private void Awake()
    {
        if (!Singleton)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _velocityAnimHash = Animator.StringToHash("Velocity");
        _isMiningAnimHash = Animator.StringToHash("IsMining");
    }
    private void FixedUpdate()
    {
        Vector3 inputVelocity = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        Vector3 moveVector = new Vector3(0, gravity, 0) + (inputVelocity * moveSpeed);
        characterController.Move(moveVector * Time.fixedDeltaTime);

        animator.SetFloat(_velocityAnimHash, inputVelocity.magnitude);

        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(inputVelocity);
            if (_isMining)
            {
                StopMining();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tree"))
        {
            OnInteractionEnter?.Invoke(InteractionType.Tree);
        }

        if (other.gameObject.CompareTag("Rock"))
        {
            OnInteractionEnter?.Invoke(InteractionType.Rock);
        }
    }

    public void StartMining()
    {
        _isMining = true;
        pickaxe.SetActive(true);
        animator.SetBool(_isMiningAnimHash, _isMining);
    }

    public void StopMining()
    {
        pickaxe.SetActive(false);
        _isMining = false;
        animator.SetBool(_isMiningAnimHash, _isMining);
    }

    private void OnTriggerExit(Collider other)
    {
        OnInteractionExit?.Invoke();
    }
}
