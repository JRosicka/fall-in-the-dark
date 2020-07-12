﻿using UnityEngine;

// TODO: Move more things here once we know what we'll want for all shot types
/// <summary>
/// A Shot is a ConfigurationEvent that can be spawned into the scene to make bullets appear and generally look pretty and stuff
/// </summary>
public abstract class Shot : ConfigurationEvent {
    public new enum Values {
        Unset = ConfigurationEvent.Values.Unset,
        None = ConfigurationEvent.Values.None,
        FireShot = 1
    }

    public abstract void Shoot(Transform spawner, BulletParticleSystem system);
}