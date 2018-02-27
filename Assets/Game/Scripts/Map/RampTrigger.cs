using UnityEngine;
using SGame.Entities.Common;
using Utils;

/// <summary>
/// Component in charge of handling trigger events and changing enemies or characters' movement when they are entering or exiting
/// a ramp.
/// </summary>
public class RampTrigger : MonoBehaviour {
        
	/// <summary>
    /// Method responsible for rotating and changing movement of the entering entity according to the type of ramp and the previous
    /// direction of the entity.
    /// </summary>
    /// <param name="other">Entity to modify.</param>
    void OnTriggerEnter(Collider other)
    {
        if((other.tag == "Enemy" || other.tag == "Character") && Mathf.Floor(other.transform.position.z) == Mathf.Floor(transform.position.z))
        {           
            MovableEntity movableEntity = other.GetComponent<MovableEntity>();
            float directionAngle = 0.0f;
            float rotationAngle = 0.0f;


            if (gameObject.tag == "RampLeft")
            {
                if (movableEntity.MovementDirection.x > 0)
                {
                    directionAngle = 20;
                    rotationAngle = 20;
                }
                else
                {
                    directionAngle = 20;
                    rotationAngle = -20;
                }
            }
            else
            {
                if (movableEntity.MovementDirection.x > 0)
                {
                    directionAngle = -20;
                    rotationAngle = -20;
                }
                else
                {
                    directionAngle = -20;
                    rotationAngle = 20;
                }
            }

            if (directionAngle != 0.0f)
            {
                movableEntity.transform.Rotate(Vector3.forward, rotationAngle);
                Vector3 movement = Transform2D.V2toV3(movableEntity.MovementDirection);
                movement = Quaternion.Euler(0, 0, directionAngle) * movement;
                movableEntity.MovementDirection = Transform2D.V3toV2(movement);

            }
        }
    }

    /// <summary>
    /// Method responsible for rotating and changing movement of the exiting entity according to the type of ramp and the previous
    /// direction of the entity.
    /// </summary>
    /// <param name="other">Entity to modify.</param>
    void OnTriggerExit(Collider other)
    {
        if ((other.tag == "Enemy" || other.tag == "Character") && Mathf.Floor(other.transform.position.z) == Mathf.Floor(transform.position.z))
        {
            MovableEntity movableEntity = other.GetComponent<MovableEntity>();
            float directionAngle = 0.0f;
            float rotationAngle = 0.0f;
            if (gameObject.tag == "RampLeft")
            {
                if (movableEntity.MovementDirection.x > 0)
                {
                    directionAngle = -20;
                    rotationAngle = -20;
                }
                else
                {
                    directionAngle = -20;
                    rotationAngle = 20;
                }
            }
            else
            {
                if (movableEntity.MovementDirection.x > 0)
                {
                    directionAngle = 20;
                    rotationAngle = 20;
                }
                else
                {
                    directionAngle = 20;
                    rotationAngle = -20;
                }
            }
            if (directionAngle != 0.0f)
            {
                movableEntity.transform.Rotate(Vector3.forward, rotationAngle);
                Vector3 movement = Transform2D.V2toV3(movableEntity.MovementDirection);
                movement = Quaternion.Euler(0, 0, directionAngle) * movement;
                movableEntity.MovementDirection = Transform2D.V3toV2(movement);
            }
        }
    }
}
