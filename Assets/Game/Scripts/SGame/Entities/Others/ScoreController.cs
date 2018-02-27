using UnityEngine;
using System.Collections;

namespace SGame.Entities.Other
{

    /// <summary>
    /// This Component manages the behaviour of the floating numbers after adding or substracting score.
    /// <seealso cref="GameManager.AddScore"/>
    /// </summary>
    public class ScoreController : MonoBehaviour {

        #region Private variables

        private Vector3 _destination;
        private SpriteRenderer _spriteData;

        #endregion

        #region Serialize fields

        [SerializeField]private float moveDistance = 1.0f;
        [SerializeField]private float moveSpeed = 10.0f;

        #endregion

        #region Event funtions
        

        /// <summary>
        /// Once enabled, the component assign the destination position for the floating number's movement.
        /// </summary>
        void OnEnable () {
            _spriteData = gameObject.GetComponent<SpriteRenderer>();
            _destination = transform.position + new Vector3(0, moveDistance, 0);
	    }
	
	    
	    /// <summary>
        /// Every frame the number moves and adds some alpha to its renderSprite color in order to create a fading effect.
        /// </summary>
        void Update () {
            transform.position = Vector3.MoveTowards(transform.position, _destination, moveSpeed);
            _spriteData.color = new Color(_spriteData.color.r, _spriteData.color.g, _spriteData.color.b, _spriteData.color.a - (moveSpeed * 0.5f));
            if (transform.position == _destination)
                Destroy(gameObject);
	    }

        #endregion

    }

}