using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public LevelBuilder m_LevelBuilder;
    public GameObject m_NextButton;

    public Text m_CurrentLevelText;
    public int CurrentLevel;
    private bool m_ReadyForInput;
    private Player m_Player;

    
    public void updateLabelLevel()
    {
        CurrentLevel = m_LevelBuilder.m_CurrentLevel + 1;
        m_CurrentLevelText.text = "Level "+CurrentLevel.ToString();
        Debug.Log(m_CurrentLevelText.text);
        //Text_CurrentLevel.text = m_CurrentLevel; //CALCULAR EL NIVELL ACTUAL
        //m_CurrentLevel = GetComponent<LevelBuilder>().m_CurrentLevel;
    }

    void Start()
    {
        m_NextButton.SetActive(false);
        ResetScene();
        updateLabelLevel();
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.Normalize();
        if (moveInput.sqrMagnitude > 0.5)//Button pressed or held
        {
            if (m_ReadyForInput)
            {
                m_ReadyForInput = false;
                m_Player.Move(moveInput);
                //m_NextButton.SetActive(IsLevelComplete());
                if (IsLevelComplete()==true)
                {
                    NextLevel();
                }
                
            }
        }
        else
        {
            m_ReadyForInput = true;
        }
    }

    public void NextLevel()
    {
        m_NextButton.SetActive(false);
        m_LevelBuilder.NextLevel();
        StartCoroutine(ResetSceneASync());
        updateLabelLevel();
    }

    public void ResetScene()
    {
        StartCoroutine(ResetSceneASync());
    }

    bool IsLevelComplete()
    {
        Box[] boxes = FindObjectsOfType<Box>();
        foreach (var box in boxes)
        {
            if (!box.m_OnCross) return false;
        }
        return true;
    }

    IEnumerator ResetSceneASync()
    {
        if (SceneManager.sceneCount > 1)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("LevelScene");
            while (!asyncUnload.isDone)
            {
                yield return null;
                Debug.Log("UnLoading...");
            }
            Debug.Log("Unload Done");
            Resources.UnloadUnusedAssets();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelScene", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log("Loading...");
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("LevelScene"));
        m_LevelBuilder.Build();
        m_Player = FindObjectOfType<Player>();
        Debug.Log("Level loaded");
    }

    
}


