using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    // Reference to the Player GameObject
    [SerializeField] private GameObject m_player;

    // UI Colors
    public Color m_crosshairColor;
    public Color m_healthShieldOutlineColor;
    public Color m_healthBarFillColor;
    public Color m_shieldBarFillColor;

    // Crosshair Sprites and Transforms
    public Image m_crosshairImage;
    public Image m_thresholdImage;
    private RectTransform m_crosshairTransform;
    private RectTransform m_thresholdTransform;

    // Shield Bar Images
    public Image m_shieldBarOutlineImage;
    public Image m_shieldBarFillImage;
    // Health Bar Images
    public Image m_healthBarOutlineImage;
    public Image m_healthBarFillImage;

    // Start is called before the first frame update
    void Start()
    {
        // Get the crosshair image transforms
        m_crosshairTransform = m_crosshairImage.GetComponent<RectTransform>();
        m_thresholdTransform = m_thresholdImage.GetComponent<RectTransform>();

        ColorUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the Crosshair Sprite's position to the current position of the mouse
        m_crosshairTransform.position = Input.mousePosition;

        // Compute the center of the ellipsis (should be center screen)
        Vector3 ellipsisCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        // Calculate how far away we are from the threshold
        float distance = DistancePastThreshold(ellipsisCenter);

        // If the crosshair is outside the threshold, then rotate the player's ship
        if (distance > 0f)
        {
            // Normalize the vector from the screen center to the mouse position to determine
            // how much to rotate in the X and Y directions respectively
            Vector3 directionToMouse = (Input.mousePosition - ellipsisCenter).normalized;
            // Rearrange the components so the player rotates properly and multiply by the
            // distance from the threshold: the further away, the faster you rotate
            m_player.transform.Rotate(new Vector3(-directionToMouse.y * distance * 0.1f, directionToMouse.x * distance * 0.1f, 0f));
        }
        // Otherwise, if the crosshair is inside the threshold and the player wants to shoot, fire a laser
        else if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            m_player.GetComponent<PlayerController>().FireLaser();
        }
    }

    /// <summary>
    /// Update the Health and Shield Bars.
    /// </summary>
    /// <param name="healthPercent">The player's current health as a percentage of the max</param>
    /// <param name="shieldPercent">The player's current shield as a percentage of the max</param>
    public void UpdateHealthAndShield(float healthPercent, float shieldPercent)
    {
        m_healthBarFillImage.fillAmount = healthPercent;
        m_shieldBarFillImage.fillAmount = shieldPercent;
    }    

    /// <summary>
    /// Set the colors of the different UI sprite elements. If the seconday color
    /// is either black (0,0,0,1) or clear (0,0,0,0), set all Sprites to the 
    /// primary color.
    /// </summary>
    private void ColorUI()
    {
        // The Crosshair and Threshold images get the Crosshair Color
        m_crosshairImage.color = m_crosshairColor;
        m_thresholdImage.color = m_crosshairColor;
        // The Shield and Health Bar Outline images get the Outline Color
        m_shieldBarOutlineImage.color = m_healthShieldOutlineColor;
        m_healthBarOutlineImage.color = m_healthShieldOutlineColor;
        // The Shield and Health Bar Fill images get their respective colors
        m_shieldBarFillImage.color = m_shieldBarFillColor;
        m_healthBarFillImage.color = m_healthBarFillColor;
    }

    /// <summary>
    /// Determine whether the crosshair is beyond the threshold ellipsis.
    /// Compute using the following equation for whether a point (x,y) is 
    /// inside an ellipsis centered at (h,k) of size (rx,ry).
    /// [ (x - h)^2 / (rx)^2 ] + [ (y - k)^2 / (ry)^2 ] <= 1
    /// (x,y): Input Mouse Position
    /// (h,k): Center of the ellipsis, passed in as argument
    /// (rx,ry): Half the width and height of the threshold image transform
    /// </summary>
    /// <returns>
    /// Returns the distance from the Mouse Position to the Threshold
    /// dist > 0: outside the threshold
    /// dist <= 0: inside the threshold
    /// </returns>
    private float DistancePastThreshold(Vector3 ellipsisCenter)
    {
        // Compute (rx^2 and ry^2): half the dimensions of the Threshold Image, then squared
        float xRadius = Mathf.Pow(m_thresholdTransform.rect.width * m_thresholdTransform.localScale.x / 2, 2);
        float yRadius = Mathf.Pow(m_thresholdTransform.rect.height * m_thresholdTransform.localScale.y / 2, 2);
        // Compute each half of the ellipsis equation
        float xComponent = Mathf.Pow(Input.mousePosition.x - ellipsisCenter.x, 2) / xRadius; // (x - h)^2 / (rx)^2
        float yComponent = Mathf.Pow(Input.mousePosition.y - ellipsisCenter.y, 2) / yRadius; // (y - k)^2 / (ry)^2
        // Return the difference from 1
        return (xComponent + yComponent) - 1;
    }
}
