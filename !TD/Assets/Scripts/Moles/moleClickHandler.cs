using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;


public class moleClickHandler : MonoBehaviour, IPointerClickHandler
{


    public Sprite[] hideFrames;
    public static bool rightTimerSet = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            GameManager.Instance.StartMinigame(GameIdentifier.MiniGameRight);
            if (gameObject.GetComponent<SpriteRenderer>().sprite.name == "frame2")

                if (!rightTimerSet)
                {
                    switch (MoleSpawner.Instance.questionModel.taskType)
                    {
                        case TaskType.Counting:
                            GameManager.Instance.minigameTimer = 25f;
                            rightTimerSet = true;
                            break;
                        case TaskType.MathEquation:
                            GameManager.Instance.minigameTimer = 10f;
                            rightTimerSet = true;
                            break;
                        case TaskType.Words:
                            GameManager.Instance.minigameTimer = 30f;
                            rightTimerSet = true;
                            break;
                    }
                }
            if (gameObject.GetComponent<SpriteRenderer>().sprite.name == "frame2")
            {
                int tempAnswer = 0;
                if (MoleSpawner.Instance.questionModel.taskType == TaskType.Words)
                {
                    tempAnswer = char.Parse(gameObject.GetComponentInChildren<TextMesh>().text);
                }
                else if (MoleSpawner.Instance.questionModel.taskType == TaskType.MathEquation)
                {
                    tempAnswer = int.Parse(gameObject.GetComponentInChildren<TextMesh>().text);
                }
                if (!GameManager.Instance.minigameSelected)
                {
                    GameManager.Instance.minigameSelected = true;
                    GameManager.Instance.startMinigameTimer(GameIdentifier.MiniGameRight);
                }
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(hideFrames[0].name);
                MoleSpawner.Instance.CheckAnswer(tempAnswer);
                gameObject.GetComponentInChildren<TextMesh>().GetComponent<MeshRenderer>().enabled = false;
            }
        }
        catch
        {
            Debug.Log("Error");
        }
    }

}

