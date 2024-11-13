using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Samples;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Oculus.Interaction.Input;
using UnityEngine.Events;

public class BowInteraction : MonoBehaviour
{
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private GrabInteractable interactable;

    private bool isBowGrabbed = false;

    private ControllerRef controller;

    public ControllerButtonUsageActiveState controllerButtonUsageActiveSate;
    public UnityEvent bowActivated;

    private void OnEnable()
    {
        interactable.WhenSelectingInteractorAdded.Action += WhenSelectingInteractorAdded_Action;
        interactable.WhenSelectingInteractorViewRemoved += Interactable_WhenSelectingInteractorViewRemoved;
    }

    private void WhenSelectingInteractorAdded_Action(GrabInteractor obj)
    {
        isBowGrabbed = true;
        controller = obj.GetComponent<ControllerRef>();
        controllerButtonUsageActiveSate.InjectController(controller);
    }

    private void Interactable_WhenSelectingInteractorViewRemoved(IInteractorView obj)
    {
        isBowGrabbed = false;
    }

    private void Update()
    {
        if (!isBowGrabbed)
            return;
        if (controllerButtonUsageActiveSate.Active)
            bowActivated.Invoke();
    }
}
