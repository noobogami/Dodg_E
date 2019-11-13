using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class SqHandler : MonoBehaviour
{
    public GameObject imageNormal;
    public GameObject imagePoint;
    
    
    private Transform _sqTrans;
    private Animator _sqAnimator;
    private BoxCollider2D _sqCollider;
    private SpriteRenderer _sqSprite;

    private float scrH;
    
    internal Vector3 speed;
    internal float angularSpeed;
    internal bool isPoint = false;
    


    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            if (isPoint == false)
            {
                GameManager.instance.Collided();
            }
            else
            {
                GameManager.instance.IncreasePoint();
                GamePlayManager.instance.PointCollected(this);
            }
        }
        
    }


    void Awake()
    {
        _sqTrans = GetComponent<Transform>();
        _sqAnimator = GetComponent<Animator>();
        _sqSprite = GetComponent<SpriteRenderer>();
        _sqCollider = GetComponent<BoxCollider2D>();
    }

    internal void Init()
    {
        // print(name + " Initialized");
        _sqCollider.enabled = true;
        
        imageNormal.SetActive(true);
        imagePoint.SetActive(false);
        transform.localScale = Vector3.one;
        isPoint = false;
    }
    
    public void ChangeToPoint()
    {
        //print(gameObject.name);
        isPoint = true;
        
        imageNormal.SetActive(false);
        imagePoint.SetActive(true);
    }
    public void Pop()
    {
        _sqAnimator.SetTrigger("Pop");
        _sqCollider.enabled = false;
    }

    public void DestroySq()
    {
        Destroy(gameObject);
    }

    public void HideSq()
    {
        gameObject.SetActive(false);
        _sqCollider.enabled = true;
    }
}
