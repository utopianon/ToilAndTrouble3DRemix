using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    public Sprite[] possibleIngredients;
    List<GrabbableObject> ingredientsInScene;
    int receivedIngredients = 0;
    GameObject wantedSign;
    bool thinking = false;
    public IngredientType wantedIngredient;
    bool receivingInmgredients;
    public float comboTimer = 0.5f;
    public int scorePerIngredient = 20;

    float timer;


    public void Start()
    {
        wantedSign = transform.GetChild(0).gameObject;
        LoadIngrediensInScene();
        ChooseIngredient();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChooseIngredient();
        }
    }

    private void LoadIngrediensInScene()
    {
        GameObject holder = GameObject.FindGameObjectWithTag("IngredientHolder");
        Ingredient[] ingredients = holder.GetComponentsInChildren<Ingredient>();
        ingredientsInScene = new List<GrabbableObject>();
        foreach (Ingredient i in ingredients)
        {
            ingredientsInScene.Add(i);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidingObject = collision.gameObject;

        if (collidingObject.tag == "Ingredient")
        {
            Ingredient temp = collidingObject.GetComponent<Ingredient>();
            if (temp.type == wantedIngredient)
            {
                receivedIngredients++;
                ingredientsInScene.Remove(temp);
                Destroy(collidingObject);
                if (!thinking)
                {
                    thinking = true;
                    StartCoroutine(NewIngredientTimer());
                }
            }
            else
            {
                ingredientsInScene.Remove(temp);
                Destroy(collidingObject);
            }
        }
    }

    public IEnumerator NewIngredientTimer()
    {
        yield return new WaitForSeconds(0.5f);
        UseIngredients();
        ChooseIngredient();
        Debug.Log(ingredientsInScene.Count + " ingredients in scene");
    }

    void UseIngredients()
    {
        float multiplier = 0.5f;

        for (int i = receivedIngredients; i > 0; i--)
        {
            multiplier += 0.5f;
        }
        GameMaster.Instance.AddScore(multiplier, receivedIngredients * scorePerIngredient);
        receivedIngredients = 0;
    }

    public void ChangeMind(List<GrabbableObject> ingredients)
    {
        Debug.Log("list size is " + ingredients.Count);
        for (int i = 0; i < ingredients.Count; i++)
        {
            int index = ingredientsInScene.LastIndexOf(ingredients[i]);
            ingredientsInScene.RemoveAt(index);
        }
        StartCoroutine(NewIngredientTimer());

    }

    public void ChooseIngredient()
    {
        if (ingredientsInScene.Count > 0)
        {
            int index = Random.Range(0, ingredientsInScene.Count);
            wantedIngredient = ingredientsInScene[index].type; 
            wantedSign.GetComponent<SpriteRenderer>().sprite = possibleIngredients[(int)wantedIngredient];
            thinking = false;
        }
        else
        {
            //won scene
            wantedSign.SetActive(false);
            Debug.Log("won scene");
        }
    }

}
