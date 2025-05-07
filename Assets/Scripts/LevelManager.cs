using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Header("Order System")]
    public List<string> orderList = new List<string>
    {
        "salad",
        "beef(Raw)",
        "beef(Medium)",
        "beef(WellDone)",
        "chicken(Raw)",
        "chicken(Medium)",
        "chicken(WellDone)",
        "salt",
        "pepper"
    };

    public GameObject orderBoxPrefab;
    public RectTransform topPanel;
    public float orderSpacing = 160f;
    public int maxOrders = 5;
    public float slideDuration = 0.4f;

    [Header("Audio")]
    public AudioClip orderSound;
    private AudioSource audioSource;

    [Header("Timer & Stars")]
    public float levelTime = 120f;
    private float remainingTime;
    public int maxStars = 5;
    private int currentStars;
    public float timeBeforeLosingStars = 60f;
    public float starLossInterval = 12f;

    [Header("UI")]
    public GameObject completeCanvas;
    public TextMeshProUGUI timerText;
    public int stars;

    private List<List<string>> activeOrderData = new List<List<string>>();
    private List<GameObject> activeOrders = new List<GameObject>();
    public Image[] starImg;
    public Sprite starSprite;
    public TextMeshProUGUI timerInfo;
    public bool Completed = false;
    public MoneyManager moneyManager;
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        remainingTime = levelTime;
        currentStars = maxStars;
        moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager == null)
            Debug.LogError("No LevelManager in scene!");

        completeCanvas.SetActive(false);

        StartCoroutine(LevelTimer());
        GenerateOrder();
        StartCoroutine(SpawnOrdersRoutine());
    }

    private IEnumerator LevelTimer()
    {
        float starLossTimer = 0f;

        while (remainingTime > 0 && !Completed)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime < (levelTime - timeBeforeLosingStars))
            {
                starLossTimer += Time.deltaTime;

                if (starLossTimer >= starLossInterval && currentStars > 0)
                {
                    currentStars--;
                    starLossTimer = 0f;
                }
            }

            UpdateTimerUI();
            yield return null;
        }

        currentStars = 0;
        UpdateTimerUI();
        CompleteGame();
    }

    private IEnumerator SpawnOrdersRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            GenerateOrder();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(remainingTime)}s";

        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            CompleteGame();
        }
    }
    public void CompleteGame()
    {
        Completed = true;
        //.timeScale = 0f; // Pause the game
        if (remainingTime > 60)
        {
            stars = 5;
        }
        else if (remainingTime > 50)
        {
            stars = 4;
        }
        else if (remainingTime > 40)
        {
            stars = 3;
        }
        else if (remainingTime > 30)
        {
            stars = 2;
        }
        else if (remainingTime > 20)
        {
            stars = 1;
        }
        else if (remainingTime > 10)
        {
            stars = 1;
        }
        else if (remainingTime <= 0)
        {
            stars = 0;

        }
        GameManager.Instance.level2Locked = false;
        GameManager.Instance.level3Locked = false;
        moneyText.text = "Money: " + moneyManager.money.ToString();


        completeCanvas.SetActive(true);
        timerInfo.text = "Completed In: " + (int)remainingTime + "s";

        for (int i = 0; i < currentStars; i++)
        {
            starImg[i].sprite = starSprite;
            Debug.Log($"Loop {i + 1}: Star reward processing...");
            // Insert any actions here (like reward animations, spawning effects, etc.)
        }

    }

    public void GenerateOrder()
    {
        if (activeOrders.Count >= maxOrders)
            return;

        int orderSize = Random.Range(2, 5); // 2 to 4 items
        List<string> selectedItems = new List<string>();
        List<string> tempList = new List<string>(orderList);

        for (int i = 0; i < orderSize && tempList.Count > 0; i++)
        {
            int index = Random.Range(0, tempList.Count);
            selectedItems.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        activeOrderData.Add(new List<string>(selectedItems));

        string orderText = string.Join(", ", selectedItems);
        NewOrder(selectedItems, orderText);
    }

    public int FindMatchingOrder(List<string> plateItems)
    {
        var sortedPlate = plateItems.Select(s => s.ToLower()).OrderBy(x => x).ToArray();
        for (int i = 0; i < activeOrderData.Count; i++)
        {
            var ord = activeOrderData[i]
                           .Select(s => s.ToLower())
                           .OrderBy(x => x)
                           .ToArray();
            if (sortedPlate.SequenceEqual(ord))
                Destroy(activeOrders[i]); // remove the order UI
            return i;
        }
        return -1;
    }

    public void NewOrder(List<string> order, string message)
    {
        // don't exceed max
        if (activeOrders.Count >= maxOrders)
            return;

        // store the order’s data so FindMatchingOrder and CompleteOrder can see it
        activeOrderData.Add(new List<string>(order));

        // play sound
        if (orderSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(orderSound);
        }

        // instantiate UI box
        GameObject newOrder = Instantiate(orderBoxPrefab, topPanel);
        newOrder.transform.SetAsLastSibling();

        // position off‑screen then slide in
        RectTransform rect = newOrder.GetComponent<RectTransform>();
        Vector2 targetPos = new Vector2(activeOrders.Count * orderSpacing, 0f);
        rect.anchoredPosition = new Vector2(-500f, 0f);

        // fill text
        TextMeshProUGUI text = newOrder.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var item in order)
                sb.AppendLine("• " + item);
            text.text = sb.ToString();
        }

        StartCoroutine(SlideToPositionEased(rect, targetPos, slideDuration));

        // keep track of the UI box so we can delete it later
        activeOrders.Add(newOrder);
    }

    private IEnumerator SlideToPositionEased(RectTransform rect, Vector2 target, float duration)
    {
        Vector2 start = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = 1f - Mathf.Pow(1f - t, 3); // EaseOutCubic
            rect.anchoredPosition = Vector2.LerpUnclamped(start, target, easedT);
            yield return null;
        }

        rect.anchoredPosition = target;
    }
}
