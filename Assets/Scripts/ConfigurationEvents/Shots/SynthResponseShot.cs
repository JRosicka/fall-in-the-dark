﻿using UnityEngine;

/// <summary>
/// The shot that goes "bya bya byaaa BA-BA"
/// Fires ring of static rotating shots
///
/// TODO: I think this functionality was moved to <see cref="BassPattern"/>, so we probably don't need this anymore
/// </summary>
public class SynthResponseShot : MonoBehaviour {
    public BulletParticleSystem BSystem;

    public void Shoot(bool clockwise) {
        if (!clockwise) {
            BSystem.GetActiveParticleSystems().ForEach(e => e.transform.Rotate(180, 0, 0));
        }
        BSystem.Shoot();
    }
}
