using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Ball currentBall;
    public Vector2Int index;
    
    public void OnClick()
    {
        GameController.Instance.OnSelectCell(this);
    }
}
