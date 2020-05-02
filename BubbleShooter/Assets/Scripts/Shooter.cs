using UnityEngine;

public class Shooter : MonoBehaviour
{
    public float speed = 8f;

    private Vector2 lookDirection;
    private float lookAngle;

    public void Update()
    {
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
    }

    public void Shoot(GameObject bubble)
    {
        bubble.GetComponent<Bubble>().isFixed = false;

        transform.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);
        bubble.transform.rotation = transform.rotation;

        bubble.GetComponent<Rigidbody2D>().AddForce(bubble.transform.up * speed, ForceMode2D.Impulse);
    }
}
