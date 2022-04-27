using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    // Reference to the Player GameObject
    [SerializeField] private GameObject m_player;

    // UI Colors
    [SerializeField] private Color m_uiColor;
    [SerializeField] private Color m_healthBarColor;

    // Crosshair Sprites and Transforms
    [SerializeField] private Image m_crosshairImage;
    [SerializeField] private Image m_thresholdImage;
    private RectTransform m_crosshairTransform;
    private RectTransform m_thresholdTransform;
    private float m_thresholdWidth;
    private float m_thresholdHeight;

    // Shield Bar Images
    [SerializeField] private Image m_shieldBarOutlineImage;
    [SerializeField] private Image m_shieldBarFillImage;
    // Health Bar Images
    [SerializeField] private Image m_healthBarOutlineImage;
    [SerializeField] private Image m_healthBarFillImage;

    // Start is called before the first frame update
    void Start()
    {
        // Get the crosshair image transforms
        m_crosshairTransform = m_crosshairImage.GetComponent<RectTransform>();
        m_thresholdTransform = m_thresholdImage.GetComponent<RectTransform>();

        // Calculate the width and height of the crosshair threshold image
        Vector3[] corners = new Vector3[4];
        m_thresholdTransform.GetWorldCorners(corners);
        m_thresholdWidth = (corners[2] - corners[1]).x;
        m_thresholdHeight = (corners[1] - corners[0]).y;

        // Initialize with full Health and Shield
        UpdateHealthAndShield(1f, 1f);

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
            m_player.transform.Rotate(new Vector3(-directionToMouse.y * distance * 28.0f * Time.deltaTime, directionToMouse.x * distance * 28.0f * Time.deltaTime, 0f));
        }

        // If the player wants to shoot, fire a laser
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
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
        // Half the opacity of the UI color
        Color transparentUIColor = m_uiColor * new Color(1, 1, 1, 0.5f);

        // The Crosshair and Threshold images get the transparent Color
        m_crosshairImage.color = transparentUIColor;
        m_thresholdImage.color = transparentUIColor;
        // The Shield and Health Bar Outline images get the Outline Color
        m_shieldBarOutlineImage.color = m_uiColor;
        m_healthBarOutlineImage.color = m_uiColor;
        // The Shield and Health Bar Fill images get their respective colors
        m_shieldBarFillImage.color = transparentUIColor;
        m_healthBarFillImage.color = m_healthBarColor;
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
        float xRadius = Mathf.Pow(m_thresholdWidth / 2, 2);
        float yRadius = Mathf.Pow(m_thresholdHeight / 2, 2);
        // Compute each half of the ellipsis equation
        float xComponent = Mathf.Pow(Input.mousePosition.x - ellipsisCenter.x, 2) / xRadius; // (x - h)^2 / (rx)^2
        float yComponent = Mathf.Pow(Input.mousePosition.y - ellipsisCenter.y, 2) / yRadius; // (y - k)^2 / (ry)^2
        // Return the difference from 1
        return (xComponent + yComponent) - 1;
    }
}
