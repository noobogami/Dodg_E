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

        ResetDifficulty();
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

                    //DecreaseDifficulty();

                    ResetCombo();                    
                    //ChangeElectracityParticleBurst(5);
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
        SetDifficulty(GameManager.instance.score);
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
        if (combo+1 >= (currentCombo + 1) * 10 && currentCombo < comboParent.transform.childCount-1)
        {
            IncreaseCombo();
            SetComboImage();
            DeletePSqStack();
            CreatePSqStack();
        }
        ViewManager.instance.ChangeComboText(combo);
        StatHandler.instance.SetStat("MAX COMBO", combo, true);


        //ChangeElectracityParticleBurst(5, true);
    }

    private void IncreaseCombo()
    {
        combo++;
        ViewManager.instance.SetFireIntensity(combo, 40);    
    }

    private void ResetCombo()
    {
        combo = 0;
        ViewManager.instance.SetFireIntensity(combo, 40);
        ViewManager.instance.ChangeComboText(combo);
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

    private float sqSpeed_start = 5, sqSpeed_end = 10;
    private float sqGenRate_start = 1, sqGenRate_end = 0.5f;
    private float playerSpeed_start = 7, playerSpeed_end = 14;


    private int maxDiffPoint = 1000;
    private void SetDifficulty(int point)
    {
        float p = Mathf.Clamp(point * 1.0f / maxDiffPoint, 0, 1);
        
        sqSpeed = ((sqSpeed_end - sqSpeed_start) * p) + sqSpeed_start;
        sqGenRate = ((sqGenRate_end - sqGenRate_start) * p) + sqGenRate_start;
        playerSpeed = (((playerSpeed_end - playerSpeed_start) * p) + playerSpeed_start) * (playerSpeed < 0 ? -1 : 1);
    }
    

    /*internal void DecreaseDifficulty(bool reset = false)
    {
        ViewManager.instance.DecreaseFireIntensity(reset);
        
        sqGenRate += 0.015f;
        sqSpeed -= 0.13f;
        playerSpeed -= 0.2f * (playerSpeed < 0 ? -1 : 1);

        if (reset || sqGenRate >= 1) sqGenRate = 1;
        if (reset || sqSpeed <= 5) sqSpeed = 5;
        if (reset || (playerSpeed <= 7 && playerSpeed > 0)) playerSpeed = 7;
        if (reset || (playerSpeed >= -7 && playerSpeed < 0)) playerSpeed = -7;
    }*/

    private void ResetDifficulty()
    {
        //DecreaseDifficulty(true);
        SetDifficulty(0);
        currentCombo = 0;
        SetComboImage();
    }



    [Header("Player Combo")] public GameObject comboParent;
    private int currentCombo = 0;

    private void SetComboImage()
    {
        for (int i = 0; i < comboParent.transform.childCount; i++)
        {
            comboParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        comboParent.transform.GetChild(currentCombo).gameObject.SetActive(true);
    }
}