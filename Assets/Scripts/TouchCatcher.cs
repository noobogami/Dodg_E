using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchCatcher : MonoBehaviour, IPointerDownHandler
{
    public ParticleSystem tapWave;
    public void OnPointerDown(PointerEventData eventData)
    {
        GamePlayManager.instance.ChangePlayerDirection(true);
        tapWave.transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
        tapWave.transform.position = new Vector3(tapWave.transform.position.x, tapWave.transform.position.y, 0);
        tapWave.Play();
    }
}
