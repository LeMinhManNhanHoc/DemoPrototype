using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBallState
{
    public abstract void OnStateEnter(Ball ball);
    public abstract void OnStateUpdate(Ball ball);
    public abstract void OnStateExit(Ball ball);
}
