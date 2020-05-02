using UnityEngine;

public class Bubble : MonoBehaviour
{
    public bool isFixed;
    public BubbleColor bubbleColor;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bubble")
        {
            if (!isFixed)
            {
                HasCollided();
            }
        }

        if (collision.gameObject.tag == "Limit")
        {
            if (!isFixed)
            {
                HasCollided();
            }
        }
    }

    void HasCollided()
    {
        var rb = GetComponent<Rigidbody2D>();
        Destroy(rb);
        isFixed = true;
        GameManager.instance.EndOfTurn();

    }

    public enum BubbleColor
    {
        BLUE, YELLOW, RED, GREEN
    }
}
