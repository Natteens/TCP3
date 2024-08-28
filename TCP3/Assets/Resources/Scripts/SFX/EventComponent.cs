using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EventComponent : MonoBehaviour
{
    public Transform feet;
    public Action OnShootingWithWeapon;
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    [SerializeField] private CharacterController _controller;

    public void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    public void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }

    public void OnShooting()
    {
        OnShootingWithWeapon.Invoke();
    }

    public void OnLandVFX()
    {
        Spawner.Instance.SpawnInWorldServerRpc(feet.position, "landVFX");
        Spawner.Instance.DespawnByTimeInWorld(this.GetComponent<NetworkObject>(), 1f);
    }
}
