using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInfo : MonoBehaviour
{
    public float DamageAmount;
    public bool bCausesKnockDown;
    public AudioClip[] AttackSounds;
    public AudioClip ConnectSound;    
    protected PlayerController Player;

    public void OnEnable()
    {
        if (Player == null)
        {
            Player = GetComponentInParent<PlayerController>();
        }

        if (AttackSounds.Length > 0 && Player.EffectPlayer != null)
        {
            if (Player.EffectPlayer.isPlaying)
            {
                Player.EffectPlayer.Stop();
            }
            int RandomSoundIndex = Random.Range(0, AttackSounds.Length);
            Player.EffectPlayer.PlayOneShot(AttackSounds[RandomSoundIndex]);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (ConnectSound != null && Player.EffectPlayer != null)
        {            
            if (Player.EffectPlayer.isPlaying)
            {
                Player.EffectPlayer.Stop();
            }
            Player.EffectPlayer.PlayOneShot(ConnectSound);
        }
    }

}
