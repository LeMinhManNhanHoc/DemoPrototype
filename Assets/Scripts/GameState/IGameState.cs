using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IGameState
{
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();

    public abstract void OnClickCell(Cell cell);
}
