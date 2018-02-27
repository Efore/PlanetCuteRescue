using UnityEngine;
using System.Collections;

namespace SGame.Entities.Common.Utils
{
    /// <summary>
    /// Component that is able to create a single bounce and triggers an event when it finishes. It inherits from EntityAnimation.
    /// <seealso cref="EntityAnimation"/>
    /// </summary>
    public class AnimationBounce : EntityAnimation
    {

        #region Private variables

        private bool _singleBouncing = false;
        private float _bouncingStartingY = 0.0f;
        private float _timeForBounce = 2.0f;

        #endregion

        #region Serialize fields

        [SerializeField]private float bouncingStep = 10.0f;
        [SerializeField]private float bouncingRangeY = 0.1f;

        #endregion

        public event System.Action FinishSingleJump;

        #region Event functions

        /// <summary>
        /// It initializes the FinishSingleJump event with a function to be called when the jump is finished.
        /// </summary>
        public override void Start()
        {
            base.Start();
            FinishSingleJump += new System.Action(StopSingleBounce);
        }
        
        /// <summary>
        /// Called once per frame. It bounces the owner GameObject according to a Sin function to simulate a better oscilation.
        /// </summary>
        void Update()
        {
            if (_singleBouncing && !_isPaused)
            {
                Vector3 nextPos = transform.position + new Vector3(0, Mathf.Sin(_timeForBounce * bouncingStep) * bouncingRangeY, 0);
                if (nextPos.y - 0.05f < _bouncingStartingY)
                {
                    nextPos.y = _bouncingStartingY;
                    FinishSingleJump();
                    _timeForBounce = 0.0f;
                }
                transform.position = nextPos;
                _timeForBounce += Time.deltaTime;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Method that prepares everything needed for the bounce.
        /// </summary>
        /// <param name="position">Current position of the GameObject.</param>
        public void StartSingleBounce(Vector3 position)
        {
            _singleBouncing = true;
            _bouncingStartingY = position.y;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Method to stop the bounce.
        /// </summary>
        private void StopSingleBounce()
        {
            _singleBouncing = false;
        }

        #endregion
    }
}
