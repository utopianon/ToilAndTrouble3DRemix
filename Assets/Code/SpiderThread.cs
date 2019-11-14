using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderThread : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Enemy spider;

    public bool toggle = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Spider" && !toggle)
        {
            if (!spider)
            {
                spider = collision.gameObject.GetComponent<Enemy>();
            }

            if (spider.VerticalDirection() < 0)
            {
                toggle = !toggle;
                spriteRenderer.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    

        if (collision.gameObject.tag == "Spider" && toggle)
        {
            if (spider.VerticalDirection() > 0)
            {
                spriteRenderer.enabled = false;
                toggle = !toggle;
            }

        }
    }



}
