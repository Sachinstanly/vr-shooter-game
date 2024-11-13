using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Haptics;

public class HapticsController : MonoBehaviour
{
    [SerializeField] private Controller controller;

    public Controller Controller { get => controller; }
}
