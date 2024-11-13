using Meta.XR.InputActions;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ArrowToBowInteraction : MonoBehaviour
{
    [SerializeField] private GrabInteractable stringGrabInteractable;
    [SerializeField] private Grabbable stringGrabbable;
    [SerializeField] private float maxPullLength;
    [SerializeField] private Transform attachPoint;
    private ArrowInteraction arrowInteraction;
    private GrabInteractor arrowGrabInteractor;
    private float pullStrengnth;
    private Vector3 pullPosition;
    private Vector3 pullDirection;
    private Vector3 targetDirection;
    private bool isArrowAttached;

    public float PullStrength { get => pullStrengnth; }

    public GrabInteractor stringGrabInteractor { get => arrowGrabInteractor; }

    public bool IsArrowAttached { get => isArrowAttached; }

    private Vector3 initialStringPosition;

    private void Awake()
    {
        initialStringPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        stringGrabInteractable.WhenSelectingInteractorViewRemoved += StringGrabInteractable_WhenSelectingInteractorViewRemoved;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (arrowInteraction != null)
        {
            return;
        }

        if (other.TryGetComponent(out ArrowInteraction arrow))
        {
            HandleArrowSnapped(arrow);
        }
    }

    private void HandleArrowSnapped(ArrowInteraction arrow)
    {
        arrowGrabInteractor = arrow.ArrowInteractor;
        if (arrowGrabInteractor == null
            || arrowGrabInteractor.State != InteractorState.Select)
        {
            return;
        }

        if (arrowGrabInteractor.CanSelect(stringGrabInteractable))
        {
            arrowGrabInteractor.ForceRelease();

            arrowGrabInteractor.ForceSelect(stringGrabInteractable);
            arrowGrabInteractor.SetComputeShouldUnselectOverride(() => !ReferenceEquals(stringGrabInteractable, arrowGrabInteractor.SelectedInteractable), true);

            arrowInteraction = arrow;
            isArrowAttached = true;
            return;
        }
    }

    private void Update()
    {
        pullStrengnth = CalculatePull(transform.localPosition);
        if (!isArrowAttached)
            return;
        arrowInteraction.transform.localPosition = attachPoint.position;
        arrowInteraction.transform.localRotation = attachPoint.rotation;
    }

    private void StringGrabInteractable_WhenSelectingInteractorViewRemoved(IInteractorView obj)
    {
        ResetString();
    }

    private void ResetString()
    {
        if (arrowInteraction != null)
        {
            arrowInteraction.ReleaseArrow(CalculatePull(transform.localPosition));
            isArrowAttached = false;
            pullStrengnth = 0;
            arrowInteraction = null;
        }
        stringGrabbable.enabled = false;
        transform.localPosition = initialStringPosition;
        stringGrabbable.enabled = true;
    }

    private float CalculatePull(Vector3 pullPosition)
    {
        this.pullDirection = pullPosition - initialStringPosition;
        targetDirection = new Vector3(0, 0, maxPullLength);
        float maxLength = targetDirection.magnitude;

        targetDirection.Normalize();

        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
        return Mathf.Clamp(pullValue, 0, 1);
    }
}
