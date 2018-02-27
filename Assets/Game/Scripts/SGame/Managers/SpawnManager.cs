using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using SGame.Entities.Common;
using SGame.Entities.Characters;
using Utils;
using Map;

/// <summary>
/// Manager in charge of spawning Entities over the map. For enemies, it uses the PoolManager class 
/// in order to avoid On The Fly Allocation. It also uses MapManager to get random spawn positions in the map.
/// Also, with the finality of avoiding overlaping of entities moving in the same pathway, each row of the map is 
/// stored along with a direction, a certain speed and a spawn position in one of the extremes of the map.
/// When the Entities are spawned, a random row is chosen. After that, the Entity takes the stored direction and speed for the
/// movement, and it spawns in the corresponding spawn position. The class RowData contains the whole information.
/// <seealso cref="PoolManager"/>
/// <seealso cref="MapManager.GetRandomSpawnPosition"/>
/// <seealso cref="RowData"/>
/// </summary>
public class SpawnManager : GenericManager<SpawnManager> {

    #region Private variables

    private Dictionary<int, RowData> _rowData;
    private List<MovableEntity> _enemiesToRearrange;
    private int _mapRows;
    private PoolManager _poolManager;
    private List<MovableEntity> _availableCharacters;

    #endregion

    #region SerializeFields

    [SerializeField]private int initialPool = 15;
    [SerializeField]private GameObject enemyGenerator;
    [SerializeField]private GameObject gemGenerator;
    [SerializeField]private GameObject entityFolder;
    [SerializeField]private MovableEntity[] characters;

    #endregion

    #region Properties
    public int TotalCharacters
    {
        get { return characters.Length; }
    }

    public int RemainingCharacters
    {
        get { return _availableCharacters.Count; }
    }
    #endregion

    #region Event functions


    /// <summary>
    /// Initializes every variable needed. It fills the dictionary _rowData with information 
    /// for each row: direction, speed and spawn position. This spawn position is provided by MapManager 
    /// It also stores every character in the _availableCharacters list.
    /// <seealso cref="MapManager.GetRandomSpawnPosition"/>
    /// </summary>
    void Start()
    {
        _poolManager = PoolManager.SINGLETON;
        _poolManager.InitPool(initialPool, enemyGenerator, entityFolder);
        _mapRows = MapManager.SINGLETON.MapRows;
        _enemiesToRearrange = new List<MovableEntity>();
        _availableCharacters = new List<MovableEntity>();
        _rowData = new Dictionary<int, RowData>(_mapRows);
        for (int i = 0; i < _mapRows; ++i)
        {
            _rowData.Add(i, new RowData());
            float rand = Random.Range(GameManager.SINGLETON.SpeedMin, GameManager.SINGLETON.SpeedMax);
            _rowData[i].AssignedSpeed = rand;
            _rowData[i].SpawnPosition = MapManager.SINGLETON.GetRandomSpawnPosition(i);
        }
        for (int i = 0; i < characters.Length; ++i)
            _availableCharacters.Add(characters[i]);
    }

    /// <summary>
    /// Once per frame, if there is any enemy to arrange in the _enemiesToRearrange list,
    /// tries to do it. If succeed, the enemy is rearranged and removed from the list.
    /// </summary>
    void Update()
    {
        if (!GameManager.SINGLETON.PausedGame)
        {
            if (_enemiesToRearrange.Count > 0)
            {
                foreach (MovableEntity enemy in _enemiesToRearrange.ToList())
                {
                    if (TryToRearrangeEnemy(enemy))
                    {
                        _enemiesToRearrange.Remove(enemy);
                    }
                }
            }
        }
    }

    #endregion

    #region Public methods



