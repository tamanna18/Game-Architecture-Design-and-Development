using UnityEngine;
using TMPro;
public class QuestionModel
{
    [Header("Specific Game Fields")]
    public string gameObjectTag;
    public GameIdentifier gameIdentifier;
    [Header("Task Fields")]
    public TaskType taskType;
    public int minValue;
    public int maxValue;
    public int questionAnswer;
    public string questionText;
    public int taskRepeats;
    [Header("Math Task")]
    public Operators randomOperator;
    public string mathOperatorString;
    public int argument1;
    public int argument2;
    [Header("Word Task")]
    public string[] words;
    public int randomWord;
    public string wordAnswer;
    [Header("Parity Task")]
    public Parity randomParity;
    public int clickedNeeded;
    [Header("Patterns Task")]
    public Sprite[] patternSprites; //Todo: add unique sprites
    public int randomPattern;
    public int patternSelected;
    public int patternsClicked = 0;

    public QuestionModel(GameIdentifier gameIdentifier, string gameObjectTag)
    {
        this.gameIdentifier = gameIdentifier;
        this.gameObjectTag = gameObjectTag;
        this.words = QuestionSystem.Instance.words;
        this.patternSprites = QuestionSystem.Instance.patternSprites;
        GenerateQuestion();
    }

    public void GenerateQuestion()
    {
        bool oldTaskWords = taskType == TaskType.Words;
        if (gameObjectTag == "Mole")
        {
            taskType = (TaskType)Random.Range(0, 3);
        }
        else
        {
            taskType = (TaskType)Random.Range(1, System.Enum.GetValues(typeof(TaskType)).Length);
        }
        if ((oldTaskWords && taskType != TaskType.Words) || (!oldTaskWords && taskType == TaskType.Words))
        {
            if (gameObjectTag == "Ball")
            {
                QuestionSystem.Instance.DestroyAllObjects(gameObjectTag);
            }
        }

        if (taskType == TaskType.MathEquation)
        {
            GenerateMathematicalQuestion(taskType);
        }
        else if (taskType == TaskType.Counting)
        {
            clickedNeeded = Random.Range(13, 18);
            questionText = "Click on " + clickedNeeded + " " + gameObjectTag.ToLower() + "s";
        }
        else if (taskType == TaskType.Words)
        {
            randomWord = Random.Range(0, words.Length);
            setMinAndMaxBallValues(65, 90);
            questionText = "Assemble the following word: " + words[randomWord];
        }
        else if (taskType == TaskType.Parity)
        {
            setMinAndMaxBallValues(1, 100);
            randomParity = (Parity)Random.Range(0, 2);
            taskRepeats = Random.Range(2, 5);
            questionText = "Click only <color=red>" + randomParity.ToString() + "</color> numbers, " + taskRepeats + " times!";
        }
        else if (taskType == TaskType.Patterns)
        {
            randomPattern = Random.Range(0, patternSprites.Length);
            taskRepeats = Random.Range(3, 6);
            questionText = "Click two " + gameObjectTag + "s with the same color in a row, " + taskRepeats + " times!";
        }
        SetQuestionText();
    }

    private void GenerateMathematicalQuestion(TaskType taskType)
    {
        randomOperator = (Operators)Random.Range(0, System.Enum.GetValues(typeof(Operators)).Length);
        switch (randomOperator)
        {
            case Operators.Addition:
                setArguments(1, 100);
                questionAnswer = argument1 + argument2;
                mathOperatorString = "+";
                break;
            case Operators.Subtraction:
                setArguments(1, 100);
                questionAnswer = argument1 - argument2;
                if (questionAnswer < 0) minValue = -100;
                mathOperatorString = "-";
                break;
            case Operators.Multiplication:
                do
                {
                    setArguments(1, 20);
                    questionAnswer = argument1 * argument2;
                } while (questionAnswer > 100);
                mathOperatorString = "X";
                break;
            case Operators.Division:
                do
                {
                    setArguments(1, 20);
                    questionAnswer = argument1 * argument2;
                } while (questionAnswer > 100);
                mathOperatorString = "/";
                break;
        }
        if (randomOperator == Operators.Division)
        {
            questionText = string.Format("{0} {1} {2} = ?", questionAnswer.ToString(), mathOperatorString, argument1.ToString());
        }
        else
        {
            questionText = string.Format("{0} {1} {2} = ?", argument1.ToString(), mathOperatorString, argument2.ToString());
        }

    }

    private void setArguments(int min, int max)
    {
        argument1 = Random.Range(min, max);
        argument2 = Random.Range(min, max);
        setMinAndMaxBallValues(min, max);
    }

