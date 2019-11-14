using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{
    public float speed;
    public float startWaitTime;
    public float waitTime;
   

    public Transform[] waypoints;
    private int waypointIndex = 0;   

    private void Start()
    {
        //waypoints = GetComponentsInChildren<Transform>();
      
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                waypointIndex = (waypointIndex < waypoints.Length -1) ? waypointIndex+1 : 0;
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player temp;
       if (temp = collision.gameObject.GetComponent<Player>())
        {
            temp.GetHit();
        }
        
    }  

    public int VerticalDirection()
    {
        Vector2 direction =  waypoints[waypointIndex].position - transform.position;
        return (int) Mathf.Sign(direction.y);
    }

    public enum EnemyType
    {
        spider,
        bat
    }
}
