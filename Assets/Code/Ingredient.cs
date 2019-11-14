using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J
}

public class Ingredient : GrabbableObject
{  
    private Vector3 velocity;

    private void Update()
    {
        if (!grabbed)        {

            if (collisions.above || collisions.below)
            {
                velocity.y = 0;
            }

            velocity.y += gravity * Time.deltaTime;
            Move(velocity * Time.deltaTime);
        }

    }

}

