using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class MoleSpawner : MonoBehaviour
{
    public static int clicksCounter = 0;
    public GameObject molePrefab;
    GameObject[] allMoles;
    List<GameObject> shownMoles;
    public Animator animator;
    public QuestionModel questionModel;
    private bool gameRunning;
    public int answerValue;
    static MoleSpawner _instance;
    public static MoleSpawner Instance

    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MoleSpawner>();
            }
            return _instance;
        }
    }

    private int hideMoles(int counter)
    {

        shownMoles = new List<GameObject>();

        foreach (GameObject mole in allMoles)
        {
            int isVisible = UnityEngine.Random.Range(0, 2);
            if (isVisible == 0)
            {
                counter--;
                mole.SetActive(false);
            }
            else
            {
                mole.SetActive(true);
                shownMoles.Add(mole);
            }
        }
        if (questionModel.taskType == TaskType.Counting)
        {
            foreach (GameObject mole in allMoles)
            {
                mole.GetComponentInChildren<TextMesh>().GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            generateAnswers();
        }
        return counter;
    }


    void generateAnswers()
    {
        int correctAnswerPosition = UnityEngine.Random.Range(0, shownMoles.Count - 1);
        for (int i = 0; i < shownMoles.Count; i++)
        {
            if (i == correctAnswerPosition)
            {
                setMoleAnswer(shownMoles[i], true);
            }
            else
            {
                setMoleAnswer(shownMoles[i], false);
            }

        }
    }

    void setMoleAnswer(GameObject gameMole, bool isTrue)
    {
        int ballNumber = questionModel.SetObjectValue(isTrue);
        if (isTrue)
        {
            answerValue = ballNumber;
        }

        if (questionModel.taskType == TaskType.MathEquation)
        {
            gameMole.GetComponentInChildren<TextMesh>().text = ballNumber.ToString();
        }
        else if (questionModel.taskType == TaskType.Words)
        {
            gameMole.GetComponentInChildren<TextMesh>().text = System.Convert.ToChar(ballNumber).ToString();
        }
        gameMole.GetComponentInChildren<TextMesh>().GetComponent<MeshRenderer>().enabled = false;

    }

    void resetMoles()
    {
        foreach (GameObject mole in allMoles)
        {
            mole.SetActive(true);
            mole.GetComponent<BoxCollider2D>().enabled = true;
            mole.GetComponent<Animator>().runtimeAnimatorController = animator.runtimeAnimatorController;
            mole.GetComponent<Animator>().Rebind();
            mole.GetComponentInChildren<TextMesh>().GetComponent<MeshRenderer>().enabled = true;
        }

    }

    public void resetMoleText()
    {
        foreach (GameObject tempMole in allMoles)
        {
            tempMole.GetComponent<BoxCollider2D>().enabled = true;
            tempMole.GetComponent<Animator>().runtimeAnimatorController = animator.runtimeAnimatorController;
        }
    }

    IEnumerator showAnswerValue(List<GameObject> moles)
    {
        yield return new WaitForSeconds(0.9f);
        if (questionModel.taskType != TaskType.Counting)
        {
            {
                foreach (GameObject mole in moles)
                {
                    if (mole.GetComponent<SpriteRenderer>().sprite.name == "frame2")
                    {
                        mole.GetComponentInChildren<TextMesh>().GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        // Debug.Log("too soon");
                    }
                }
            }

        }

    }
    void createNewView()
    {

        int shownMolesAmount = hideMoles(allMoles.Length);
        StartCoroutine(showAnswerValue(shownMoles));
        while (shownMolesAmount < 7)
        {
            shownMolesAmount = hideMoles(allMoles.Length);
            StartCoroutine(showAnswerValue(shownMoles));
            if (shownMolesAmount >= 7)
            {
                break;
            }
        }


    }

    IEnumerator startNewCoroutine()
    {
        allMoles = GameObject.FindGameObjectsWithTag("Mole");

        while (gameRunning)
        {
            resetMoles();
            createNewView();
            yield return new WaitForSeconds(4);
        }
    }

    public void startMoleSpawning(GameIdentifier gameIdentifier)
    {
        questionModel = QuestionSystem.Instance.GetQuestionModelForGame(gameIdentifier);
        gameRunning = true;
        StartCoroutine(startNewCoroutine());

    }

    public bool CheckAnswer(int answer)
    {
        if (questionModel.taskType == TaskType.Counting)
        {
            clicksCounter++;
            answer = clicksCounter;
        }

        return questionModel.CheckAnswer(answer);
    }


}
