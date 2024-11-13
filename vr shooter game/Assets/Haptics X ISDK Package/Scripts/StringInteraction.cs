using Oculus.Interaction.HandGrab;
using Oculus.Interaction;
using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringInteraction : MonoBehaviour
{
    [SerializeField] private LineRenderer stringLineRenderer;
    [SerializeField] private Transform arrowAttachPoint;

    private Vector3 lastPosition = Vector3.zero;

    private void Update()
    {
        if (arrowAttachPoint.localPosition != lastPosition)
        {
            lastPosition = arrowAttachPoint.localPosition;
            stringLineRenderer.SetPosition(1, lastPosition);
        }
    }
}
