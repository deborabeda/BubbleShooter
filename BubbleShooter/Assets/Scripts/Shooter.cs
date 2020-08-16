using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform gunSprite;
    public bool canShoot;
    public float speed = 6f;

    public Transform nextBubblePosition;
    public GameObject currentBubble;
    public GameObject nextBubble;

    private Vector2 lookDirection;
    private float lookAngle;
    public bool isSwaping = false;
    public float time = 0.02f;

    public void Update()
    {
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        gunSprite.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);

        if(isSwaping)
        {
            if(Vector2.Distance(currentBubble.transform.position, nextBubblePosition.position) <= 0.2f
                && Vector2.Distance(nextBubble.transform.position, transform.position) <= 0.2f)
            {
                nextBubble.transform.position = transform.position;
                currentBubble.transform.position = nextBubblePosition.position;

                currentBubble.GetComponent<Collider2D>().enabled = true;
                nextBubble.GetComponent<Collider2D>().enabled = true;

                isSwaping = false;

                GameObject reference = currentBubble;
                currentBubble = nextBubble;
                nextBubble = reference;
            }

            nextBubble.transform.position = Vector2.Lerp(nextBubble.transform.position, transform.position, time);
            currentBubble.transform.position = Vector2.Lerp(currentBubble.transform.position, nextBubblePosition.position, time);
        }
    }

    public void Shoot()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);
        currentBubble.transform.rotation = transform.rotation;
        currentBubble.GetComponent<Rigidbody2D>().AddForce(currentBubble.transform.up * speed, ForceMode2D.Impulse);
        currentBubble = null;
    }

    [ContextMenu("SwapBubbles")]
    public void SwapBubbles()
    {
        currentBubble.GetComponent<Collider2D>().enabled = false;
        nextBubble.GetComponent<Collider2D>().enabled = false;
        isSwaping = true;
    }

    [ContextMenu("CreateNextBubble")]
    public void CreateNextBubble()
    {
        List<GameObject> bubblesInScene = LevelManager.instance.bubblesInScene;
        List<string> colors = LevelManager.instance.colorsInScene;

        if (nextBubble == null)
        {
            nextBubble = InstantiateNewBubble(bubblesInScene);
        }
        else
        {
            if(!colors.Contains(nextBubble.GetComponent<Bubble>().bubbleColor.ToString()))
            {
                Destroy(nextBubble);
                nextBubble = InstantiateNewBubble(bubblesInScene);
            }
        }

        if(currentBubble == null)
        {
            currentBubble = nextBubble;
            currentBubble.transform.position = new Vector2(transform.position.x, transform.position.y);
            nextBubble = InstantiateNewBubble(bubblesInScene);
        }
    }

    private GameObject InstantiateNewBubble(List<GameObject> bubblesInScene)
    {
        GameObject newBubble = Instantiate(bubblesInScene[(int)(Random.Range(0, bubblesInScene.Count * 1000000f) / 1000000f)]);
        newBubble.transform.position = new Vector2(nextBubblePosition.position.x, nextBubblePosition.position.y);
        newBubble.GetComponent<Bubble>().isFixed = false;
        Rigidbody2D rb2d = newBubble.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;

        return newBubble;
    }
}
