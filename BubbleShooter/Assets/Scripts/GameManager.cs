using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Shooter shootScript;

    public Transform pointerToLastLine;
    public List<GameObject> referenceToBubblesInGame;
    
    private List<GameObject> mirrorToBubblesInGame;
    private List<GameObject> bubbleSequence;
    private GameObject currentBubble;

    private int sequenceSize = 3;

    void Start()
    {
        bubbleSequence = new List<GameObject>();

        LevelManager.instance.GenerateLevel();
        UpdateBubbleList();
        CreateNextBubble();
    }

    void Update()
    {
        if (shootScript.canShoot && Input.GetMouseButtonDown(0))
        {
            shootScript.canShoot = false;
            shootScript.Shoot(currentBubble);
        }
    }

    public void EndOfTurn()
    {
        currentBubble.transform.SetParent(LevelManager.instance.bubblesArea.transform);
        UpdateBubbleList();

        bubbleSequence.Clear();
        mirrorToBubblesInGame = referenceToBubblesInGame;
        CheckBubblesSequence(currentBubble.GetComponent<Bubble>());
        if (bubbleSequence.Count >= sequenceSize)
        {
            Invoke("DestroyBubbleSequence", 0.1f);
            Invoke("DropDesconectedBubbles", 0.1f);
        }

        LevelManager.instance.UpdateListOfBubblesInScene();
        CreateNextBubble();
    }

    void CreateNextBubble()
    {
        List<GameObject> bubblesInScene = LevelManager.instance.bubblesInScene;
        currentBubble = Instantiate(bubblesInScene[(int)(Random.Range(0, bubblesInScene.Count * 1000000f) / 1000000f)], shootScript.transform);
        currentBubble.GetComponent<Bubble>().isFixed = false;
        Rigidbody2D rb2d = currentBubble.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;
        shootScript.canShoot = true;
    }

    void CheckBubblesSequence(Bubble bubble)
    {
        bubbleSequence.Add(bubble.gameObject);
        mirrorToBubblesInGame.Remove(bubble.gameObject);

        List<Bubble> bubblesCall = new List<Bubble>();
        foreach (GameObject go in mirrorToBubblesInGame)
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

    void UpdateBubbleList()
    {
        Transform[] allChildren = LevelManager.instance.bubblesArea.GetComponentsInChildren<Transform>();

        referenceToBubblesInGame = new List<GameObject>();
        foreach (Transform child in allChildren)
        {
            referenceToBubblesInGame.Add(child.gameObject);
        }
    }

    void DestroyBubbleSequence()
    {
        foreach (GameObject go in bubbleSequence)
        {
            Destroy(go);
        }
    }

    private void DropDesconectedBubbles()
    {
        SetAllBubblesConnectionToFalse();
        SetConnectedBubblesToTrue();
        SetGravityToDesconectedBubbles();
    }

    private void SetAllBubblesConnectionToFalse()
    {
        foreach(Transform bubble in LevelManager.instance.bubblesArea.transform)
        {
            bubble.GetComponent<Bubble>().isConnected = false;
        }
    }

    private void SetConnectedBubblesToTrue()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(pointerToLastLine.position, pointerToLastLine.right, 15f);
        for (int i = 0; i < hits.Length; i++)
        { 
            if (hits[i].transform.gameObject.tag.Equals("Bubble"))
                SetNeighboursConnectionToTrue(hits[i].transform);
        }
    }

    private void SetNeighboursConnectionToTrue(Transform bubble)
    {
        if(bubble != null && bubble.gameObject.tag.Equals("Bubble") && !bubble.GetComponent<Bubble>().isConnected)
        {
            bubble.GetComponent<Bubble>().isConnected = true;
            RaycastHit2D[] neighbours = Physics2D.CircleCastAll(bubble.transform.position, 0.71f, bubble.transform.position);
            Debug.DrawRay(bubble.transform.position, bubble.transform.right, Color.yellow);
            for (int i = 0; i < neighbours.Length; i++)
            {
                SetNeighboursConnectionToTrue(neighbours[i].transform);
            }
        }

    }

    private void SetGravityToDesconectedBubbles()
    {
        foreach (Transform bubble in LevelManager.instance.bubblesArea.transform)
        {
            if(!bubble.GetComponent<Bubble>().isConnected)
            {
                bubble.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                Rigidbody2D rb2d = bubble.gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            }
        }
    }
}