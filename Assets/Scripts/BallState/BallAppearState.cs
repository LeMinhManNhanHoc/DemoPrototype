using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAppearState : IBallState
{
    public override void OnStateEnter(Ball ball)
    {
        ball.ballAnimation.clip = ball.ballAnimation.GetClip("BallAppear");
        ball.ballAnimation.Play();
    }

    public override void OnStateExit(Ball ball)
    {

    }

    public override void OnStateUpdate(Ball ball)
    {

    }
}