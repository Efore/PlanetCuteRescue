using UnityEngine;
using System.Collections;
using SGame.Entities.Common.Utils;

namespace SGame.Entities.Other
{
    /// <summary>
    /// Component in charge of controlling Gem's behaviour and handling touching events. It inherits from TouchHandler
    /// <seealso cref="TouchHandler"/>
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(AnimationWalk))]
    public class GemController : TouchHandler
    {
        #region Private variables

        private float _acumTime;
        private bool _dissapear;

        #endregion

        #region Event functions

        void OnEnable()
        {
            InputManager.SINGLETON.OnTouchCollider += HandleOnTouchCollider;
            _acumTime = 0.0f;
            _dissapear = false;
        }

        /// <summary>
        /// Once touched, the component waits 2 seconds before deactivating the gem.
        /// </summary>
        void Update()
        {
            if (_dissapear)
            {
                _acumTime += Time.deltaTime;
                if (_acumTime > 2)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        void OnDisable()
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Method responsible for handling any touch over the gem. 
        /// If it occurs, positive score is added and the gem renderer is deactivated.
        /// <seealso cref="GameManager.AddScore"/>
        /// </summary>
        /// <param name="obj">Object collided by the touch.</param>
        public override void HandleOnTouchCollider(RaycastHit obj)
        {
            if (obj.collider.transform == transform)
            {
                gameObject.GetComponent<AudioSource>().Play();
                gameObject.GetComponent<ParticleSystem>().Emit(30);
                GameManager.SINGLETON.AddScore(100, transform.position);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<AnimationWalk>().enabled = false;
                _dissapear = true;
                InputManager.SINGLETON.OnTouchCollider -= HandleOnTouchCollider;
            }
        }

        #endregion


    }
}

