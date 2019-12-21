using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    internal static TutorialManager instance;
    
    private int _tutState;

    public GameObject tutParentPanel, tutPanel01, tutPanel02, tutPanel03;

    internal void ResetPanels()
    {
        tutPanel01.SetActive(false);
        tutPanel02.SetActive(false);
        tutPanel03.SetActive(false);
    }
    public void StartTutorial(){
        if (_tutState != 0)
            return;
        tutParentPanel.SetActive(true);
        _tutState = 1;
        tutPanel01.SetActive(true);
    }

    public void ShowTutorial02()
    {
        if (_tutState != 1)
            return;
        _tutState = 2;
        ResetPanels();
        tutPanel02.SetActive(true);
    }
    public void ShowTutorial03()
    {
        if (_tutState != 2)
            return;
        _tutState = 3;
        ResetPanels();
        tutPanel03.SetActive(true);
    }

    public void EndTutorial()
    {
        if (_tutState != 3)
            return;
        _tutState = 4;
        ResetPanels();
        GameManager.instance.EndTutorial();
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _tutState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
