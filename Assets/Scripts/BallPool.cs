using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    public int poolSize;
    public GameObject ballPrefab;
    public List<Ball> ballList;
    
    [ContextMenu("Create pool")]
    public void CreatePool()
    {
        ballList = new List<Ball>();
        for (int i = 0; i < poolSize; i++)
        {
            Ball ball = Instantiate(ballPrefab, transform).GetComponent<Ball>();
            ball.gameObject.SetActive(false);
            ballList.Add(ball);
        }
    }

    private Ball GetAvailableBall()
    {
        for (int i = 0; i < ballList.Count; i++)
        {
            if(!ballList[i].gameObject.activeInHierarchy)
            {
                return ballList[i];
            }
        }

        return null;
    }

    public Ball ActiveBall(Cell cell)
    {
        Ball ball = GetAvailableBall();
        ball.index = cell.index;
        ball.transform.position = cell.transform.position;
        ball.gameObject.SetActive(true);

        return ball;
    }

    public void DeactiveBall(Ball ball)
    {
        ball.gameObject.SetActive(false);
        ball.transform.position = Vector3.zero;
    }
}
