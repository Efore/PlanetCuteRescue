using UnityEngine;
using SGame.Entities.Common;
using Utils;
using System.Collections.Generic;

/// <summary>
/// Master class of the game. Developed as singleton, it controls everthing related to the state of the game, by itself
/// or by using other managers. 
/// Is the bridge between game elements and the rest of managers.
/// In charge of: Keeping records of current score and level, keeping record of the movement speed of entities according to the
/// current level, paying background musics and communicating with GUIController and SpawnManager.
/// <seealso cref="SpawnManager"/>
/// <seealso cref="GUIController"/>
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GameManager : GenericManager<GameManager> {

    #region Private variables

    private float _timeSinceLastSpawn;
    private float _timeSinceLastCharacter;
    private bool _pausedGame;
    private int _savedCharacters;
    private int _deadCharacters;
    private int _score;
    private SpawnManager _spawnManager;
    private int _level;
    private int _killingSpree;

    #endregion

    #region Serialize fields

    [SerializeField]private float speedMin = 1.5f;
    [SerializeField]private float speedMax = 0.8f;
    [SerializeField]private int remainingBullets = 20;
    [SerializeField]private float timeBetweenSpawn = 2.0f;
    [SerializeField]private float timeBetweenCharacter = 2.0f;
    [SerializeField]private AudioClip shotSound;
    [SerializeField]private AudioClip defeatMusic;
    [SerializeField]private AudioClip victoryMusic;
    [SerializeField]private AudioClip perfectVictoryMusic;
    [SerializeField]private GameObject[] characterSlotPositions;
    [SerializeField]private GUIController guiController;
    [SerializeField]private GameObject scores;
    [SerializeField]private int maxKillingSpree = 10;

    #endregion

    public event System.Action<MovableEntity> EnemyDeadEvent;
    public event System.Action<bool> PauseEvent;

    #region Properties

    public float SpeedMax
    {
        get { return speedMax; }
    }

    public float SpeedMin
    {
        get { return speedMin; }
    }

    public int RemainingBullets
    {
        get { return remainingBullets; }
    }

    public bool PausedGame
    {
        get { return _pausedGame; }
    }

    public Vector3 LastCharacterSlotPosition
    {
        get { return characterSlotPositions[_savedCharacters + _deadCharacters].transform.position; }
    }

    #endregion

    #region Event functions

    /// <summary>
    /// Initialization of every needed variable. Score and level are got from GameData
    /// <seealso cref="GameData"/>
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        _savedCharacters = 0;
        _deadCharacters = 0;
                
        _timeSinceLastCharacter = 0.0f;
        _timeSinceLastSpawn = 0.0f;     
        _pausedGame = false;
        Time.timeScale = 1;
        _score = GameData.score;
        _level = GameData.level;
        _killingSpree = 0;
        speedMax += 0.3f * _level;
        speedMin += 0.3f * _level;
        timeBetweenSpawn -= 0.1f * _level;        
    }

    public override void OnEnable()
    {
        base.OnEnable();
        EnemyDeadEvent += new System.Action<MovableEntity>(AddScore);
        PauseEvent += new System.Action<bool>(SetPause);        
        guiController.SetScore(_score);
    }

    void Start()
    {
        _spawnManager = SpawnManager.SINGLETON ;
    }

    /// <summary>
    /// If the game is not paused, it decides when to spawn Enemies or Characters according to a provided time for each action.
    /// </summary>
    void Update()
    {
        if (!_pausedGame)
        {
            _timeSinceLastSpawn += Time.deltaTime;

            if (_spawnManager.RemainingCharacters > 0)
                _timeSinceLastCharacter += Time.deltaTime;

            if (_timeSinceLastSpawn > timeBetweenSpawn)
            {
                if( _spawnManager.GenerateEnemy())
                    _timeSinceLastSpawn = 0.0f;
            }
            if(_timeSinceLastCharacter > timeBetweenCharacter)
            {
                if(_spawnManager.SpawnCharacter())
                 _timeSinceLastCharacter = 0.0f;
            }

        }


    }

    void OnDisable()
    {
        EnemyDeadEvent -= AddScore;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Method to trigger EnemyDeadEvent, used by EnemyController
    /// </summary>
    /// <param name="enemy">Dead enemy</param>
    public void EnemyDead(MovableEntity enemy)
    {
        EnemyDeadEvent(enemy);
    }

    /// <summary>
    /// Method to add a deactivated enemy to the Enemy pool.
    /// <seealso cref="SpawnManager.AddToPool"/>
    /// </summary>
    /// <param name="enemy">Enemy to add</param>
    public void AddToPool(GameObject enemy)
    {
        _spawnManager.AddToPool(enemy);
    }
 

    /// <summary>
    /// Method to trigger PauseEvent.
    /// </summary>
    /// <param name="pause">Indicates if it pauses or resumes the game</param>
    public void Pause(bool pause)
    {
        PauseEvent(pause);
    }

    /// <summary>
    /// Method to pause the game, setting the timeScale to 0. Added to PauseEvent event.
    /// </summary>
    /// <param name="pause">Indicates if it pauses or resumes the game</param>
    public void SetPause(bool pause)
    {
        _pausedGame = pause;
        if (pause)
        {
            Time.timeScale = 0;
        }
        else
            Time.timeScale = 1;
    }

    /// <summary>
    /// Saves current score and level in GameData and reload the current scene.
    /// <seealso cref="GameData"/>
    /// </summary>
    public void NextLevel()
    {
        GameData.level++;
        GameData.score = _score;
        Application.LoadLevel("Game");
    }

    /// <summary>
    /// It restarts the game.
    /// </summary>
    public void Restart()
    {
        GameData.level = 0;
        GameData.score = 0;
        Application.LoadLevel("Game");
    }

    /// <summary>
    /// Pauses the game, play defeat music and actives every GUI DefeatScreen element.
    /// <seealso cref="GUIController.ActiveDefeatScreen"/>
    /// </summary>
    public void LoadDefeatScene()
    {
        PauseEvent(true);
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.clip = defeatMusic;
        audioSource.loop = false;
        audioSource.Play();

        guiController.ActiveDefeatScreen();
    }

    /// <summary>
    /// Pauses the game, play victory music and actives every GUI VictoryScreen element.
    /// <seealso cref="GUIController.ActiveVictoryScreen"/>
    /// </summary>
    public void LoadVictoryScene()
    {
        PauseEvent(true);
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.clip = victoryMusic;
        audioSource.loop = false;
        audioSource.Play();

        guiController.ActiveVictoryScreen();
    }

    /// <summary>
    /// Pauses the game, play perfect victory music and actives every GUI PerfectVictoryScreen element.
    /// <seealso cref="GUIController.ActivePerfectVictoryScreen"/>
    /// </summary>
    public void LoadPerfectScene()
    {
        PauseEvent(true);
        
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.clip = perfectVictoryMusic;
        audioSource.loop = false;
        audioSource.Play();

        guiController.ActivePerfectVictoryScreen();
    }


    /// <summary>
    /// Substracts one bullet and plays shotSound.
    /// If there is no more bullets left, the game is over.
    /// </summary>
    public void UseBullet()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(shotSound);
        remainingBullets--;
        guiController.SetAmmo(remainingBullets);

        if(remainingBullets == 0)
        {
            CheckGameOver();
        }
    }

    /// <summary>
    /// It adds a positive or negative number to the current score (-100,30 or 100) and
    /// instantiates a its sprite from the position given. 
    /// </summary>
    /// <param name="add">Score to add</param>
    /// <param name="position">Position where the floating number will be instantiated</param>
    public void AddScore(int add, Vector3 position)
    {
        _score += add;
        if (_score < 0)
            _score = 0;

        guiController.SetScore(_score);

        GameObject scoreSprite = (GameObject)Instantiate(scores.transform.FindChild(add.ToString()).gameObject, position, Quaternion.identity);
        scoreSprite.SetActive(true);
        scoreSprite.transform.parent = scores.transform;
    }


    /// <summary>
    /// Method in charge of setting the state of a character after it has been killed or freed.
    /// It also calls GUIController.AssignCharacter to show the character in the top right corner.
    /// When the number of deadCharacters plus the number of aliveCharacters is equal to the number of total characters,
    /// the game is over.
    /// <seealso cref="GUIController.AssignCharacter"/>
    /// </summary>
    /// <param name="character">Character to set.</param>
    /// <param name="alive">Indicates if the character has been freed or killed.</param>
    public void SettleCharacter(MovableEntity character, bool alive)
    {
             
        if (alive)
        {
            _savedCharacters++;
        }
        else
        {
            _deadCharacters++;            
        }
        character.SetActive(false);
        guiController.AssignCharacter(character, alive);

        if(_savedCharacters + _deadCharacters == _spawnManager.TotalCharacters)
            CheckGameOver();
    }

    /// <summary>
    /// Called by lateral walls to relocate an enemy that has surpassed them.
    /// It uses SpawnManager.RearrangeEnemy
    /// <seealso cref="SpawnManager.RearrangeEnemy"/>
    /// </summary>
    /// <param name="enemy">Enemy to relocate</param>
    public void RearrangeEnemy(GameObject enemy)
    {
        _spawnManager.RearrangeEnemy(enemy);
    }

    /// <summary>
    /// Called by lateral walls to relocate an Character that has surpassed them.
    /// It is added to the SpawnManager.AvailableCharacter in order to be considerated when 
    /// spawning a new character.
    /// <seealso cref="SpawnManager.AddToAvailableCharacters"/>
    /// <seealso cref="SpawnManager._availableCharacters"/>
    /// </summary>
    /// <param name="character">Character to add.</param>
    public void AddToAvailableCharacters(GameObject character)
    {
        _spawnManager.AddToAvailableCharacters(character);
    }

    /// <summary>
    /// It counts the consecutive enemy kills without falling a shot.
    /// When it reach the limit provided, a gem is spawned in the middle of the screen.
    /// <seealso cref="SpawnManager.SpawnGem"/>
    /// </summary>
    /// <param name="kill">Indicates if is an aimed or a missed shot</param>
    public void AddKillingSpree(bool kill)
    {
        if(kill)
        {
            _killingSpree++;
            if(_killingSpree == maxKillingSpree)
            {
                _killingSpree = 0;
                _spawnManager.SpawnGem();
            }
        }
        else
        {
            _killingSpree = 0;
        }
    }

    #endregion

    #region Private methods

    private void AddScore(MovableEntity enemy)
    {        
        guiController.SetScore(_score);
    }

    /// <summary>
    /// Called when the game is over. It checks the difference between killed characters and saved ones,
    /// and active its correpsonding screen.
    /// Also, if the current score is higher than the highest one so far, it saves it as the new highest score.
    /// </summary>
    private void CheckGameOver()
    {

        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", _score);
        else if (PlayerPrefs.GetInt("HighScore") < _score)
            PlayerPrefs.SetInt("HighScore", _score);

        PlayerPrefs.Save();

        if (_savedCharacters == _spawnManager.TotalCharacters)
            LoadPerfectScene();
        else if (_savedCharacters > _deadCharacters)
            LoadVictoryScene();
        else
            LoadDefeatScene();

    }

    #endregion
}
