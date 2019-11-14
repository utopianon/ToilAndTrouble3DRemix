using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCaster : MonoBehaviour
{
    public LayerMask grabbablesMask;
    public Texture2D cursorTexture;
    private Vector2 cursorHotspot;
    Camera mainCamera;
  
    void Start()
    {
        mainCamera = Camera.main;
        cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit;
            Vector3 currentMouse = Input.mousePosition;        
        
            if (hit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(currentMouse)))
            {
                if (hit.transform.tag == "Grabbable" || hit.transform.tag == "Ingredient")
                hit.transform.GetComponent<GrabbableObject>().Grabbed();
            }
                
        }
    }
}
