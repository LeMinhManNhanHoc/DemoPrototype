using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BallDestoyState : IBallState
{
    Tween t;
    public override void OnStateEnter(Ball ball)
    {
        //ball.ballAnimation.clip = ball.ballAnimation.GetClip("BallDestroy");
        //ball.ballAnimation.Play();
        Sequence s = DOTween.Sequence();
        s.Append(ball.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f)).OnStepComplete(()=> GameController.Instance.audioSource.PlayOneShot(GameController.Instance.popClip));
        s.Append(ball.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f));

        Image ballSprite = ball.GetComponent<Image>();
        s.Join(DOTween.ToAlpha(() => ballSprite.color, x => ballSprite.color = x, 1, 0));

        s.Play().OnComplete(() => GameController.Instance.ballPool.DeactiveBall(ball));
    }

    public override void OnStateExit(Ball ball)
    {

    }

    public override void OnStateUpdate(Ball ball)
    {

    }
}