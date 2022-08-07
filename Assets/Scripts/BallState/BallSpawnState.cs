using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawnState : IBallState
{
    public override void OnStateEnter(Ball ball)
    {
        ball.ballAnimation.clip = ball.ballAnimation.GetClip("BallSpawn");
        ball.ballAnimation.Play();
    }

    public override void OnStateExit(Ball ball)
    {
        
    }

    public override void OnStateUpdate(Ball ball)
    {
        
    }
}
