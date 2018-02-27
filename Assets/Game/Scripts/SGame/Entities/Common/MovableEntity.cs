using UnityEngine;
using System.Collections;
using Utils;
using SGame.Entities.Common.Utils;


namespace SGame.Entities.Common
{    
    /// <summary>
    /// Class that represent every touchable gameObject that is able to move.
    /// It inherits from the class Entity.
    /// </summary>
    public class MovableEntity : Entity
    {

        #region Private variables

        private Movement _movement;

        #endregion

        #region Properties

        public Vector2 MovementDirection
        {
            get { return _movement.MovementDirection; }
            set { _movement.MovementDirection = value; }
        }

        public float Speed
        {
            get { return _movement.Speed; }
            set { _movement.Speed = value; }
        }

        #endregion

        #region Event functions

        public override void Update()
        {
            if (IsActive)
            {
                base.Update();
                Move();
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Method to assign a new movement to the entity. Used by SpawnManager just before spawning the entity.
        /// <seealso cref="SpawnManager.SpawnCharacter"/>
        /// <seealso cref="SpawnManager.GenerateEnemy"/>
        /// </summary>
        /// <param name="movementDirection">Direction of the new movement.</param>
        /// <param name="speed">Speed of the new movement</param>
        public void AssignMovement(Vector2 movementDirection, float speed)
        {
            _movement = new Movement(movementDirection, speed);
        }

        #endregion

        #region Private methods

        private void Move()
        {
            transform.position += Transform2D.V2toV3(MovementDirection) * Speed * Time.deltaTime;
        }

        #endregion
    }
}
