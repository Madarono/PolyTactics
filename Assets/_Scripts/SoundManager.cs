using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public float masterVolume;
    public float backgroundVolume;

    public AudioSource source;

    [Header("UI")]
    public AudioClip[] buttonClicks;
    public AudioClip confirmUpgrade;
    public AudioClip addUpgrade;
    public AudioClip subUpgrade;

    [Header("Tower")]
    public AudioClip basicShoot;
    public AudioClip sniperShoot;
    public AudioClip splashShoot;
    public AudioClip freeze;
    public AudioClip immunity;
    public AudioClip criticalShoot;
    public AudioClip selectTower;
    public AudioClip explosion;
    public AudioClip removeImmunity;
    public AudioClip enemyHit;

    [Header("Placing Tower")]
    public AudioClip changePosition;
    public AudioClip placeTower;
    
    [Header("Base")]
    public AudioClip[] baseHit;

    [Header("Misc")]
    public AudioClip changingSliders;
    public AudioClip endOfRound;
    public AudioClip beginWave;
    public AudioClip fastForward;

    void Awake()
    {
        Instance = this;
    }

    public void PlayClip(AudioClip clip, float amplifier)
    {
        source.PlayOneShot(clip, masterVolume * amplifier);
    }
}
