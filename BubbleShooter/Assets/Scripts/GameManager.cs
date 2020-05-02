using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Shooter shootScript;
    public List<GameObject> bubblesPrefabs;
    public GameObject leftLinePrefab;
    public GameObject rightLinePrefab;

    public GameObject bubblesArea;
    public List<GameObject> bubbles;
    
    private List<GameObject> bubblesMirror;
    private List<GameObject> bubbleSequence;
    private GameObject currentBubble;

    private bool canShoot;
    private int sequenceSize = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        bubbleSequence = new List<GameObject>();
        bubbles = GameObject.FindGameObjectsWithTag("Bubble").ToList();
        CreateNextBubble();
    }

    void Update()
    {
        if (canShoot && Input.GetMouseButtonDown(0))
        {
            canShoot = false;
            shootScript.Shoot(currentBubble);
        }
    }

    void CreateNextBubble()
    {
        currentBubble = Instantiate(bubblesPrefabs[(int)(Random.Range(0, bubblesPrefabs.Count * 1000000f) / 1000000f)], shootScript.transform);
        currentBubble.GetComponent<Bubble>().isFixed = false;
        Rigidbody2D rb2d = currentBubble.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;
        canShoot = true;
    }

    public void EndOfTurn()
    {
        //Adicionar a bolha atirada no novo parent e transforma fixed para true
        currentBubble.transform.SetParent(bubblesArea.transform);

        //Recria a lista de bolinha da tela
        UpdateBubbleList();

        //Conferir se tem que destruir bolhas
        bubbleSequence.Clear();
        bubblesMirror = bubbles;
        CheckBubblesSequence(currentBubble.GetComponent<Bubble>());
        if (bubbleSequence.Count >= sequenceSize)
        {
            DestroyBubbleSequence();
        }

        //Cria uma nova bolha no gunpoint
        CreateNextBubble();
    }

    void CheckBubblesSequence(Bubble bubble)
    {
        bubbleSequence.Add(bubble.gameObject);
        bubblesMirror.Remove(bubble.gameObject);

        List<Bubble> bubblesCall = new List<Bubble>();
        foreach (GameObject go in bubblesMirror)
        {
            var bubbleScript = go.GetComponent<Bubble>();
            if (Vector2.Distance(bubble.gameObject.transform.position, go.transform.position) < (0.71f * 1.1f))
            {
                if (bubble.bubbleColor == bubbleScript.bubbleColor)
                {
                    bubblesCall.Add(bubbleScript);
                }
            }
        }

        foreach (Bubble b in bubblesCall)
        {
            CheckBubblesSequence(b);
        }
    }

    void DestroyBubbleSequence()
    {
        foreach (GameObject go in bubbleSequence)
        {
            Destroy(go);
        }
    }

    void UpdateBubbleList()
    {
        Transform[] allChildren = bubblesArea.GetComponentsInChildren<Transform>();

        List<GameObject> list = new List<GameObject>();
        foreach (Transform child in allChildren)
        {
            list.Add(child.gameObject);
        }

        bubbles = list;
    }

    [ContextMenu("TestNewLine")]
    void SetLevel()
    {
        var line = Instantiate(leftLinePrefab, bubblesArea.transform);
        FillBubbleLine(line);
        UpdateBubbleList();
    }

    void FillBubbleLine(GameObject line)
    {
        foreach (Transform t in line.transform)
        {
            var bubble = Instantiate(bubblesPrefabs[(int)(Random.Range(0, bubblesPrefabs.Count * 1000000f) / 1000000f)], bubblesArea.transform);
            bubble.transform.position = t.position;
        }

        Destroy(line);
    }

}