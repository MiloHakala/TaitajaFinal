using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class LevelManager : MonoBehaviour
{
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

    public GameObject orderBoxPrefab;         // Prefab with Image + Text
    public RectTransform topPanel;            // The UI parent (anchor at top of screen)
    public float orderSpacing = 160f;         // Horizontal space between orders
    public int maxOrders = 5;                 // Max orders visible at once
    public float slideDuration = 0.4f;        // Slide animation duration

    public AudioClip orderSound;              // Sound to play when order is created
    private AudioSource audioSource;

    private List<List<string>> activeOrderData = new List<List<string>>();
    private List<GameObject> activeOrders = new List<GameObject>();

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Called by UI Button
    public void GenerateOrder()
    {
        if (activeOrders.Count >= maxOrders)
            return;

        int orderSize = Random.Range(2, 5); // 1 to 3 items in an order
        List<string> selectedItems = new List<string>();
        List<string> tempList = new List<string>(orderList);

        for (int i = 0; i < orderSize && tempList.Count > 0; i++)
        {
            int index = Random.Range(0, tempList.Count);
            selectedItems.Add(tempList[index]);
            tempList.RemoveAt(index); // Prevent duplicate items in one order
        }

        // store the generated order data
        activeOrderData.Add(new List<string>(selectedItems));

        string orderText = string.Join(", ", selectedItems); // e.g., "beef(Medium), salt, pepper"
        NewOrder(selectedItems, orderText);
    }

    // <summary>
    /// Returns the index (0‑based) of a matching order, or –1 if none match.
    /// Comparison ignores order of items but requires same counts.
    /// </summary>
    public int FindMatchingOrder(List<string> plateItems)
    {
        var sortedPlate = plateItems.OrderBy(x => x).ToArray();
        for (int i = 0; i<activeOrderData.Count; i++)
        {
            var ord = activeOrderData[i];
            var sortedOrd = ord.OrderBy(x => x).ToArray();
            if (sortedPlate.SequenceEqual(sortedOrd))
                return i;
        }
        return -1;
    }

    public void NewOrder(List<string> order, string message)
    {
        if (activeOrders.Count >= maxOrders)
            return;

        // Play sound with slight pitch variation
        if (orderSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(orderSound);
        }

        // Instantiate order box off-screen to the left
        GameObject newOrder = Instantiate(orderBoxPrefab, topPanel);
        newOrder.transform.SetAsLastSibling();

        RectTransform rect = newOrder.GetComponent<RectTransform>();
        Vector2 targetPos = new Vector2(activeOrders.Count * orderSpacing, 0f);

        // Set starting position off-screen to the left
        rect.anchoredPosition = new Vector2(-500f, 0f);

        // Set the order message text
        TextMeshProUGUI text = newOrder.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            // build a bullet list
            var sb = new System.Text.StringBuilder();
            foreach (var item in order)
            {
                sb.AppendLine("• " + item);
            }
            text.text = sb.ToString();
        }

        // Animate slide-in
        StartCoroutine(SlideToPositionEased(rect, targetPos, slideDuration));

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

/*
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
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

    public GameObject orderBoxPrefab;         // Prefab with Image + Text
    public RectTransform topPanel;            // The UI parent (anchor at top of screen)
    public float orderSpacing = 160f;         // Horizontal space between orders
    public int maxOrders = 5;                 // Max orders visible at once
    public float slideDuration = 0.4f;        // Slide animation duration

    public AudioClip orderSound;              // Sound to play when order is created
    private AudioSource audioSource;

    private List<GameObject> activeOrders = new List<GameObject>();

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void Start()
    {
        
    }

    public void NewOrder(List<string> order, string message)
    {
        if (activeOrders.Count >= maxOrders)
            return;
        if (orderSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(orderSound);
        }
        // Instantiate order box off-screen to the left
        GameObject newOrder = Instantiate(orderBoxPrefab, topPanel);
        newOrder.transform.SetAsLastSibling();

        RectTransform rect = newOrder.GetComponent<RectTransform>();
        Vector2 targetPos = new Vector2(activeOrders.Count * orderSpacing, 0f);

        // Start far left offscreen
        rect.anchoredPosition = new Vector2(-500f, 0f);

        // Set the message text
        Text text = newOrder.GetComponentInChildren<Text>();
        if (text != null)
            text.text = message;

        // Play sound with pitch variation
        

        // Start animation
        StartCoroutine(SlideToPositionEased(rect, targetPos, slideDuration));
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

            // EaseOutCubic
            float easedT = 1f - Mathf.Pow(1f - t, 3);
            rect.anchoredPosition = Vector2.LerpUnclamped(start, target, easedT);

            yield return null;
        }

        rect.anchoredPosition = target;
    }
    public void GenerateOrder()
    {
        int orderSize = Random.Range(1, 4);
        for (int i = 0; i < orderSize; i++)
        {
            int orderIndex = Random.Range(0, orderList.Count);
            // get random index from order list
        }
    }
}
*/