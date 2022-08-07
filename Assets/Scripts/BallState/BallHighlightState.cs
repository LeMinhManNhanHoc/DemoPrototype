using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BallHighlightState : IBallState
{
    Tween t;

    public override void OnStateEnter(Ball ball)
    {
        
    }

    public override void OnStateExit(Ball ball)
    {
        t.Kill();
    }

    public override void OnStateUpdate(Ball ball)
    {
        t = ball.transform.DORotate(new Vector3(0f, 0f, 360f), 0.5f, RotateMode.FastBeyond360);
        t.Play();
    }
}
