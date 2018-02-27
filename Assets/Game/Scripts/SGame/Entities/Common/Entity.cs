using UnityEngine;
using SGame.Entities.Common.Utils;

namespace SGame.Entities.Common
{
    /// <summary>
    /// Base class for everything that is touchable in the game, apart from the UI elements. 
    /// It contains basic information that is shared for movable and still entities.
    /// </summary>
    [RequireComponent(typeof(TouchHandler))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Entity : MonoBehaviour
    {

        #region Serialize fields
         
        [SerializeField]protected TouchHandler touchHandler;
        [SerializeField]protected SpriteRenderer spriteData;
        [SerializeField]protected Sprite deadSprite;

        #endregion

        #region Properties

        public bool IsActive
        {
            get;
            set;
        }

        public SpriteRenderer SpriteData
        {
            get { return spriteData; }
        }

        public Sprite DeadSprite
        {
            get { return deadSprite; }
        }

        #endregion

        #region Event functions

        public virtual void OnEnable()
        {
            IsActive = true;
            InputManager.SINGLETON.OnTouchCollider += touchHandler.HandleOnTouchCollider;
            GameManager.SINGLETON.PauseEvent += Pause;      
        }

        public virtual void Update()
        {

        }

        public virtual void OnDisable()
        {
            InputManager.SINGLETON.OnTouchCollider -= touchHandler.HandleOnTouchCollider;
        }

        #endregion

        #region Public methods

        public void SetActive(bool set)
        {
            gameObject.SetActive(set);
        }       

        public void Pause(bool pause)
        {
            touchHandler.enabled = !pause;
            IsActive = !pause;
            if(pause)
            {
                InputManager.SINGLETON.OnTouchCollider -= touchHandler.HandleOnTouchCollider;
            }
            else
            {
                InputManager.SINGLETON.OnTouchCollider += touchHandler.HandleOnTouchCollider;
            }
        }

        #endregion
    }
}