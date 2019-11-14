using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GamePlayManager : MonoBehaviour
{
    internal static GamePlayManager instance;
    
    
    
    
    public GameObject player;
    public ParticleSystem sparkParticle;
    public ParticleSystem shortCircuit;
    public GameObject square;
    public GameObject pSq;
    public GameObject sqParent;
    public GameObject pSqParent;

    [Space(10)]
    public float sqGenRate;
    public float sqSpeed;
    public float playerSpeed;
    public int pointGenRate;

    [Space(10)] 
    public int stackSize;

    
    
    

    private float timer;
    private float withoutTouchTimer;
    private int sqCounter;
    internal bool paused;

    internal int combo;

    private List<SqHandler> sqStack = new List<SqHandler>();
    private List<SqHandler> activeSquares = new List<SqHandler>();

    private List<GameObject> pSqStack = new List<GameObject>();
    private List<GameObject> ActivePSqStack = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        Init();
    }

    private void Init()
    {
    }

    internal void StartPlaying()
    {
        player.SetActive(true);

        SetDifficulty();
        combo = 0;
        //ChangeElectracityParticleBurst(1);
        timer = 0;
        sqCounter = 0;
        paused = false;
        CreateSquareStack();
        DeletePSqStack();
        CreatePSqStack();
        ViewManager.instance.ChangeComboText(combo);
        StatHandler.instance.ResetStats();
    }

    void Update()
    {
        if (GameManager.instance.isPlaying)
        {
            StatHandler.instance.SetStat("TIME", Time.deltaTime, false, true);
            withoutTouchTimer += Time.deltaTime;
            
            MoveObjects();
            if (timer > sqGenRate)
            {
                timer -= sqGenRate;
                sqCounter++;
                ThrowSq(sqCounter % pointGenRate == 0);
            }
        }
        else if (!paused)
        {   
            int no = activeSquares.Count;
            for (int i = 0; i < no; i++)
            {
                var sq = activeSquares[0];
                //print(sq.gameObject);
                sq.Pop();
                activeSquares.Remove(sq);
            }

            //player.GetComponent<Animator>().SetTrigger("Pop");
            shortCircuit.Play();
            paused = true;

        }

        if (!paused)
        {
            timer += Time.deltaTime;
        }
    }

    private void MoveObjects()
    {
        for (int i = 0; i < activeSquares.Count; i++)
        {
            var sq = activeSquares[i];
            sq.transform.position += sq.speed * Time.deltaTime;
            sq.transform.Rotate(0, 0, sq.angularSpeed * Time.deltaTime);

            if (sq.transform.position.y < -3)
            {
                if (sq.isPoint)
                {
                    StatHandler.instance.SetStat("MISSED POINTS", 1, false, true);
                    DeletePSqStack();
                    CreatePSqStack();

                    DecreaseDifficulty();

                    combo = 0;
                    //ChangeElectracityParticleBurst(5);
                    ViewManager.instance.ChangeComboText(combo);
                }
                PopSq(sq);
                i--;
            }
        }

        // Change Player Direction
        if (!IsPlayerInBound())
        //if (!IsPlayerInBound() || Input.GetMouseButtonDown(0))
        {
            ChangePlayerDirection();
            SetPlayerInBound();
        }

        player.transform.position += new Vector3(playerSpeed * Time.deltaTime, 0, 0);
    }

    internal void ChangePlayerDirection(bool isTouched = false)
    {
        playerSpeed *= -1;

        if (isTouched)
        {
            StatHandler.instance.SetStat("WITHOUT TOUCH", withoutTouchTimer, true);
            withoutTouchTimer = 0;
        }
    }
    private void SetPlayerInBound()
    {
        if (player.transform.position.x > 4)
            player.transform.position = new Vector3(4, player.transform.position.y, 0);
        if (player.transform.position.x < -4)
            player.transform.position = new Vector3(-4, player.transform.position.y, 0);
    }

    private bool IsPlayerInBound()
    {
        Vector3 position = player.transform.position;
        return position.x < 4 && position.x > -4;
    }
    
    int w = Screen.width;
    int h = Screen.height;
    private void ThrowSq(bool isPoint = false)
    {
        SqHandler sqHandlerTemp = sqStack[0];
        sqStack.RemoveAt(0);
        activeSquares.Add(sqHandlerTemp);

        GameObject sq = sqHandlerTemp.gameObject;

        sqHandlerTemp.gameObject.SetActive(true);
        sqHandlerTemp.Init();
        if (isPoint)
        {
            sqHandlerTemp.ChangeToPoint();
            //print("Here's a Point");
        }

        //print(sqHandlerTemp.isPoint);

        float xStart = Utility.Rand(-w * 0.4f, w * 0.4f) / 100;
        float xEnd = Utility.Rand(-w * 0.4f, w * 0.4f) / 100;
        Vector3 startPoint = new Vector3(xStart, h / 200.0f + 1, 0);
        Vector3 endPoint = new Vector3(xEnd, 0, 0);
        Vector3 dir = (endPoint - startPoint).normalized;

        sq.transform.position = startPoint;
        sqHandlerTemp.speed = dir * sqSpeed;
        sqHandlerTemp.angularSpeed = Utility.RandAngle();
        
        //sq.transform.SetAsFirstSibling();
    }

    private void CreatePlayerSq()
    {
        GameObject tempSq = pSqStack[0];
        pSqStack.RemoveAt(0);
        ActivePSqStack.Add(tempSq);

        tempSq.gameObject.SetActive(true);

        tempSq.transform.Rotate(0, 0, Utility.RandAngle(0, 180));
        
        tempSq.transform.SetAsFirstSibling();
    }

    private void CreateSquareStack()
    {
        //print("child Count: " + sqParent.transform.childCount);
        for (int i = 0; i <  sqParent.transform.childCount; i++)
        {
            Destroy(sqParent.transform.GetChild(i).gameObject);
        }
        //print("child Count: " + sqParent.transform.childCount);
        sqStack = new List<SqHandler>();
        activeSquares = new List<SqHandler>();
        
        
        for (int i = 0; i < stackSize; i++)
        {
            GameObject sq = Instantiate(square, sqParent.transform);
            sq.name = "Square" + i;
            SqHandler sqHandlerTemp = sq.GetComponent<SqHandler>();
            sqStack.Add(sqHandlerTemp);
            sq.SetActive(false);
        }
    }

    private void DeletePSqStack()
    {
        //print("child Count: " + sqParent.transform.childCount);
        for (int i = 0; i <  pSqParent.transform.childCount; i++)
        {
            Destroy(pSqParent.transform.GetChild(i).gameObject);
        }
        //print("child Count: " + sqParent.transform.childCount);
        pSqStack = new List<GameObject>();
        ActivePSqStack= new List<GameObject>();
    }
    
    private void CreatePSqStack()
    {   
        for (int i = 0; i < stackSize; i++)
        {
            GameObject sq = Instantiate(pSq, pSqParent.transform);
            sq.name = "Square" + i;
            pSqStack.Add(sq);
            sq.SetActive(false);
        }
    }
    
    private void PopSq(SqHandler sq)
    {
        activeSquares.Remove(sq);
        sq.Pop();
        sqStack.Add(sq);
    }

    internal void PointCollected(SqHandler sq)
    {
        IncreaseDifficulty();
        sparkParticle.transform.position = (sq.transform.position + player.transform.position) / 2;
        sparkParticle.Play(true);
        
        
        StatHandler.instance.SetStat("COLLECTED POINTS", 1, false, true);
        PopSq(sq);
        if (pSqStack.Count == 0)
        {
            CreatePSqStack();
        }
        CreatePlayerSq();
        combo++;
        ViewManager.instance.ChangeComboText(combo);
        StatHandler.instance.SetStat("MAX COMBO", combo, true);


        //ChangeElectracityParticleBurst(5, true);
    }

    /*private void ChangeElectracityParticleBurst(short count, bool isCumulative = false)
    {
        var tempBurst = shortCircuit.emission.GetBurst(0);

        if (isCumulative)
        {
            tempBurst.maxCount += count;
            tempBurst.minCount += count;
        }
        else
            tempBurst.count = count;

        shortCircuit.emission.SetBurst(0, tempBurst);
    }*/
    

    private void SetDifficulty(float sqGenRateD = 1, float sqSpeedD = 5, float playerSpeedD = 7)
    {
        sqGenRate = sqGenRateD;
        sqSpeed = sqSpeedD;
        playerSpeed = playerSpeedD;
    }

    private void IncreaseDifficulty()
    {
        sqGenRate *= 0.98f;
        sqSpeed *= 1.02f;
        playerSpeed *= 1.02f;

        if (sqGenRate <= 0.5) sqGenRate = 0.5f;
        if (sqSpeed >= 10) sqSpeed = 10;
        if (playerSpeed >= 14 && playerSpeed > 0) playerSpeed = 14;
        if (playerSpeed <= -14 && playerSpeed < 0) playerSpeed = -14;
    }
    internal void DecreaseDifficulty()
    {
        sqGenRate *= 1.02f;
        sqSpeed *= 0.98f;
        playerSpeed *= 0.98f;

        if (sqGenRate >= 1) sqGenRate = 1;
        if (sqSpeed <= 5) sqSpeed = 5;
        if (playerSpeed <= 7 && playerSpeed > 0) playerSpeed = 7;
        if (playerSpeed >= -7 && playerSpeed < 0) playerSpeed = -7;
    }
}