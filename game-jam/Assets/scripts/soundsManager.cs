using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundsManager : MonoBehaviour
{
  [Header("Sound Clips")]
  [SerializeField] private AudioClip landingSound;
  [SerializeField] private AudioClip deathSound;
  [SerializeField] private AudioClip bgm;


  [Header("Sound Sources")]
  [SerializeField] private AudioSource landingSource;
  [SerializeField]  private AudioSource deathSource;
  [SerializeField] private AudioSource bgmSource;
  // Start is called before the first frame update
  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  private void playClip(AudioSource source, AudioClip clip, Boolean loop = false, float volume = 1f)
  {
    source.clip = clip;
    source.volume = volume;
    source.Play();
    source.loop = loop;
  }
}
