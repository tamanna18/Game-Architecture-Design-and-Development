using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallScript : MonoBehaviour, IPointerClickHandler
{
    private Vector2 screenBoundsMin;
    private Vector2 screenBoundsMax;
    public int ballNumber;
    public Sprite[] answerSprites;
    public static bool leftTimerSet = false;
    private bool isClicked = false;

    //TODO: Need to set camera dynamically when games are done (and not in Editor), then recalculate screen bounds?
    public Camera gameCamera;
    void Awake()
    {
        gameCamera = GameObject.FindWithTag("MiniGameLeftCamera").GetComponent<Camera>();
    }
    void Start()
    {
        float halfHeight = gameCamera.orthographicSize;
        float halfWidth = halfHeight * gameCamera.aspect;
        float horizontalMin = gameCamera.transform.position.x - halfWidth;
        float horizontalMax = gameCamera.transform.position.x + halfWidth;
        float verticalMin = gameCamera.transform.position.y - halfHeight;
        float verticalMax = gameCamera.transform.position.y + halfHeight;
        screenBoundsMin = new Vector2(horizontalMin, verticalMin);
        screenBoundsMax = new Vector2(horizontalMax, verticalMax);
    }
    void Update()
    {
        if (transform.position.y < screenBoundsMin.y)
        {
            Destroy(this.gameObject);
        }

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClicked)
        {
            GameManager.Instance.StartMinigame(GameIdentifier.MiniGameLeft);
            if (!leftTimerSet)
            {
                switch (BallsSpawner.Instance.questionModel.taskType)
                {
                    case TaskType.MathEquation:
                        leftTimerSet = true;
                        GameManager.Instance.minigameTimer = 10f;
                        break;
                    case TaskType.Parity:
                        leftTimerSet = true;
                        GameManager.Instance.minigameTimer = 20f;
                        break;
                    case TaskType.Words:
                        leftTimerSet = true;
                        GameManager.Instance.minigameTimer = 30f;
                        break;
                    case TaskType.Patterns:
                        leftTimerSet = true;
                        GameManager.Instance.minigameTimer = 20f;
                        break;
                }
            }
            if (GameManager.Instance.minigameSelected == false)
            {
                GameManager.Instance.minigameSelected = true;
                GameManager.Instance.startMinigameTimer(GameIdentifier.MiniGameLeft);
            }
            // Add support for string conversion depending on task type (words vs numbers)
            this.GetComponentInChildren<TextMesh>().text = "";
            if (BallsSpawner.Instance.CheckAnswer(ballNumber))
            {
                this.GetComponent<SpriteRenderer>().sprite = answerSprites[0];
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = answerSprites[1];
            }
            StartCoroutine(holdSpriteChange());
            isClicked = true;
        }

    }
    IEnumerator holdSpriteChange()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }


}