    private void setMinAndMaxBallValues(int min, int max)
    {
        minValue = min;
        maxValue = max;
    }
    //Checking Answer on Click
    public bool CheckAnswer(int answer)
    {
        bool checkAnswer = false, questionSolved = false;

        if (taskType == TaskType.MathEquation)
        {
            if (randomOperator != Operators.Division)
            {
                checkAnswer = answer == questionAnswer;
                questionSolved = answer == questionAnswer;
            }
            else
            {
                checkAnswer = answer == argument2;
                questionSolved = answer == argument2;
            }
        }
        else if (taskType == TaskType.Words)
        {
            wordAnswer += System.Convert.ToChar(answer).ToString();
            if (wordAnswer == words[randomWord].Substring(0, wordAnswer.Length))
            {
                checkAnswer = true;
                if (wordAnswer.Length == words[randomWord].Trim().Length)
                {
                    questionSolved = true;
                    wordAnswer = "";
                }
            }
            else
            {
                wordAnswer = wordAnswer.Substring(0, wordAnswer.Length - 1);
            }
        }
        else if (taskType == TaskType.Parity)
        {
            if ((answer % 2 == 0 && randomParity == Parity.Even) || (answer % 2 != 0 && randomParity == Parity.Odd))
            {
                checkAnswer = true;
                if (--taskRepeats <= 0) questionSolved = true;
                questionText = "Click only <color=red>" + randomParity.ToString() + "</color> numbers, " + taskRepeats + " times!";
                SetQuestionText();

            }
        }
        else if (taskType == TaskType.Patterns)
        {
            checkAnswer = true;
            patternsClicked++;
            if (patternsClicked == 2 && patternSelected == answer)
            {
                patternsClicked = 0;
                if (--taskRepeats <= 0) questionSolved = true;
                questionText = "Click two " + gameObjectTag + "s with the same colors in a row, " + taskRepeats + " times!";
                SetQuestionText();
            }
            else if (patternsClicked == 2 && patternSelected != answer)
            {
                patternsClicked = 0;
                patternSelected = 0;
                checkAnswer = false;
            }
            else
            {
                patternSelected = answer;
            }
        }
        else if (taskType == TaskType.Counting)
        {
            questionText = gameObjectTag + "s to click: " + answer + "/" + clickedNeeded;
            SetQuestionText();
            if (answer == clickedNeeded)
            {
                checkAnswer = true;
                questionSolved = true;
                MoleSpawner.clicksCounter = 0;
            }
            else
            {
                checkAnswer = false;
            }
        }
        if (questionSolved)
        {
            if (GameManager.Instance.lockLeft.activeSelf)
            {
                GameManager.Instance.lockRight.SetActive(true);
                GameManager.Instance.lockRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Win";
                GameManager.Instance.lockRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
            }
            else
            {
                GameManager.Instance.lockLeft.SetActive(true);
                GameManager.Instance.lockLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Win";
                GameManager.Instance.lockLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
            }
            RewardSystem.Instance.GiveReward(gameIdentifier);
        }
        if (!checkAnswer && taskType != TaskType.Counting)
        {
            GameManager.Instance.minigameLost = true;
            if (CastleScript.Instance.health < CastleScript.Instance.maxHealth)
            {
                CastleScript.Instance.Heal(1000);
            }
            else
            {
                GameObject[] fortifications = GameObject.FindGameObjectsWithTag("Fortification");
                if (fortifications.Length > 0)
                {
                    foreach (GameObject fortification in fortifications)
                    {
                        FortificationScript fortScript = fortification.GetComponent<FortificationScript>();
                        fortScript.Heal(100);
                    }
                }
                else
                {
                    CastleScript.Instance.attackDamage += 2;
                }
            }
            wordAnswer = "";
            GameManager.Instance.miniGamesLost++;
        }
        return checkAnswer;
    }
    void SetQuestionText()
    {
        switch (gameIdentifier)
        {
            case GameIdentifier.MiniGameLeft:
                GameObject.Find("QuestionTextLeft").GetComponent<TextMeshProUGUI>().text = questionText;
                break;
            case GameIdentifier.MiniGameRight:
                GameObject.Find("QuestionTextRight").GetComponent<TextMeshProUGUI>().text = questionText;
                break;
        }
    }

    public int SetObjectValue(bool isAnswerBall)
    {
        if (!isAnswerBall)
        {
            return Random.Range(minValue, maxValue);
        }
        else if (taskType == TaskType.MathEquation && randomOperator == Operators.Division)
        {

            return argument2;
        }
        else if (taskType == TaskType.Words)
        {
            return (int)words[randomWord].Trim()[wordAnswer != null ? wordAnswer.Length : 0];
        }
        else if (taskType == TaskType.Parity)
        {
            if (randomParity == Parity.Even)
            {
                return (Random.Range(minValue, maxValue) / 2) * 2;
            }
            else
            {
                return (Random.Range(minValue, maxValue) / 2) * 2 + 1;
            }
        }
        else
        {
            return questionAnswer;
        }
    }
}