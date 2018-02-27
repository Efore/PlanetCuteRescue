using UnityEngine;
using Utils;

/// <summary>
/// Manager in charge of recolect every kind of click (mouse or touch) in the screen
/// and if it collides, triggers the event OnTouchCollider.
/// </summary>
public class InputManager : GenericManager<InputManager>
{

    public event System.Action<RaycastHit> OnTouchCollider;

    #region Event functions

    public override void OnEnable()
    {
        base.OnEnable();
        Input.simulateMouseWithTouches = true;
        Input.multiTouchEnabled = false;
        OnTouchCollider += new System.Action<RaycastHit>(emptyCollider);
    }
    
    /// <summary>
    /// Called every frame, it casts a ray where the touch has been perceived.
    /// </summary>
    void Update()
    {
        if (!GameManager.SINGLETON.PausedGame &&  !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            bool contact = false;
            Camera cam = Camera.main;

            if(Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began)
                contact = RayForPosition(cam, Input.touches[0].position);               
            
            if (Input.GetMouseButtonDown(0))
            {
                contact = RayForPosition(Camera.main, Input.mousePosition);
            }
            if(contact)
            {
                GameManager.SINGLETON.UseBullet();
            }
        }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Method responsible for casting a ray in a screen position provided, using the 
    /// main camera.
    /// </summary>
    /// <param name="cam">Camera to be used</param>
    /// <param name="screenPos">Screen coordinates of the touch</param>
    /// <returns>True if it collides with some element. False otherwise.</returns>
    private bool RayForPosition(Camera cam, Vector2 screenPos)
    {
        RaycastHit hit;
        Vector3 pos_VP = cam.ScreenToViewportPoint(screenPos);
        Ray ray = cam.ViewportPointToRay(pos_VP);
       
        if (Physics.Raycast(ray, out hit, 40.0f))
        {
            if (hit.collider.tag == "Enemy")
                GameManager.SINGLETON.AddKillingSpree(true);
            else
                GameManager.SINGLETON.AddKillingSpree(false);

            OnTouchCollider(hit);

            return true;
        }
        else return false;
    }

    private void emptyCollider(RaycastHit hit)
    { }

    #endregion
}
