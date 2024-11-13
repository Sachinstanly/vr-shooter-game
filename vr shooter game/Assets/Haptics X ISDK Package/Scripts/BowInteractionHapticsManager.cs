using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Haptics;
using Oculus.Interaction;

public class BowInteractionHapticsManager : MonoBehaviour
{
    [SerializeField] private GrabInteractable bowGrabInteractable;
    [SerializeField] private GrabInteractable stringGrabInteractable;

    [SerializeField] private HapticClip stringPullHapticClip;
    [SerializeField] private HapticClip arrowShotHapticClip;
    [SerializeField] private HapticClip shieldOpenHapticClip;
    [SerializeField] private HapticClip projectileBlockedClip;

    private HapticClipPlayer stringPullHapticClipPlayer;
    private HapticClipPlayer arrowShotHapticClipPlayer;
    private HapticClipPlayer shieldHapticClipPlayer;
    private HapticClipPlayer projectileBlockedClipPlayer;

    private ArrowToBowInteraction arrowToBowInteraction;

    private Shield shield;

    private GrabInteractor bowInteractor;
    private GrabInteractor stringInteractor;

    private Controller stringController;
    private Controller bowController;

    private bool isStringGrabbed;

    private void Awake()
    {
        stringPullHapticClipPlayer = new HapticClipPlayer(stringPullHapticClip);
        arrowShotHapticClipPlayer = new HapticClipPlayer(arrowShotHapticClip);
        shieldHapticClipPlayer = new HapticClipPlayer(shieldOpenHapticClip);
        projectileBlockedClipPlayer = new HapticClipPlayer(projectileBlockedClip);

        shieldHapticClipPlayer.priority = 1;
        arrowShotHapticClipPlayer.priority = 2;
        projectileBlockedClipPlayer.priority = 3;

        shield = GetComponentInChildren<Shield>();
        arrowToBowInteraction = GetComponentInChildren<ArrowToBowInteraction>();
    }

    private void OnEnable()
    {
        bowGrabInteractable.WhenSelectingInteractorAdded.Action += WhenBowSelectingInteractorAdded;
        bowGrabInteractable.WhenSelectingInteractorRemoved.Action += WhenBowSelectingInteractorRemoved;

        stringGrabInteractable.WhenSelectingInteractorAdded.Action += WhenStringSelectingInteractorAdded;
        stringGrabInteractable.WhenSelectingInteractorRemoved.Action += WhenStringSelectingInteractorRemoved;

        shield.onShieldOpen.AddListener(PlayHapticsOnShieldOpen);
        shield.onShieldClose.AddListener(PlayHapticsOnShieldOpen);
        shield.onShieldHit.AddListener(PlayHapticsOnProjectileBlocked);
    }

    private void WhenBowSelectingInteractorAdded(GrabInteractor obj)
    {
        bowInteractor = obj;
        bowController = bowInteractor.GetComponentInParent<HapticsController>().Controller;
    }

    private void WhenBowSelectingInteractorRemoved(GrabInteractor obj)
    {
        bowInteractor = null;
    }

    private void WhenStringSelectingInteractorAdded(GrabInteractor obj)
    {
        stringInteractor = obj;
        stringController = stringInteractor.GetComponentInParent<HapticsController>().Controller;
        PlayHapticsOnStringPull();
    }

    public void PlayHapticsOnStringPull()
    {
        if (stringInteractor == null)
            return;
        stringPullHapticClipPlayer.isLooping = true;
        isStringGrabbed = true;
        stringPullHapticClipPlayer.Play(stringController);
    }

    private void Update()
    {
        if (isStringGrabbed)
        {
            stringPullHapticClipPlayer.amplitude = arrowToBowInteraction.PullStrength;
        }
    }

    private void WhenStringSelectingInteractorRemoved(GrabInteractor obj)
    {
        stringPullHapticClipPlayer.Stop();
        PlayHapticsOnArrowShot();
        stringInteractor = null;
        isStringGrabbed = false;
    }

    public void PlayHapticsOnArrowShot()
    {
        if (bowInteractor == null)
            return;
        arrowShotHapticClipPlayer.Play(bowController);
    }

    public void PlayHapticsOnShieldOpen()
    {
        if (bowInteractor == null)
            return;
        shieldHapticClipPlayer.Play(bowController);
        StartCoroutine(PlayShieldHaptic());
    }

    public void StopHapticsOnShieldClose()
    {
        if (bowInteractor == null)
            return;
        shieldHapticClipPlayer.Play(bowController);
        StartCoroutine(PlayShieldHaptic());
    }

    private IEnumerator PlayShieldHaptic()
    {
        yield return new WaitForSeconds(shield.AnimationDutation);
        shieldHapticClipPlayer.Stop();
    }

    private void PlayHapticsOnProjectileBlocked()
    {
        if (bowInteractor == null)
            return;
        projectileBlockedClipPlayer.Play(bowController);
    }
}
