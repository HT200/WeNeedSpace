using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;

    // The player's health and damage output
    // Temporary?
    public float health = 100.0f;
    public float damage = 5.0f;

    Vector3 pos;
    Vector3 vel;
    Vector3 acc;

    public GameObject laserfire;
    float lasercooldown;
    
    float speed;
    float SCREEN_WIDTH;
    float SCREEN_HEIGHT;
    bool MajorAxis;
    Vector3 f1;
    Vector3 f2;
    Vector2 centerScreen;
    float limit;
    bool test;
    Vector2 rot;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        test = false;
        limit = 100;
        lasercooldown = 0;
        speed = 0.0000f;
        pos = transform.position;
        vel = transform.forward * 0.00f;
        acc = transform.forward * speed;
        SCREEN_WIDTH = Screen.width;
        SCREEN_HEIGHT = Screen.height;
        centerScreen = new Vector2(SCREEN_WIDTH / 2.0f, SCREEN_HEIGHT / 2.0f);


        //the radius of the gui circle is 315 with my current resolution
        //So when mouse position is more than 315 units away from (SCREEN_WIDTH,SCREEN_HEIGHT) it should start rotating the ship
        //However hard coding it to 315 wouldn't react to different resolutions
        //So it should be represented by a percentage of the screen instead
        //Additionally since the measurements of Screen_Width and Screen_Height are done in the start method it doesnt react to resizing

        //Addtionally additionally I'm fitting a circle in a rectangle, stretching the rectangle would create an ellipse so maybe distance should
        //be cross-referenced?

        //currently the diameter is 630  so 630/1920 is the ratio of the width
        //Similarly the ratio for height is 630/907

        //1:Any point on an ellipse can be described as (distance from focus 1 + distance from focus 2) = diameter of major axis
        //2:What needs to be known: focus 1, focus 2, the major axis
        //3:The major axis can be found easily: multiply both screen attributes by their respective ratios, see which one is bigger
        //4:Now that we know the major axis, we also know the diameter of the major axis
        //5:going back to line 1, we see we have a variable solved
        //6:If you think about it, either of the points on the minor axis have an equal distance to both foci
        //7:By the same method in line 3, we can find the minor diameter and subsequently the points on it
        //8:Take one of those points, and imagine a triangle with that point, one of the foci, and the origin
        //9:The height is the radius of the minor axis, and using line 7, we already have that
        //10:and from line 1  -> 2(distance from either focus) = diameter of major axis -> distance from either focus = radius of major axis
        //11:So the hypotenuse is the radius of the major axis
        //12:By the pythagorean theorem, we can find the third side, which is each focus' distance to the origin
        //13:Now we just travel that distance along the major axis in both dimensions, and we have both foci 

        float wRatio = 630.0f / 1920.0f;
        float hRatio = 630.0f / 907.0f;
        f1 = centerScreen;
        f2 = f1;

        MajorAxis = SCREEN_HEIGHT * hRatio > SCREEN_WIDTH * wRatio;
        if (MajorAxis)
        {
            //This is if the screen height is stretched more than width, it means that the height is the major axis
            Vector2 mPoint = new Vector2((SCREEN_WIDTH * wRatio) / 2.0f, 0);
            Vector2 MAxis = new Vector2(0, SCREEN_HEIGHT * hRatio / 2.0f);
            float focusOffset = Mathf.Sqrt(MAxis.sqrMagnitude - mPoint.sqrMagnitude);
            f1 += new Vector3(0, focusOffset);
            f2 += new Vector3(0, -focusOffset);
            limit = SCREEN_HEIGHT * hRatio / 2.0f;
        }
        else
        {
            Vector2 mPoint = new Vector2((SCREEN_HEIGHT * hRatio) / 2.0f, 0);
            Vector2 MAxis = new Vector2(0, SCREEN_WIDTH * wRatio / 2.0f);
            float focusOffset = Mathf.Sqrt(MAxis.sqrMagnitude - mPoint.sqrMagnitude);
            f1 += new Vector3(focusOffset, 0);
            f2 += new Vector3(-focusOffset, 0);
            limit = SCREEN_WIDTH * wRatio / 2.0f;
        }

    }
    //Starting speed is 10^-2 unity blocks per frame
    //acceleration increments by 10^-7 unity blocks per frame^2

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        //Acceleration controls
        if (Input.GetKey(KeyCode.W))
        {
            if (speed < 1.0f)
            {
                speed += 0.1f * dt;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (speed > 0.0f)
            {
                speed -= 0.1f * dt;
            }
        }

        //Rotation Controls via keyboard (Roll)
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, 0, 0.1f, Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 0, -0.1f, Space.Self);
        }

        // if (Input.GetKey(KeyCode.UpArrow))
        // {
        //     transform.Rotate(0.1f, 0, 0, Space.Self);
        // }
        // if (Input.GetKey(KeyCode.DownArrow))
        // {
        //     transform.Rotate(-0.1f, 0, 0, Space.Self);
        // }

        rot = ((Vector2)Input.mousePosition - centerScreen);

        //All rotation via mouse input (Pitch, Yaw)
        if ((Input.mousePosition - f1).magnitude + (Input.mousePosition - f2).magnitude > 2 * limit)
        {
            test = true;
            transform.Rotate(-rot.normalized.y * 0.1f, rot.normalized.x * 0.1f, 0.0f);
        }
        else
        {
            test = false;
        }

        rot /= limit;

        //Firing mechanism
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && lasercooldown <= 0.0f)
        {
            //Currently this has the laserfire instantiated at the cylinder end, and the rotation is perfectly alignned with the "cannon"
            //In the future it might be best to have the rotation be slightly randomized to allow for "bullet" spread
            //Additionally it might be best to tweak the code so it doesnt perform physical movement on a frame by frame basis but
            //instead uses actual time since the last frame to account for differing computer speeds

            //You may be wondering why im creating a temp object when the Instantiate() method can be called alone, creating
            //A temp object allows us to reference it and its subcomponents if we want to after firing
            //So if we create more complex laser fire it might be necessary
            GameObject temp = Instantiate(laserfire, transform.position + transform.forward * 1.5f, transform.rotation);
            temp.transform.Rotate(-rot.y * 21.8f, rot.x * 21.8f, 0.0f);
            // the interval between shots is 1/5th of a second
            lasercooldown = 0.2f;
        }
        else if (lasercooldown > 0.0f)
        {
            lasercooldown -= dt;
        }

        //ALL ROTATIONS SHOULD BE APPLIED BEFORE VECTOR CHANGES
        //Since acc is dependent on the transform.forward

        //Vector changes applied

        acc = transform.forward * speed;
        if (vel.magnitude >= 5.0f)
        {
            //This means that velocity is already at max

            //If velocity is at max, you cant acclerate FORWARD, but you can accelerate in the sense of turning
            //To replicate this, if  at max speed, update only the direction of the velocity not the magnitude

            //EDIT: because of how this works there is no loss of speed while turning
            //as a result you can turn on a dime and suddenly be moving in a new direction easily
            //We need to create a way to LOSE speed not only during turns but in general as well

            vel = (vel + acc).normalized * 5f;
        }
        else
        {
            vel += acc * dt;
        }

        pos += vel * dt;
        transform.position = pos;
        // OnGUI();
    }

    /// <summary>
    /// Update the player's health
    /// </summary>
    public void UpdateHealth(float num)
    {
        if (num < 0)
        {
            // Player has taken damage, reset combo
            gameManager.SetCombo(1);
        }

        health += num;
        print(num + " Health");
    }

    /// <summary>
    /// Update the player's damage output
    /// </summary>
    public void UpdateDamage(float num)
    {
        // TODO: Upgrade weapons for more damage?
        damage += num;
        print("Damage: " + damage);
    }

    public void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.box.fontSize = 15;
        GUI.skin.box.wordWrap = false;
        /*

        GUI.Box(new Rect(0, 0, 300, 30), "Current velocity: " + Mathf.Round(vel.magnitude * 10000) / 100 + " x 10^-2");
        GUI.Box(new Rect(0, 30, 300, 30), "Acceleration: " + speed);
        GUI.Box(new Rect(0, 120, 300, 30), "Screen Width: " + SCREEN_WIDTH + " Screen Height: " + SCREEN_HEIGHT);
        GUI.Box(new Rect(0, 150, 300, 30), "F1: " + f1 + " F2: " + f2);
        GUI.Box(new Rect(0, 180, 300, 30), "Mouse Position: " + Input.mousePosition);
        GUI.Box(new Rect(0, 210, 300, 30), "Limit: " + limit);
        if (test)
        {
            GUI.Box(new Rect(0, 240, 300, 30), "You are outside the ellipse");
        }
        GUI.Box(new Rect(0, 270, 300, 30), "Rotation: " + rot);
        */
    }
}