    /// <summary>
    /// Using the GameObject gem provided by gemGenerator, the method activates it and
    /// locates it in the middle of a random row.
    /// </summary>
    public void SpawnGem()
    {
        int randSpawnPoint = Random.Range(0, _mapRows);

        Vector3 position = _rowData[randSpawnPoint].SpawnPosition;
        position.x = 0;
        position.y += 1.0f;
        position.z = 1;
        gemGenerator.transform.position = position;
        gemGenerator.transform.rotation = Quaternion.identity;
        gemGenerator.transform.parent = entityFolder.transform;
        gemGenerator.SetActive(true);
    }

    /// <summary>
    /// Method in charge of generating enemies. A random row is chosen to spawn an enemy.
    /// For that, it casts a ray where the enemy is supposed to be generated. If the ray
    /// doesn't collide with any other entity, the enemy is taken from the pool if possible and 
    /// spawned in the corresponding position; adding also a new Movement created according to the
    /// direction and speed of the chosen row indicated in _rowData.
    /// If the ray collides with something, the method returns false.
    /// <seealso cref="RowData"/>
    /// </summary>
    /// <returns>True if the enemy has been generated. False otherwise.</returns>
    public bool GenerateEnemy()
    {
        int randSpawnPoint = Random.Range(0, _mapRows);

        Vector3 position = _rowData[randSpawnPoint].SpawnPosition - new Vector3(0, 0, 0.3f);

        Vector3 rayOrigin = position;
        Vector3 rayDirection = Vector3.forward;
        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 15))
        {
            Quaternion rotation = Quaternion.Euler(0, position.x > 0 ? -180 : 0, 0);

            GameObject enemy = _poolManager.TakeEnemy();

            if (enemy == null)
                enemy = Instantiate(enemyGenerator);

            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.transform.parent = entityFolder.transform;

            MovableEntity newEnemy = enemy.GetComponent<MovableEntity>();
            Vector2 movement = _rowData[randSpawnPoint].SpawnPosition.x > 0 ? new Vector2(-1.0f, 0.0f) : new Vector2(1.0f, 0.0f);

            newEnemy.AssignMovement(movement, _rowData[randSpawnPoint].AssignedSpeed);
            newEnemy.SetActive(true);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Method in charge of spawning Characters. Characters are always followed and preceded by enemies,
    /// so they also need to be generated.
    /// For that, it casts a ray where the enemy is supposed to be generated. If the ray
    /// doesn't collide with any other entity, the proceeding enemy is taken from the pool and generated.
    /// Then, the character is generated in a position with a different X. Finally, the second enemy is also generated.
    /// If the ray collides with something, the method returns false.
    /// <seealso cref="RowData"/>
    /// </summary>
    /// <returns>True if the group has been generated. False otherwise.</returns>
    public bool SpawnCharacter()
    {
        int randChar = Random.Range(0, RemainingCharacters);
        int randSpawnPoint = Random.Range(0, _mapRows);


        MovableEntity character = _availableCharacters[randChar];
        _availableCharacters.Remove(character);

        MovableEntity[]  enemies = new MovableEntity[2];

        Vector3 position = _rowData[randSpawnPoint].SpawnPosition - new Vector3(0, 0, 0.3f);
        Vector3 rayOrigin = position;
        Vector3 rayDirection = Vector3.forward;
        Ray ray = new Ray(rayOrigin, rayDirection);
        if (!Physics.Raycast(ray, 15))
        {

            Quaternion rotation = Quaternion.Euler(0, position.x > 0 ? -180 : 0, 0);
            Vector2 movement = _rowData[randSpawnPoint].SpawnPosition.x > 0 ? new Vector2(-1.0f, 0.0f) : new Vector2(1.0f, 0.0f);

            GameObject enemy = _poolManager.TakeEnemy();

            if (enemy == null)
                enemy = (GameObject)Instantiate(enemyGenerator);

            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            MovableEntity newEnemy = enemy.GetComponent<MovableEntity>();

            newEnemy.name = enemyGenerator.tag + entityFolder.transform.childCount;
            newEnemy.transform.parent = entityFolder.transform;
            newEnemy.AssignMovement(movement, _rowData[randSpawnPoint].AssignedSpeed);
            newEnemy.SetActive(true);
            position.x += newEnemy.SpriteData.bounds.size.x * (movement.x * -1);
            enemies[0] = newEnemy;

            character.GetComponent<CharController>().AssignEnemy(newEnemy);

            character.transform.position = position;
            character.transform.rotation = rotation;
            character.AssignMovement(movement, _rowData[randSpawnPoint].AssignedSpeed);
            character.SetActive(true);
            position.x += character.SpriteData.bounds.size.x * (movement.x * -1);

            enemy = _poolManager.TakeEnemy();
            if (enemy == null)
                enemy = (GameObject)Instantiate(enemyGenerator);

            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            newEnemy = enemy.GetComponent<MovableEntity>();
            newEnemy.name = enemyGenerator.tag + entityFolder.transform.childCount;
            newEnemy.transform.parent = entityFolder.transform;
            newEnemy.AssignMovement(movement, _rowData[randSpawnPoint].AssignedSpeed);
            enemies[1] = newEnemy;
            character.GetComponent<CharController>().AssignEnemy(newEnemy);
            newEnemy.SetActive(true);

            return true;
        }
        return false;
    }


    /// <summary>
    /// Adds an Enemy to the _enemiesToRearrange list.
    /// In Update method, this class will try to relocate it.
    /// </summary>
    /// <param name="enemy">Enemy to add.</param>
    public void RearrangeEnemy(GameObject enemy)
    {
        _enemiesToRearrange.Add(enemy.GetComponent<MovableEntity>());
    }
    
    /// <summary>
    /// Adds a killed Enemy to the pool.
    /// </summary>
    /// <param name="enemy">Enemy to add.</param>
    public void AddToPool(GameObject enemy)
    {
        _poolManager.AddEnemy(enemy);
    }


    /// <summary>
    /// Adds a Character to the availableCharacter list;
    /// </summary>
    /// <param name="character">Character to add.</param>
    public void AddToAvailableCharacters(GameObject character)
    {
        character.GetComponent<CharController>().RemoveEnemies();
       _availableCharacters.Add(character.GetComponent<MovableEntity>());
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Chooses a random row with its corresponding information. 
    /// Then casts a ray where the Enemy is supposed to be relocated. If the ray
    /// doesn't collide with any other entity, the Enemy is relocated and removed from the _enemyToRearrange list.
    /// </summary>
    /// <param name="enemy">Enemy to rearrange</param>
    /// <returns>True if the Enemy is relocated. False otherwise.</returns>
    private bool TryToRearrangeEnemy(MovableEntity enemy)
    {
        int randSpawnPoint = (int)Random.Range(0, _mapRows);        
        Vector3 position = _rowData[randSpawnPoint].SpawnPosition - new Vector3(0, 0, 0.3f); ;
        Vector3 rayOrigin = position - Vector3.back;
        Vector3 rayDirection = Vector3.forward;
        Ray ray = new Ray(rayOrigin, rayDirection);

        if(!Physics.Raycast(ray,15))
        {
            Quaternion rotation = Quaternion.Euler(0, position.x > 0 ? -180 : 0, 0);
            Vector2 movement = _rowData[randSpawnPoint].SpawnPosition.x > 0 ? new Vector2(-1.0f, 0.0f) : new Vector2(1.0f, 0.0f);

            float xAdjust = _rowData[randSpawnPoint].SpawnPosition.x > 0 ? enemy.SpriteData.bounds.size.x : -enemy.SpriteData.bounds.size.x;
            enemy.transform.position = position + new Vector3(xAdjust, 0, 0); 


            enemy.transform.rotation = rotation;
            enemy.AssignMovement(movement, _rowData[randSpawnPoint].AssignedSpeed);

            return true;
        }

        return false;
    }

    #endregion

}
