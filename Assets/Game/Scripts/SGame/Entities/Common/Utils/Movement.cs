using UnityEngine;
using System.Collections;

namespace SGame.Entities.Common.Utils
{
    /// <summary>
    /// Simple class used by entities to storage its current movement: direction and speed
    /// </summary>
    public class Movement
    {
        public Movement(Vector2 movementDirection, float speed)
        {
            MovementDirection = movementDirection;
            Speed = speed;
        }

        public Vector2 MovementDirection
        {
            get;
            set;
        }

        public float Speed
        {
            get;
            set;
        }
    }
}
