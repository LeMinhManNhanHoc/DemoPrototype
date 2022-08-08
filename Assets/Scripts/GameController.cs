using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance
    {
        get
        {
            return instance;
        }
    }
    public int comboLenght = 3;
    public TextMeshProUGUI scoreUpdate;
    public TextMeshProUGUI highScore;
    public Cell[,] gridArray;
    public BallPool ballPool;
    public GridGenerator gridGenerator;
    public NextBallIndicator nextBallIndicator;
    public Ball currentSelectBall = null;
    public Cell currentSelectCell = null;
    public AudioSource audioSource;

    public AudioClip popClip;
    public AudioClip clickClip;
    public AudioClip wrongClip;

    private Queue<Ball> ballQueue = new Queue<Ball>();
    private List<Cell> finalComboList;
    private int currentScore;
    private int displayScore;

    public GameObject settingPanel;
    public GameObject gameOverPanel;
    public Slider volumeSlider;
    //public IGameState currentGameState;

    //GameSelectBallState selectState = new GameSelectBallState();

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        highScore.text = PlayerPrefs.GetInt("highScore", 0).ToString();
        audioSource.volume = PlayerPrefs.GetFloat("volume", 1f);
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);

        StartCoroutine(StartNewGame());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    RandomNextBall();
        //}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingPanel.SetActive(!settingPanel.activeInHierarchy);
        }
        //currentGameState.OnStateUpdate();
    }

    public void OnReturnToMainMenuClicked()
    {
        SceneManager.LoadScene(0);
    }

    public void OnResumeClick()
    {
        settingPanel.SetActive(false);
    }

    public void OnRetryClicked()
    {
        SceneManager.LoadScene(1);
    }

    IEnumerator StartNewGame()
    {
        gridGenerator.CreateGrid();
        yield return new WaitForEndOfFrame();
        RandomBallStart();
        RandomNextBall();
        //SetState(selectState);
    }

    public void RandomBallStart()
    {
        for (int i = 0; i < 10;)
        {
            int x = Random.Range(0, gridGenerator.gridSize.x);
            int y = Random.Range(0, gridGenerator.gridSize.y);

            if (gridArray[x, y].currentBall == null)
            {
                i++;
                gridArray[x, y].currentBall = ballPool.ActiveBall(gridArray[x, y]);
                gridArray[x, y].currentBall.ReadyBall();
            }
        }
    }

    public void RandomNextBall()
    {
        int availableSlot = 0;

        foreach (var slot in gridArray)
        {
            if (slot.currentBall == null)
            {
                availableSlot++;
            }
        }

        if (availableSlot <= 3)
        {
            for (int i = 0; i < availableSlot;)
            {
                int x = Random.Range(0, gridGenerator.gridSize.x);
                int y = Random.Range(0, gridGenerator.gridSize.y);

                if (gridArray[x, y].currentBall == null)
                {
                    gridArray[x, y].currentBall = ballPool.ActiveBall(gridArray[x, y]);
                    nextBallIndicator.nextBallArray[i].sprite = gridArray[x, y].currentBall.myImage.sprite;
                    ballQueue.Enqueue(gridArray[x, y].currentBall);
                    i++;
                }
            }

            int previousHighscore = PlayerPrefs.GetInt("highScore", 0);
            if (currentScore > previousHighscore)
            {
                PlayerPrefs.SetInt("highScore", currentScore);
            }

            gameOverPanel.SetActive(true);
        }
        else
        {
            for (int i = 0; i < 3;)
            {
                int x = Random.Range(0, gridGenerator.gridSize.x);
                int y = Random.Range(0, gridGenerator.gridSize.y);

                if (gridArray[x, y].currentBall == null)
                {
                    gridArray[x, y].currentBall = ballPool.ActiveBall(gridArray[x, y]);
                    nextBallIndicator.nextBallArray[i].sprite = gridArray[x, y].currentBall.myImage.sprite;
                    ballQueue.Enqueue(gridArray[x, y].currentBall);
                    i++;
                }
            }
        }
    }

    //public void SetState(IGameState state)
    //{
    //    currentGameState = state;
    //    currentGameState.OnStateEnter();
    //}

    public void OnSelectCell(Cell cell)
    {
        audioSource.PlayOneShot(clickClip);
        if (currentSelectBall == null)
        {
            //if (cell != null)
            {
                currentSelectCell = cell;
                currentSelectBall = cell.currentBall;

                if (currentSelectBall != null && (currentSelectBall.currentState is BallAppearState))
                {
                    currentSelectBall.SetState(currentSelectBall.hightlightState);
                }
                else
                {
                    currentSelectCell = null;
                    currentSelectBall = null;
                }
            }
        }
        else
        {
            //Debug.LogError("1");
            if (cell.currentBall == null)
            {
                //if (currentSelectBall.type == BallType.GHOST)
                if(currentSelectBall.isGhost)
                {
                    CheckValidMoveGhost(cell);
                }
                else
                {
                    if (currentSelectBall.type == BallType.MAGIC)
                    {
                        CheckValidMoveMagic(cell);
                    }
                    else if (currentSelectBall.type == BallType.WRECK)
                    {
                        CheckValidMoveWreck(cell);
                    }
                    else
                    {
                        CheckValidMove(cell);
                    }
                }

                currentSelectBall.SetState(currentSelectBall.appearState);
                currentSelectBall = null;
            }
            else
            {
                currentSelectBall.SetState(currentSelectBall.appearState);
                currentSelectBall = null;
                currentSelectCell = null;
            }

            foreach (var item in gridArray)
            {
                if (item.currentBall != null)
                {
                    comboListSouth.Clear();
                    comboListSouthEast.Clear();
                    comboListSouthWest.Clear();
                    comboListEast.Clear();

                    comboListSouth.Add(item);
                    comboListSouthEast.Add(item);
                    comboListSouthWest.Add(item);
                    comboListEast.Add(item);

                    CheckSouth(item);
                    CheckSouthEast(item);
                    CheckSouthWest(item);
                    CheckEast(item);

                    if (comboListSouth.Count < comboLenght)
                    {
                        comboListSouth.Clear();
                    }

                    if (comboListSouthEast.Count < comboLenght)
                    {
                        comboListSouthEast.Clear();
                    }

                    if (comboListSouthWest.Count < comboLenght)
                    {
                        comboListSouthWest.Clear();
                    }

                    if (comboListEast.Count < comboLenght)
                    {
                        comboListEast.Clear();
                    }

                    finalComboList = comboListSouth.Union(comboListSouthEast).Union(comboListSouthWest).Union(comboListEast).ToList();

                    foreach (var cellItem in finalComboList)
                    {
                        cellItem.currentBall.SetState(cellItem.currentBall.destroyState);
                        cellItem.currentBall = null;
                        currentScore += 100;
                    }
                }
            }

            Tween t = DOTween.To(() => displayScore, x => displayScore = x, currentScore, 0.5f).SetEase(Ease.Linear).OnUpdate(() => scoreUpdate.text = displayScore.ToString()).OnComplete(() => displayScore = currentScore);
            t.Play();
        }
    }

    List<Ball> blocker;
    public void CheckValidMove(Cell cell)
    {
        //if (CheckHorizontal(cell))
        blocker = new List<Ball>();

        if (!CheckHorizontal(cell) && !CheckVertical(cell))
        {
            foreach (var ball in blocker)
            {
                ball.transform.DOShakeRotation(0.5f, 45);
            }
            audioSource.PlayOneShot(wrongClip);
        }
        else
        {
            currentSelectBall.transform.position = cell.transform.position;
            currentSelectBall.index = cell.index;
            cell.currentBall = currentSelectBall;

            currentSelectCell.currentBall = null;
            currentSelectCell = null;

            for (int i = ballQueue.Count; i > 0; i--)
            {
                Ball nextBall = ballQueue.Dequeue();
                nextBall.SetState(nextBall.appearState);
            }

            RandomNextBall();
        }
        //Debug.LogError("H: " + CheckHorizontal(cell));
        //Debug.LogError("V: " + CheckVertical(cell));
        //Debug.LogError("---------------------------------");
    }

    private bool CheckHorizontal(Cell cell)
    {
        int moveX = Mathf.Abs(cell.index.x - currentSelectBall.index.x);
        //int moveY = Mathf.Abs(currentSelectBall.index.y - cell.index.y);

        //Check right
        if (currentSelectBall.index.x < cell.index.x)
        {
            for (int x = currentSelectBall.index.x + 1; x < cell.index.x; x++)
            //for (int x = cell.index.x; x > currentSelectBall.index.x; x--)
            {
                if (gridArray[x, currentSelectBall.index.y].currentBall != null &&
                   (gridArray[x, currentSelectBall.index.y].currentBall.currentState is BallAppearState))
                {
                    blocker.Add(gridArray[x, currentSelectBall.index.y].currentBall);
                    //gridArray[x, currentSelectBall.index.y].currentBall.transform.DOShakeRotation(0.5f, 45);
                    return false;
                }
            }

            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y < cell.index.y; y++)
                //for (int y = cell.index.y; y > currentSelectBall.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x + moveX, y].currentBall != null &&
                       (gridArray[currentSelectBall.index.x + moveX, y].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[currentSelectBall.index.x + moveX, y].currentBall);
                        //gridArray[currentSelectBall.index.x + moveX, y].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y > cell.index.y; y--)
                //for (int y = cell.index.y; y < currentSelectBall.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x + moveX, y].currentBall != null &&
                       (gridArray[currentSelectBall.index.x + moveX, y].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[currentSelectBall.index.x + moveX, y].currentBall);
                        //gridArray[currentSelectBall.index.x + moveX, y].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }
        //Check left
        else if (currentSelectBall.index.x > cell.index.x)
        {
            for (int x = currentSelectBall.index.x - 1; x > cell.index.x; x--)
            //for (int x = cell.index.x; x < moveX; x++)
            {
                if (gridArray[x, currentSelectBall.index.y].currentBall != null &&
                   (gridArray[x, currentSelectBall.index.y].currentBall.currentState is BallAppearState))
                {
                    blocker.Add(gridArray[x, currentSelectBall.index.y].currentBall);
                    //gridArray[x, currentSelectBall.index.y].currentBall.transform.DOShakeRotation(0.5f, 45);
                    return false;
                }
            }

            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y < cell.index.y; y++)
                //for (int y = cell.index.y; y > currentSelectBall.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x - moveX, y].currentBall != null &&
                       (gridArray[currentSelectBall.index.x - moveX, y].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[currentSelectBall.index.x - moveX, y].currentBall);
                        //gridArray[currentSelectBall.index.x - moveX, y].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y > cell.index.y; y--)
                //for (int y = cell.index.y; y < currentSelectBall.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x - moveX, y].currentBall != null &&
                       (gridArray[currentSelectBall.index.x - moveX, y].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[currentSelectBall.index.x - moveX, y].currentBall);
                        //gridArray[currentSelectBall.index.x - moveX, y].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }
        else
        {
            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y + 1; y < cell.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x, y].currentBall != null &&
                       (gridArray[currentSelectBall.index.x, y].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[currentSelectBall.index.x, y].currentBall);
                        //gridArray[currentSelectBall.index.x, y].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y - 1; y > cell.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x, y].currentBall != null &&
                       (gridArray[currentSelectBall.index.x, y].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[currentSelectBall.index.x, y].currentBall);
                        //gridArray[currentSelectBall.index.x, y].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }
    }

    private bool CheckVertical(Cell cell)
    {
        //int moveX = Mathf.Abs(cell.index.x - currentSelectBall.index.x);
        int moveY = Mathf.Abs(currentSelectBall.index.y - cell.index.y);

        //Check down
        if (currentSelectBall.index.y < cell.index.y)
        {
            for (int y = currentSelectBall.index.y + 1; y < cell.index.y; y++)
            //for (int y = cell.index.y; y > currentSelectBall.index.y; y--)
            {
                if (gridArray[currentSelectBall.index.x, y].currentBall != null &&
                   (gridArray[currentSelectBall.index.x, y].currentBall.currentState is BallAppearState))
                {
                    blocker.Add(gridArray[currentSelectBall.index.x, y].currentBall);
                    //gridArray[y, currentSelectBall.index.x].currentBall.transform.DOShakeRotation(0.5f, 45);
                    return false;
                }
            }

            //Check right
            if (currentSelectBall.index.x < cell.index.x)
            {
                for (int x = currentSelectBall.index.x; x < cell.index.x; x++)
                //for (int x = cell.index.x; x > currentSelectBall.index.x; x--)
                {
                    if (gridArray[x, currentSelectBall.index.y + moveY].currentBall != null &&
                       (gridArray[x, currentSelectBall.index.y + moveY].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[x, currentSelectBall.index.y + moveY].currentBall);
                        //gridArray[x, currentSelectBall.index.y + moveY].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            //Check left
            else if (currentSelectBall.index.x > cell.index.x)
            {
                for (int x = currentSelectBall.index.x; x > cell.index.x; x--)
                //for (int x = cell.index.x; x < moveX; x++)
                {
                    if (gridArray[x, currentSelectBall.index.y + moveY].currentBall != null &&
                       (gridArray[x, currentSelectBall.index.y + moveY].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[x, currentSelectBall.index.y + moveY].currentBall);
                        //gridArray[x, currentSelectBall.index.y + moveY].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }
        //Check up
        else if (currentSelectBall.index.y > cell.index.y)
        {
            for (int y = currentSelectBall.index.y - 1; y > cell.index.y; y--)
            //for (int y = cell.index.y; y < currentSelectBall.index.y; y++)
            {
                if (gridArray[currentSelectBall.index.x, y].currentBall != null &&
                   (gridArray[currentSelectBall.index.x, y].currentBall.currentState is BallAppearState))
                {
                    blocker.Add(gridArray[currentSelectBall.index.x, y].currentBall);
                    //gridArray[y, currentSelectBall.index.x].currentBall.transform.DOShakeRotation(0.5f, 45);
                    return false;
                }
            }

            //Check right
            if (currentSelectBall.index.x < cell.index.x)
            {
                for (int x = currentSelectBall.index.x; x < cell.index.x; x++)
                //for (int x = cell.index.x; x > currentSelectBall.index.x; x--)
                {
                    if (gridArray[x, currentSelectBall.index.y - moveY].currentBall != null &&
                       (gridArray[x, currentSelectBall.index.y - moveY].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[x, currentSelectBall.index.y - moveY].currentBall);
                        //gridArray[x, currentSelectBall.index.y - moveY].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            //Check left
            else if (currentSelectBall.index.x > cell.index.x)
            {
                for (int x = currentSelectBall.index.x; x > cell.index.x; x--)
                //for (int x = cell.index.x; x < moveX; x++)
                {
                    if (gridArray[x, currentSelectBall.index.y - moveY].currentBall != null &&
                       (gridArray[x, currentSelectBall.index.y - moveY].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[x, currentSelectBall.index.y - moveY].currentBall);
                        //gridArray[x, currentSelectBall.index.y - moveY].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }
        else
        {
            //Check right
            if (currentSelectBall.index.x < cell.index.x)
            {
                for (int x = currentSelectBall.index.x; x < cell.index.x; x++)
                //for (int x = cell.index.x; x > currentSelectBall.index.x; x--)
                {
                    if (gridArray[x, currentSelectBall.index.y + moveY].currentBall != null &&
                       (gridArray[x, currentSelectBall.index.y + moveY].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[x, currentSelectBall.index.y + moveY].currentBall);
                        //gridArray[x, currentSelectBall.index.y + moveY].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            //Check left
            else if (currentSelectBall.index.x > cell.index.x)
            {
                for (int x = currentSelectBall.index.x; x > cell.index.x; x--)
                //for (int x = cell.index.x; x < moveX; x++)
                {
                    if (gridArray[x, currentSelectBall.index.y + moveY].currentBall != null &&
                       (gridArray[x, currentSelectBall.index.y + moveY].currentBall.currentState is BallAppearState))
                    {
                        blocker.Add(gridArray[x, currentSelectBall.index.y + moveY].currentBall);
                        //gridArray[x, currentSelectBall.index.y + moveY].currentBall.transform.DOShakeRotation(0.5f, 45);
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return true;
            }
        }
    }

    public void CheckValidMoveGhost(Cell cell)
    {
        currentSelectBall.transform.position = cell.transform.position;
        currentSelectBall.index = cell.index;
        cell.currentBall = currentSelectBall;

        currentSelectCell.currentBall = null;
        currentSelectCell = null;

        for (int i = ballQueue.Count; i > 0; i--)
        {
            Ball nextBall = ballQueue.Dequeue();
            nextBall.SetState(nextBall.appearState);
        }

        RandomNextBall();
    }

    public void CheckValidMoveWreck(Cell cell)
    {
        CheckWreckMove(cell);

        currentSelectBall.transform.position = cell.transform.position;
        currentSelectBall.index = cell.index;
        cell.currentBall = currentSelectBall;

        currentSelectCell.currentBall = null;
        currentSelectCell = null;

        for (int i = ballQueue.Count; i > 0; i--)
        {
            Ball nextBall = ballQueue.Dequeue();
            nextBall.SetState(nextBall.appearState);
        }

        RandomNextBall();
    }

    public void CheckValidMoveMagic(Cell cell)
    {
        CheckMagicMove(cell);

        currentSelectBall.transform.position = cell.transform.position;
        currentSelectBall.index = cell.index;
        cell.currentBall = currentSelectBall;

        currentSelectCell.currentBall = null;
        currentSelectCell = null;

        for (int i = ballQueue.Count; i > 0; i--)
        {
            Ball nextBall = ballQueue.Dequeue();
            nextBall.SetState(nextBall.appearState);
        }

        RandomNextBall();
    }

    private bool CheckWreckMove(Cell cell)
    {
        int moveX = Mathf.Abs(cell.index.x - currentSelectBall.index.x);
        //int moveY = Mathf.Abs(currentSelectBall.index.y - cell.index.y);

        //Check right
        if (currentSelectBall.index.x < cell.index.x)
        {
            for (int x = currentSelectBall.index.x + 1; x < cell.index.x; x++)
            //for (int x = cell.index.x; x > currentSelectBall.index.x; x--)
            {
                if (gridArray[x, currentSelectBall.index.y].currentBall != null)
                {
                    gridArray[x, currentSelectBall.index.y].currentBall.SetState(gridArray[x, currentSelectBall.index.y].currentBall.destroyState);
                    gridArray[x, currentSelectBall.index.y].currentBall = null;
                }
            }

            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y < cell.index.y; y++)
                //for (int y = cell.index.y; y > currentSelectBall.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x + moveX, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x + moveX, y].currentBall.SetState(gridArray[currentSelectBall.index.x + moveX, y].currentBall.destroyState);
                        gridArray[currentSelectBall.index.x + moveX, y].currentBall = null;
                    }
                }
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y > cell.index.y; y--)
                //for (int y = cell.index.y; y < currentSelectBall.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x + moveX, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x + moveX, y].currentBall.SetState(gridArray[currentSelectBall.index.x + moveX, y].currentBall.destroyState);
                        gridArray[currentSelectBall.index.x + moveX, y].currentBall = null;
                    }
                }
            }

            return true;
        }
        //Check left
        else if (currentSelectBall.index.x > cell.index.x)
        {
            for (int x = currentSelectBall.index.x - 1; x > cell.index.x; x--)
            //for (int x = cell.index.x; x < moveX; x++)
            {
                if (gridArray[x, currentSelectBall.index.y].currentBall != null)
                {
                    gridArray[x, currentSelectBall.index.y].currentBall.SetState(gridArray[x, currentSelectBall.index.y].currentBall.destroyState);
                    gridArray[x, currentSelectBall.index.y].currentBall = null;
                }
            }

            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y < cell.index.y; y++)
                //for (int y = cell.index.y; y > currentSelectBall.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x - moveX, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x - moveX, y].currentBall.SetState(gridArray[currentSelectBall.index.x - moveX, y].currentBall.destroyState);
                        gridArray[currentSelectBall.index.x - moveX, y].currentBall = null;
                    }
                }
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y > cell.index.y; y--)
                //for (int y = cell.index.y; y < currentSelectBall.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x - moveX, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x - moveX, y].currentBall.SetState(gridArray[currentSelectBall.index.x - moveX, y].currentBall.destroyState);
                        gridArray[currentSelectBall.index.x - moveX, y].currentBall = null;
                    }
                }
            }

            return true;
        }
        else
        {
            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y + 1; y < cell.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x, y].currentBall.SetState(gridArray[currentSelectBall.index.x, y].currentBall.destroyState);
                        gridArray[currentSelectBall.index.x, y].currentBall = null;
                    }
                }
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y - 1; y > cell.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x, y].currentBall.SetState(gridArray[currentSelectBall.index.x, y].currentBall.destroyState);
                        gridArray[currentSelectBall.index.x, y].currentBall = null;
                    }
                }
            }

            return true;
        }
    }

    private bool CheckMagicMove(Cell cell)
    {
        int moveX = Mathf.Abs(cell.index.x - currentSelectBall.index.x);
        //int moveY = Mathf.Abs(currentSelectBall.index.y - cell.index.y);

        //Check right
        if (currentSelectBall.index.x < cell.index.x)
        {
            for (int x = currentSelectBall.index.x + 1; x < cell.index.x; x++)
            //for (int x = cell.index.x; x > currentSelectBall.index.x; x--)
            {
                if (gridArray[x, currentSelectBall.index.y].currentBall != null)
                {
                    gridArray[x, currentSelectBall.index.y].currentBall.SetUpBall();
                }
            }

            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y < cell.index.y; y++)
                //for (int y = cell.index.y; y > currentSelectBall.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x + moveX, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x + moveX, y].currentBall.SetUpBall();
                    }
                }
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y > cell.index.y; y--)
                //for (int y = cell.index.y; y < currentSelectBall.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x + moveX, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x + moveX, y].currentBall.SetUpBall();
                    }
                }
            }

            return true;
        }
        //Check left
        else if (currentSelectBall.index.x > cell.index.x)
        {
            for (int x = currentSelectBall.index.x - 1; x > cell.index.x; x--)
            //for (int x = cell.index.x; x < moveX; x++)
            {
                if (gridArray[x, currentSelectBall.index.y].currentBall != null)
                {
                    gridArray[x, currentSelectBall.index.y].currentBall.SetUpBall();
                }
            }

            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y < cell.index.y; y++)
                //for (int y = cell.index.y; y > currentSelectBall.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x - moveX, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x - moveX, y].currentBall.SetUpBall();
                    }
                }
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y; y > cell.index.y; y--)
                //for (int y = cell.index.y; y < currentSelectBall.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x - moveX, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x - moveX, y].currentBall.SetUpBall();
                    }
                }
            }

            return true;
        }
        else
        {
            //Check down
            if (currentSelectBall.index.y < cell.index.y)
            {
                for (int y = currentSelectBall.index.y + 1; y < cell.index.y; y++)
                {
                    if (gridArray[currentSelectBall.index.x, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x, y].currentBall.SetUpBall();
                    }
                }
            }
            //Check up
            else if (currentSelectBall.index.y > cell.index.y)
            {
                for (int y = currentSelectBall.index.y - 1; y > cell.index.y; y--)
                {
                    if (gridArray[currentSelectBall.index.x, y].currentBall != null)
                    {
                        gridArray[currentSelectBall.index.x, y].currentBall.SetUpBall();
                    }
                }
            }

            return true;
        }
    }

    List<Cell> comboListSouth = new List<Cell>();
    List<Cell> comboListSouthEast = new List<Cell>();
    List<Cell> comboListSouthWest = new List<Cell>();
    List<Cell> comboListEast = new List<Cell>();

    public void CheckSouth(Cell cell)
    {
        //Debug.LogError($"1{(cell.index.y + 1) < gridGenerator.gridSize.y}");
        //Debug.LogError($"2{gridArray[cell.index.x, cell.index.y + 1].currentBall != null}");
        //Debug.LogError($"3{gridArray[cell.index.x, cell.index.y + 1].currentBall.type == cell.currentBall.type}");
        //Debug.LogError($"4{gridArray[cell.index.x, cell.index.y + 1].currentBall.currentState is BallAppearState}");

        if ((cell.index.y + 1) < gridGenerator.gridSize.y &&
            gridArray[cell.index.x, cell.index.y + 1].currentBall != null &&
            gridArray[cell.index.x, cell.index.y + 1].currentBall.type == cell.currentBall.type &&
            gridArray[cell.index.x, cell.index.y + 1].currentBall.currentState is BallAppearState)
        {
            comboListSouth.Add(gridArray[cell.index.x, cell.index.y + 1]);
            CheckSouth(gridArray[cell.index.x, cell.index.y + 1]);
        }
    }

    public void CheckSouthEast(Cell cell)
    {
        //Debug.LogError($"1{(cell.index.y + 1) < gridGenerator.gridSize.y}");
        //Debug.LogError($"2{gridArray[cell.index.x, cell.index.y + 1].currentBall != null}");
        //Debug.LogError($"3{gridArray[cell.index.x, cell.index.y + 1].currentBall.type == cell.currentBall.type}");
        //Debug.LogError($"4{gridArray[cell.index.x, cell.index.y + 1].currentBall.currentState is BallAppearState}");

        if ((cell.index.y + 1) < gridGenerator.gridSize.y &&
            (cell.index.x + 1) < gridGenerator.gridSize.x &&
            gridArray[cell.index.x + 1, cell.index.y + 1].currentBall != null &&
            gridArray[cell.index.x + 1, cell.index.y + 1].currentBall.type == cell.currentBall.type &&
            gridArray[cell.index.x + 1, cell.index.y + 1].currentBall.currentState is BallAppearState)
        {
            comboListSouthEast.Add(gridArray[cell.index.x + 1, cell.index.y + 1]);
            CheckSouthEast(gridArray[cell.index.x + 1, cell.index.y + 1]);
        }
    }

    public void CheckSouthWest(Cell cell)
    {
        //Debug.LogError($"1{(cell.index.y + 1) < gridGenerator.gridSize.y}");
        //Debug.LogError($"2{gridArray[cell.index.x, cell.index.y + 1].currentBall != null}");
        //Debug.LogError($"3{gridArray[cell.index.x, cell.index.y + 1].currentBall.type == cell.currentBall.type}");
        //Debug.LogError($"4{gridArray[cell.index.x, cell.index.y + 1].currentBall.currentState is BallAppearState}");

        if ((cell.index.y + 1) < gridGenerator.gridSize.y &&
            (cell.index.x - 1) >= 0 &&
            gridArray[cell.index.x - 1, cell.index.y + 1].currentBall != null &&
            gridArray[cell.index.x - 1, cell.index.y + 1].currentBall.type == cell.currentBall.type &&
            gridArray[cell.index.x - 1, cell.index.y + 1].currentBall.currentState is BallAppearState)
        {
            comboListSouthWest.Add(gridArray[cell.index.x - 1, cell.index.y + 1]);
            CheckSouthWest(gridArray[cell.index.x - 1, cell.index.y + 1]);
        }
    }

    public void CheckEast(Cell cell)
    {
        //Debug.LogError($"1{(cell.index.y + 1) < gridGenerator.gridSize.y}");
        //Debug.LogError($"2{gridArray[cell.index.x, cell.index.y + 1].currentBall != null}");
        //Debug.LogError($"3{gridArray[cell.index.x, cell.index.y + 1].currentBall.type == cell.currentBall.type}");
        //Debug.LogError($"4{gridArray[cell.index.x, cell.index.y + 1].currentBall.currentState is BallAppearState}");

        if ((cell.index.x + 1) < gridGenerator.gridSize.x &&
            gridArray[cell.index.x + 1, cell.index.y].currentBall != null &&
            gridArray[cell.index.x + 1, cell.index.y].currentBall.type == cell.currentBall.type &&
            gridArray[cell.index.x + 1, cell.index.y].currentBall.currentState is BallAppearState)
        {
            comboListEast.Add(gridArray[cell.index.x + 1, cell.index.y]);
            CheckEast(gridArray[cell.index.x + 1, cell.index.y]);
        }
    }

    public void OnVolumeChange()
    {
        audioSource.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }
}
