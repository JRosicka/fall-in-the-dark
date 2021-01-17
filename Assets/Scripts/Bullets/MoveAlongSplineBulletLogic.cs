﻿using System;
using SplineMesh;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Moves a Bullet along a spline over time
/// </summary>
public class MoveAlongSplineBulletLogic : BulletLogic {
    private Spline originalSpline;
    private Spline spline;
    private float progress;
    private bool isInPlayMode;
    private bool done;
    private float bulletSpeed;

    public Bullet FollowerPrefab;
    private Bullet follower;
    private float durationInSeconds;
    private bool restartBulletWhenDone;
    
    public MoveAlongSplineBulletLogic(Spline originalSpline, float durationInSeconds, bool restartBulletWhenDone) {
        // if (Application.IsPlaying(this))
        //     isInPlayMode = true;

        this.originalSpline = originalSpline;
        this.durationInSeconds = durationInSeconds;
        this.restartBulletWhenDone = restartBulletWhenDone;
    }
    
    public override void OnBulletSpawned(Bullet bullet) {
        follower = bullet;
        if (follower == null)
            throw new Exception("Follow is null, silly. This needs a bullet to move along the spline.");
        
        follower.speed = 0;    // TODO: We will definitely want some error handling here. We should not have any other bullet logic assigned to the bullet that adjusts its speed. 
        progress = 0;
        
        // Make a copy of the original spline and assign it as a child to the bullet GameObject so that its transform is 
        // changed accordingly
        var bulletTransform = bullet.transform;
        spline = Object.Instantiate(originalSpline, bulletTransform.position, bulletTransform.rotation, GameController.Instance.ShotBucket);
        bullet.transform.SetParent(spline.transform);
    }
    
    public override void BulletLogicUpdate(Bullet bullet, float deltaTime) {
        if (done)
            return;
        
        if (durationInSeconds <= 0)
            return;
        
        progress += deltaTime / durationInSeconds;
        if (progress > 1) {
            progress--;

            if (!restartBulletWhenDone && follower != null) {
                ReleaseBullet();
            }
        }

        MoveFollower(deltaTime);
    }
    
    /// <summary>
    /// Detach <see cref="follower"/> from the spline and send it free-floating at its current velocity
    /// </summary>
    private void ReleaseBullet() {
        follower.transform.SetParent(GameController.Instance.ShotBucket, true);
        follower.speed = bulletSpeed;
        follower = null;
        done = true;
    }

    private void MoveFollower(float deltaTime) {
        if (spline == null || follower == null)
            return;

        CurveSample lengthSample = spline.GetSampleAtDistance(progress * spline.Length);
        // CurveSample lengthSample = spline.GetSample(progress * (spline.nodes.Count - 1));    // For getting the sample weighted by nodes

        Vector3 initialPosition = follower.transform.localPosition;
        follower.transform.localPosition = lengthSample.location;
        follower.transform.localPosition = follower.transform.localPosition;
        follower.transform.localRotation = lengthSample.Rotation;

        float deltaPosition = (follower.transform.localPosition - initialPosition).magnitude;
        bulletSpeed = deltaPosition / deltaTime;
    }
}