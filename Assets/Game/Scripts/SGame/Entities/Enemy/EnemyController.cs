using UnityEngine;
using System.Collections;
using SGame.Entities.Common.Utils;
using SGame.Entities.Common;
using System;

namespace SGame.Entities.Enemy
{

    /// <summary>
    /// Component in charge of controlling Enemies' behaviour and handling touching events. It inherits from TouchHandler
    /// <seealso cref="TouchHandler"/>
    /// </summary>
    [RequireComponent(typeof(AnimationWalk))]
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(MovableEntity))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(AudioSource))]
    public class EnemyController : TouchHandler
    {
        #region Serialize fields

        [SerializeField]private Sprite aliveSprite;

        #endregion

        #region Event fuctions

        /// <summary>
        /// Inicialiation of an enemy after being actived with everything that is necessary.
        /// </summary>
        void OnEnable()
        {
            _isDying = false;
            gameObject.tag = "Enemy";
            gameObject.transform.position -= new Vector3(0, 0, 0.1f);
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            gameObject.GetComponent<AnimationWalk>().enabled = true;
            owner.SpriteData.enabled = true;
            owner.SpriteData.sprite = aliveSprite;
            ((MovableEntity)owner).enabled = true;
        }

        /// <summary>
        /// Called once every frame.
        /// If the character is dying, it makes the sprite blick for a short time.
        /// </summary>
        void Update()
        {
            if (_isDying)
            {
                _secsAcum += Time.deltaTime;
                InvokeRepeating("Blink", 0, 0.1f);
                
                if (_secsAcum > timeForDie)
                {
                    _secsAcum = 0.0f;
                    gameObject.SetActive(false);
                    GameManager.SINGLETON.AddToPool(gameObject);
                    CancelInvoke("Blink");
                }
            }
        }

        #endregion

        #region Public methods


        /// <summary>
        /// Method responsible for handling any touch over the enemy.
        /// If it occurs, it prepares everything for the death, as well as calls the AddScore method with positive score.
        /// <seealso cref="GameManager.AddScore"/>
        /// </summary>
        /// <param name="obj">Object collided by the touch.</param>
        public override void HandleOnTouchCollider(RaycastHit obj)
        {
            if (obj.collider.transform == transform)
            {
                _isDying = true;

                GameManager.SINGLETON.AddScore(30, transform.position);
                GameManager.SINGLETON.EnemyDead(((MovableEntity)owner));

                gameObject.GetComponent<AudioSource>().Play();
                gameObject.GetComponent<ParticleSystem>().Emit(20);
                owner.SpriteData.sprite = owner.DeadSprite;                
                
                ((MovableEntity)owner).enabled = false;                
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
                gameObject.GetComponent<AnimationWalk>().enabled = false;
                gameObject.tag = "DeadEnemy";
                gameObject.transform.position += new Vector3(0, 0, 0.1f);                
            }
        }

        #endregion
    }
}
