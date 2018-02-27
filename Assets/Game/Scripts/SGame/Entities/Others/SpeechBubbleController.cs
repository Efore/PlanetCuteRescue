using UnityEngine;


namespace SGame.Entities.Other
{
    /// <summary>
    /// Component in charge of managing the occasionally pop up of character's speeching bubbles.
    /// </summary>
    public class SpeechBubbleController : MonoBehaviour
    {

        #region Private variables

        private float _acumTimeShowing;
        private float _acumTimePopup;
        private bool _isPoping;
        private bool _isShowing;

        #endregion

        #region Serialize fields

        [SerializeField]private float timeShowing = 1.0f;
        [SerializeField]private float timeToPopup = 2.0f;
        [SerializeField]private float popingSpeed = 1.0f;
        [SerializeField]private Vector3 maxScale;

        #endregion

        #region Event functions

        
        /// <summary>
        /// Initialization of every variable
        /// </summary>
        void OnEnable()
        {
            _acumTimePopup = 0.0f;
            _acumTimeShowing = 0.0f;
            _isShowing = false;
            _isPoping = false;
        }

        
        /// <summary>
        /// If the bubble is not showing, it acumulates time in order to do so.
        /// After timeToPopup is reached, it starts poping by increasing the scale of the gameObject.
        /// Once reached the desired scale, the showing time starts to counts.
        /// After timeShowing is surpassed, it starts to shrink and all begins over again. 
        /// </summary>
        void Update()
        {
            if (!_isShowing)
            {
                _acumTimePopup += Time.deltaTime;
                if (_acumTimePopup > timeToPopup)
                {
                    _acumTimePopup = 0.0f;
                    _isShowing = true;
                    _isPoping = true;
                }
            }
            else
            {
                if (_isPoping)
                {
                    transform.localScale += new Vector3(popingSpeed * Time.deltaTime, popingSpeed * Time.deltaTime, 0);
                    if (transform.localScale.x >= maxScale.x)
                    {
                        transform.localScale = maxScale;
                        _isPoping = false;
                    }
                }
                else
                {
                    _acumTimeShowing += Time.deltaTime;
                    if (_acumTimeShowing > timeShowing)
                    {
                        transform.localScale -= new Vector3(popingSpeed * Time.deltaTime, popingSpeed * Time.deltaTime, 0);
                        if (transform.localScale.x < 0)
                        {
                            _acumTimeShowing = 0.0f;
                            transform.localScale = new Vector3(0, 0, 1);
                            _isShowing = false;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
