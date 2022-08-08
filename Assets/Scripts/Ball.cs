using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BallType 
{ 
    RED,
    GREEN,
    BLUE,
    YELLOW,
    PURPLE,
    BROWN,
    MAGIC,
    WRECK,
    MAX
}

public class Ball : MonoBehaviour
{
    public Vector2Int index;
    public BallType type;
    public Image myImage;
    public IBallState currentState;
    public Animation ballAnimation;
    public GameObject ghostIcon;

    public List<Sprite> spriteList;

    public BallSpawnState spawnState = new BallSpawnState();
    public BallAppearState appearState = new BallAppearState();
    public BallHighlightState hightlightState = new BallHighlightState();
    public BallDestoyState destroyState = new BallDestoyState();
    public bool isGhost = false;
    public float ghostPercent = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        SetUpBall();
        SetState(spawnState);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    SetState(spawnState);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    SetState(appearState);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    SetState(destroyState);
        //}

        currentState.OnStateUpdate(this);
    }

    public void SetState(IBallState state)
    {
        if (currentState != null)
        {
            currentState.OnStateExit(this);
        }

        currentState = state;
        currentState.OnStateEnter(this);
    }

    public void SetUpBall()
    {
        type = (BallType)Random.Range(0, (int)BallType.MAX);
        myImage.sprite = spriteList[(int)type];
        if (type == BallType.WRECK || type == BallType.MAGIC)
        {
            isGhost = false;
        }
        else
        {
            isGhost = (Random.Range(0.0f, 1.0f) < ghostPercent);
        }

        ghostIcon.SetActive(isGhost);
    }

    public void ReadyBall()
    {
        SetState(appearState);
    }
}
