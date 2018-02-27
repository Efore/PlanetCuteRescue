using UnityEngine;
using SGame.Entities.Common.Utils;
using SGame.Entities.Common;

namespace SGame.Entities.Characters
{
    /// <summary>
    /// Component in charge of controlling Characters' behaviour and handling touching events. It inherits from TouchHandler
    /// <seealso cref="TouchHandler"/>
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(AnimationBounce))]
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(MovableEntity))]
    [RequireComponent(typeof(AudioSource))]
    public class CharController : TouchHandler
    {

        #region Private variables

        private Vector3 speechBubblePos = new Vector3(0.68f, 0.45f, -0.01f);
        private Vector3 _slotTarget;

        private MovableEntity _enemy1 = null;
        private MovableEntity _enemy2 = null;

        private int _remainingEnemies = 0;
        private bool _movingToSlot = false;

        #endregion

        #region Serialize fields

        [SerializeField]private GameObject particleManager;
        [SerializeField]private float moveToSlotSpeed;
        [SerializeField]private AudioClip splashSound;
        [SerializeField]private GameObject speechBubble;

        #endregion

        #region Event functions

        /// <summary>
        /// Inicialiation of a character after being actived with everything that is necessary.
        /// Function RemoveEnemy is added to the GameManager's EnemyDeadEvent as listener.
        /// Function BreakingFree is added to the AnimationBounce's FinishSingleJump as listener.
        /// SpeechBubble is established as the character's child to control its position.
        /// <seealso cref=" GameManager.EnemyDeadEvent"/>
        /// <seealso cref="AnimationBounce.FinishSingleJump"/>
        /// </summary>
        void OnEnable()
        {
            _secsAcum = 0.0f;
            GameManager.SINGLETON.EnemyDeadEvent += RemoveEnemy;
            owner.GetComponent<AnimationBounce>().FinishSingleJump += BreakingFree;
            speechBubble.SetActive(true);
            speechBubble.transform.parent = transform;
            speechBubble.transform.localPosition = speechBubblePos;
            if(speechBubble.transform.position.x < transform.position.x)
            {
                speechBubble.transform.localPosition = new Vector3(speechBubblePos.x * -1, speechBubblePos.y, speechBubblePos.z);
            }
        }
             
        /// <summary>
        /// Called once every frame. If the character is movingToSlot, it continues that movement.
        /// If the character is dying, it makes the sprite blick for a short time.
        /// </summary>
        void Update()
        {
            if(_movingToSlot)
            {
                float step = moveToSlotSpeed * Time.deltaTime;
                gameObject.transform.position = Vector3.MoveTowards(transform.position, _slotTarget, step);
                moveToSlotSpeed += 0.5f;
                if (Vector3.Distance(transform.position, _slotTarget) < 0.0001f)
                {
                    GameManager.SINGLETON.SettleCharacter((MovableEntity)owner, true);                    
                }
            }
            else if (_isDying)
            {
                _secsAcum += Time.deltaTime;
                InvokeRepeating("Blink", 0, 0.4f);
                if (_secsAcum > timeForDie)
                {
                    gameObject.SetActive(false);
                    CancelInvoke("Blink");
                    GameManager.SINGLETON.SettleCharacter((MovableEntity)owner, false);
                }
            }

        }

        /// <summary>
        /// Called just before turning the character as deactivated.
       /// It removes RemoveEnemy as listener.
       /// SpeechBubble is remove from the character's childs and is deactivated.
        /// </summary>
        void OnDisable()
        {
            if (speechBubble.activeInHierarchy)
            {
                speechBubble.transform.parent = transform.parent;
                speechBubble.SetActive(false);
            }
            GameManager.SINGLETON.EnemyDeadEvent -= RemoveEnemy;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Method to assign an escorting enemy to the character when spawning. Used by SpawnManager.
        /// <seealso cref="SpawnManager.SpawnCharacter"/>
        /// </summary>
        /// <param name="enemy">Enemy to assign.</param>
        public void AssignEnemy(MovableEntity enemy)
        {
            if (_enemy1 == null)
                _enemy1 = enemy;
            else
                _enemy2 = enemy;

            _remainingEnemies++;
        }


        /// <summary>
        /// Method in charge of removing one of the escorting enemies. If there is no more escorting enemies, it deactives
        /// anything unnecessary and checks if the character is over water, in which case it will die. Otherwise, the character is freed, 
        /// the score is added and the bouncing animation is launched. It is called when any EnemyDeadEvent is launched.
        /// <seealso cref="GameManager.EnemyDeadEvent"/>
        /// <seealso cref="GameManager.AddScore"/>
        /// </summary>
        /// <param name="enemy"></param>
        public void RemoveEnemy(MovableEntity enemy)
        {
            if(enemy == _enemy1)
            {
                _enemy1 = null;
                _remainingEnemies--;
            }
            else if (enemy == _enemy2)
            {
                _enemy2 = null;
                _remainingEnemies--;
            }

            if(_remainingEnemies == 0)
            {
                ((MovableEntity)owner).MovementDirection = Vector3.zero;
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
                ((MovableEntity)owner).IsActive = false;
                GameManager.SINGLETON.EnemyDeadEvent -= RemoveEnemy;
                speechBubble.SetActive(false);

                if (CheckIsOverWater())
                {
                    gameObject.GetComponent<AudioSource>().PlayOneShot(splashSound);
                    Die();
                }   
                else
                {
                    GameManager.SINGLETON.AddScore(100, transform.position);
                    gameObject.GetComponent<AudioSource>().Play();
                    owner.GetComponent<AnimationBounce>().StartSingleBounce(transform.position);
                }    
            }
        }

        /// <summary>
        /// Method in charge of revoking every escorting enemy. It is called by SpawnManager when the character is not 
        /// freed or killed in its way over the map.
        /// <seealso cref="SpawnManager.AddToAvailableCharacters"/>
        /// </summary>
        public void RemoveEnemies()
        {
            _enemy1 = null;
            _enemy2 = null;
            _remainingEnemies = 0;
        }


        /// <summary>
        /// Method responsible for handling any touch over the character. If it occurs, the character dies.
        /// </summary>
        /// <param name="obj">Object collided by the touch.</param>
        public override void HandleOnTouchCollider(RaycastHit obj)
        {
            if (obj.collider.transform == transform)
            {
                gameObject.GetComponent<ParticleSystem>().Emit(12);
                Die();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Method to prepare everything for the death. It also calls the AddScore method with negative score.
        /// <seealso cref="GameManager.AddScore"/>
        /// </summary>
        private void Die()
        {
            GameManager.SINGLETON.AddScore(-100, gameObject.transform.position);
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            ((MovableEntity)owner).enabled = false;
            _isDying = true;
            GameManager.SINGLETON.EnemyDeadEvent -= RemoveEnemy;
        }

        /// <summary>
        /// Method that cast a ray where the character has been freed and returns whether it collides with a Water Block.
        /// </summary>
        /// <returns>True if the character is over water.</returns>
        private bool CheckIsOverWater()
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position,Vector3.forward, out hit,1))
            {
                if (hit.collider.tag == "WaterBlock")
                {
                    hit.collider.GetComponent<ParticleSystem>().Emit(30);
                    return true;
                }
                    
            }

            return false;
        }

        /// <summary>
        /// Method to set everything needed by the character to move towards the upper right corner.
        /// </summary>
        private void BreakingFree()
        {            
            particleManager.SetActive(true);
            _movingToSlot = true;
            _slotTarget = GameManager.SINGLETON.LastCharacterSlotPosition;
        }

        #endregion
    }
}
