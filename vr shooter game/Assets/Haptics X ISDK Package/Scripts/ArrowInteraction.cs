using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowInteraction : MonoBehaviour
{
    [SerializeField] private GrabInteractable arrowGrabInteractable;
    [SerializeField] private Grabbable arrowGrabbable;
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private float speed;
    [SerializeField] private Transform tipPosition;

    private GrabInteractor lastGrabInteractor;
    private bool inAir = false;
    private Vector3 lastPosition;

    public GrabInteractor ArrowInteractor { get => lastGrabInteractor; }

    private void OnEnable()
    {
        arrowGrabInteractable.WhenSelectingInteractorAdded.Action += WhenSelectingInteractorAdded_Action;
    }

    private void OnDisable()
    {
        arrowGrabInteractable.WhenSelectingInteractorAdded.Action -= WhenSelectingInteractorAdded_Action;
    }

    private void WhenSelectingInteractorAdded_Action(GrabInteractor obj)
    {
        lastGrabInteractor = obj;
    }

    private void Awake()
    {
        inAir = false;
        lastPosition = Vector3.zero;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        if (inAir)
        {
            CheckCollision();
            lastPosition = tipPosition.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(lastPosition, tipPosition.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.TryGetComponent(out EnemyHealth enemyhealth))
            {
                StopArrow();
                enemyhealth.ApplyDamage();
            }
        }
    }

    private void StopArrow()
    {
        this.inAir = false;
        this.SetPhysics(false);
        this.transform.gameObject.SetActive(false);
    }

    private void SetPhysics(bool usePhysics)
    {
        _rigidbody.useGravity = usePhysics;
        _rigidbody.isKinematic = !usePhysics;
    }

    public void ReleaseArrow(float value)
    {
        this.inAir = true;
        SetPhysics(true);
        MaskAndFire(value);
        StartCoroutine(RotateWithVelocity());
        this.lastPosition = tipPosition.position;
    }

    private void MaskAndFire(float power)
    {
        arrowGrabbable.enabled = false;
        Vector3 force = transform.forward * power * speed;
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (this.inAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(_rigidbody.velocity);
            this.transform.rotation = newRotation;
            yield return null;
        }
    }
}
