using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class BallsSpawner : MonoBehaviour
{
    static BallsSpawner instance;
    public static BallsSpawner Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<BallsSpawner>();
            }
            return instance;
        }
    }
    public Camera gameCamera;
    public QuestionModel questionModel;
    private float objectWidth;
    private float objectHeight;
    [Header("Spawning Settings")]
    public float spawnWait;
    public float spawnMostWait;
    public float spawnLeastWait;
    public int startWait;

    public bool stop = true;

    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Sprite ballSprite;

    public void StartBallsSpawner(GameIdentifier gameIdentifier)
    {
        questionModel = QuestionSystem.Instance.GetQuestionModelForGame(gameIdentifier);
        StartCoroutine(BallSpawner());
        StartCoroutine(AnswerBallSpawner());
    }

    void Update()
    {
        spawnWait = Random.Range(spawnLeastWait, spawnMostWait);
    }
    //Ball Spawning Methods
    IEnumerator BallSpawner()
    {
        yield return new WaitForSeconds(startWait);

        while (!stop)
        {
            SpawnBall(false);
            yield return new WaitForSeconds(spawnWait);
        }
    }

    IEnumerator AnswerBallSpawner()
    {
        yield return new WaitForSeconds(startWait);

        while (!stop)
        {
            if ((questionModel.taskType == TaskType.Patterns && questionModel.patternsClicked > 0) || questionModel.taskType != TaskType.Patterns) SpawnBall(true);
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void SpawnBall(bool isAnswerBall)
    {
        Sprite tempBallSprite = ballSprite;
        //Ball Value
        int ballNumber = questionModel.SetObjectValue(isAnswerBall);
        //Spawn Ball
        GameObject newBall = Instantiate(ballPrefab);
        //Ball Number
        if (questionModel.taskType == TaskType.Patterns)
        {
            int randomBallSpriteIndex = Random.Range(0, questionModel.patternSprites.Length);
            tempBallSprite = questionModel.patternSprites[randomBallSpriteIndex];
            if (!isAnswerBall)
            {
                newBall.GetComponent<BallScript>().ballNumber = int.Parse(tempBallSprite.name);
            }
            else if (questionModel.patternsClicked > 0)
            {
                newBall.GetComponent<BallScript>().ballNumber = int.Parse(questionModel.patternSprites[questionModel.patternSelected - 1].name);
                tempBallSprite = questionModel.patternSprites[questionModel.patternSelected - 1];
            }
        }
        else
        {
            newBall.GetComponent<BallScript>().ballNumber = ballNumber;
        }
        newBall.GetComponent<SpriteRenderer>().sprite = tempBallSprite;
        newBall.AddComponent<CircleCollider2D>();
        //Resize
        Vector3 scale = new Vector3(0.15f, 0.15f, newBall.transform.localScale.z);
        newBall.transform.localScale = scale;
        calculateObjectBoundaraies(newBall);
        //Set Text (if needed)
        if (questionModel.taskType != TaskType.Words && questionModel.taskType != TaskType.Patterns)
        {
            newBall.GetComponentInChildren<TextMesh>().text = string.Format(ballNumber.ToString(), newBall.transform.position.x, newBall.transform.position.y);
        }
        else if (questionModel.taskType == TaskType.Words)
        {
            newBall.GetComponentInChildren<TextMesh>().text = string.Format(System.Convert.ToChar(ballNumber).ToString(), newBall.transform.position.x, newBall.transform.position.y);
        }
        //Set position
        newBall.transform.position = calculatePosition();
    }

    void calculateObjectBoundaraies(GameObject ball)
    {
        objectWidth = ball.transform.GetComponent<SpriteRenderer>().bounds.extents.x; //divided by 2 because object is later on clamped only to its center
        objectHeight = ball.transform.GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    Vector3 calculatePosition()
    {
        float spawnX = Random.Range(0f, 1f);
        Vector3 spawnPosition = gameCamera.ViewportToWorldPoint(new Vector3(spawnX, 1f, 0));
        spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        float halfHeight = gameCamera.orthographicSize;
        float halfWidth = halfHeight * gameCamera.aspect;
        float horizontalMin = gameCamera.transform.position.x - halfWidth + objectWidth;
        float horizontalMax = gameCamera.transform.position.x + halfWidth - objectWidth;
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, horizontalMin, horizontalMax);
        return spawnPosition;
    }

    public bool CheckAnswer(int answer)
    {
        return questionModel.CheckAnswer(answer);
    }
}