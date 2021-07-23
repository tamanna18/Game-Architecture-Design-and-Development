using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public enum TaskType { Counting, MathEquation, Words, Parity, Patterns };
public enum Operators { Addition, Subtraction, Multiplication, Division };
public enum Parity { Even, Odd }
public class QuestionSystem : MonoBehaviour
{
    static QuestionSystem instance;
    public static QuestionSystem Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<QuestionSystem>();
            }
            return instance;
        }
    }
    private QuestionModel questionModelLeft;
    private QuestionModel questionModelRight;
    public string[] words;
    public Sprite[] patternSprites; //Todo: add unique sprites
    public TextAsset wordsTextAsset;

    void Awake()
    {
        words = wordsTextAsset.text.ToUpper().Split("\n"[0]);
    }

    public void InitializeQuestions(MiniGame miniGameLeft, MiniGame miniGameRight)
    {
        questionModelLeft = new QuestionModel(GameIdentifier.MiniGameLeft, GameManager.Instance.GetObjectTagForMinigame(miniGameLeft));
        questionModelRight = new QuestionModel(GameIdentifier.MiniGameRight, GameManager.Instance.GetObjectTagForMinigame(miniGameRight));
    }

    public QuestionModel GetQuestionModelForGame(GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft)
        {
            return questionModelLeft;
        }
        else
        {
            return questionModelRight;
        }
    }

    public void DestroyAllObjects(string gameObjectTag)
    {
        GameObject[] oldGameObjects = GameObject.FindGameObjectsWithTag(gameObjectTag);
        foreach (GameObject oldGameObject in oldGameObjects) Destroy(oldGameObject);
    }

    public void resetQuestions()
    {
        GameManager.Instance.lockLeft.transform.GetChild(0).gameObject.SetActive(true);
        GameManager.Instance.lockLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Locked";
        GameManager.Instance.lockLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
        GameManager.Instance.lockRight.transform.GetChild(0).gameObject.SetActive(true);
        GameManager.Instance.lockRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Locked";
        GameManager.Instance.lockRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
        questionModelLeft.GenerateQuestion();
        questionModelRight.GenerateQuestion();
        MoleSpawner.Instance.resetMoleText();
        RewardSystem.Instance.InitializeRewards();
        GameManager.Instance.lockLeft.SetActive(false);
        GameManager.Instance.lockRight.SetActive(false);
    }

}
