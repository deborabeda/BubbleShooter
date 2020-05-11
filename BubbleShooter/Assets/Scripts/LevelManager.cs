using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public GameObject bubblesArea;
    public List<GameObject> bubblesPrefabs;
    public List<GameObject> bubblesInScene;

    public void GenerateLevel()
    {
        bubblesInScene = new List<GameObject>();
        FillWithBubbles(GameObject.FindGameObjectWithTag("InitialLevelScene"), bubblesPrefabs);
        UpdateListOfBubblesInScene();
    }

    public void UpdateListOfBubblesInScene()
    {
        bubblesInScene = GetListOfBubblesInScene();
    }

    private List<GameObject> GetListOfBubblesInScene()
    {
        List<string> types = new List<string>();
        List<GameObject> newListOfBubbles = new List<GameObject>();

        foreach (Transform t in bubblesArea.transform)
        {
            Bubble b = t.GetComponent<Bubble>();
            if (!types.Contains(b.bubbleColor.ToString()))
            {
                string color = b.bubbleColor.ToString();
                types.Add(color);

                foreach (GameObject prefab in bubblesPrefabs)
                {
                    if (color.Equals(prefab.GetComponent<Bubble>().bubbleColor.ToString()))
                    {
                        newListOfBubbles.Add(prefab);
                    }
                }
            }
        }

       return newListOfBubbles;
    }

    private void FillWithBubbles(GameObject go, List<GameObject> bubbles)
    {
        foreach (Transform t in go.transform)
        {
            var bubble = Instantiate(bubbles[(int)(Random.Range(0, bubbles.Count * 1000000f) / 1000000f)], bubblesArea.transform);
            bubble.transform.position = t.position;
        }

        Destroy(go);
    }

}
