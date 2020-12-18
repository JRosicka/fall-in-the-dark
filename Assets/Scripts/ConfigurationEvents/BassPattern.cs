﻿using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class BassPattern : Pattern {
    private static readonly Vector2 Origin = new Vector2(0, 3);
    private static readonly float MaxDirectionalDistanceFromOrigin = .8f;
    private static readonly float MaxDirectionalMovementPerMove = .7f;
    private static readonly float MinDirectionalMovementPerMove = .2f;
    float xMin => Origin.x - MaxDirectionalDistanceFromOrigin;
    float xMax => Origin.x + MaxDirectionalDistanceFromOrigin;
    float yMin => Origin.y - MaxDirectionalDistanceFromOrigin;
    float yMax => Origin.y + MaxDirectionalDistanceFromOrigin;

    private enum MoveDirections {
        MoveLeft = 201,
        MoveRight = 202,
    }

    public SynthResponseShot SynthShotPrefab;
    
    [Header("Movement")] 
    public AnimationCurve MovementCurveRegular;
    public AnimationCurve MovementCurveFudge;
    // How long it takes to move from one waypoint to another
    public float TravelTime = 1f;

    private float currentTravelTime = -1;
    private Vector3 originWaypoint;
    private Vector3 targetWaypoint;
    private Vector3 fudgeTargetWaypoint;
    private bool movedBefore;
    private void Update() {
        if (currentTravelTime < 0)
            return;
        
        currentTravelTime += Time.deltaTime;
        if (currentTravelTime >= TravelTime) {
            transform.position = targetWaypoint;
            currentTravelTime = -1;
            return;
        }

        float normalTravelProgress = MovementCurveRegular.Evaluate(currentTravelTime / TravelTime);
        float fudgeTravelProgress = MovementCurveFudge.Evaluate(currentTravelTime / TravelTime);

        Vector3 normalProgress = Vector3.Lerp(originWaypoint, targetWaypoint, normalTravelProgress) - originWaypoint;
        Vector3 fudgeProgress = Vector3.Lerp(originWaypoint, fudgeTargetWaypoint, fudgeTravelProgress) - originWaypoint;
        Vector3 totalProgress = normalProgress + fudgeProgress;
        transform.position = originWaypoint + totalProgress;
    }

    // ReSharper disable once UnusedMember.Global
    public void FireAnimationFlare() {
        // TODO: Animate
    }

    // ReSharper disable once UnusedMember.Global
    public void FireSynthShotClockwise() {
        FireSynthShot(true);
    }
    
    // ReSharper disable once UnusedMember.Global
    public void FireSynthShotCounterClockwise() {
        FireSynthShot(false);
    }

    private void FireSynthShot(bool clockwise) {
        Transform t = transform;
        SynthResponseShot shot = Instantiate(SynthShotPrefab, t.position, t.rotation,
            PlayerController.Instance.ShotBucket);
        shot.Shoot(clockwise);
    }

    // ReSharper disable once UnusedMember.Global
    public void MoveLeft() {
        MoveToWaypoint(DetermineTargetTransform(MoveDirections.MoveLeft));
    }

    // ReSharper disable once UnusedMember.Global
    public void MoveRight() {
        MoveToWaypoint(DetermineTargetTransform(MoveDirections.MoveRight));
    }

    private void MoveToWaypoint(Vector3 waypoint) {
        Vector3 oldTarget = transform.position;
        if (movedBefore)
            oldTarget = targetWaypoint;
        originWaypoint = oldTarget;
        targetWaypoint = waypoint;
        Vector3 originalVector = targetWaypoint - originWaypoint;
        Vector3 rotatedVector = new Vector3(originalVector.y, -1 * (originalVector.x), 0);
        int direction = Random.Range(0, 2) == 0 ? 1 : -1;
        fudgeTargetWaypoint = rotatedVector * direction + originWaypoint;
        currentTravelTime = 0;

        movedBefore = true;
    }

    private Vector3 DetermineTargetTransform(MoveDirections moveEvent) {
        Vector3 ret = Vector3.zero;
        Vector3 currentPosition = transform.position;
        float xTravel;
        float yTravel;
        
        // We want to pick a random travel distance out of the legal range. That way, the agent will tend to pick 
        // movements that go towards the origin
        switch (moveEvent) {
            case MoveDirections.MoveLeft:
                xTravel =
                    Random.Range(Mathf.Max(-MinDirectionalMovementPerMove, xMin - currentPosition.x),
                        Mathf.Min(-MaxDirectionalMovementPerMove, -MinDirectionalMovementPerMove));
                yTravel =
                    Random.Range(Mathf.Max(-MaxDirectionalMovementPerMove, yMin - currentPosition.y),
                        Mathf.Min(MaxDirectionalMovementPerMove, yMax - currentPosition.y));
                ret = new Vector3(currentPosition.x + xTravel, currentPosition.y + yTravel);
                break;
            case MoveDirections.MoveRight:
                xTravel =
                    Random.Range(Mathf.Max(MinDirectionalMovementPerMove, MinDirectionalMovementPerMove), Mathf.Min(MaxDirectionalMovementPerMove, xMax - currentPosition.x));
                yTravel =
                    Random.Range(Mathf.Max(-MaxDirectionalMovementPerMove, yMin - currentPosition.y),
                        Mathf.Min(MaxDirectionalMovementPerMove, yMax - currentPosition.y));
                ret = new Vector3(currentPosition.x + xTravel, currentPosition.y + yTravel);
                break;
            default:
                Assert.IsTrue(false);
                break;
        }
        return ret;
    }
}