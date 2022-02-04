using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairMovement : MonoBehaviour
{
    public RectTransform crosshair;
    Vector3 offset;
    float SCREEN_HEIGHT;
    float SCREEN_WIDTH;
    // Start is called before the first frame update
    void Start()
    {
        crosshair = gameObject.GetComponent<RectTransform>();
        //This is a little lengthy but it lets us offset the weird bump  made by the first translation
        SCREEN_WIDTH = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect.width;
        SCREEN_HEIGHT = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect.height;
        offset = new Vector3(-SCREEN_WIDTH/2.0f, -SCREEN_HEIGHT/2.0f, 0);
    }

    // Update is called once per frame
    void Update()
    {

        //A translation of the negative of the current position will always move towards the center
        //Point1 - Point2 = vector going from point 2 to point 1
        //so origin (0,0,0) - crosshair poistion = vector from  crosshair to center
        //so: crosshair position + vector  = (0,0,0)
        //This line will always reset to center
        //crosshair.Translate(-crosshair.localPosition, Space.Self);

        //This line ties the crosshair to your mouse, but for some reason always appear to the top right
        //If you mouse is inthe bottom left of the screen, it becomes centered
        crosshair.Translate(Input.mousePosition - crosshair.localPosition, Space.Self);
        crosshair.Translate(offset, Space.Self);

        OnGUI();
    }

    public void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.box.fontSize = 15;
        GUI.skin.box.wordWrap = false;

        GUI.Box(new Rect(0, 60, 300, 30), "Current Mouse Pos: " + Input.mousePosition);
        GUI.Box(new Rect(0, 90, 300, 30), "Screen Width: " + SCREEN_WIDTH + " Screen Height: " + SCREEN_HEIGHT);
    }
}
