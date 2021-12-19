using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject mineTreeBtn;
    [SerializeField] private GameObject mineRockBtn;

    private void Start()
    {
        PlayerController.Singleton.OnInteractionEnter += OnInteractionEnter;
        PlayerController.Singleton.OnInteractionExit += OnInteractionExit;
    }

    private void OnInteractionExit()
    {
        mineTreeBtn.SetActive(false);
        mineRockBtn.SetActive(false);
    }

    private void OnInteractionEnter(InteractionType interactionType)
    {
        switch (interactionType)
        {
            case InteractionType.Tree:
                mineTreeBtn.SetActive(true);
                break;
            case InteractionType.Rock:
                mineRockBtn.SetActive(true);
                break;
        }
    }
}
