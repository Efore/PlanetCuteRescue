using UnityEngine;
using System.Collections.Generic;
using SGame.Entities.Common;
using Utils;

/// <summary>
/// Class to keep a record of the generated enemies. In order to avoid On The Fly Allocation, every killed enemy 
/// will be storaged in this class. When spawning a new enemy, the SpawnManager will try to get it from here first. 
/// If the list is empty, the SpawnManager will isntantiate a new gameobject.
/// </summary>
public class PoolManager
{
    #region Private variables

    private List<GameObject> _enemyPool;

    #endregion

    #region Singleton declaration

    private PoolManager() { }

    private static PoolManager _instance = null;

    public static PoolManager SINGLETON
    {
        get
        {
            if (_instance == null)
                _instance = new PoolManager();

            return _instance;
        }
    }

    #endregion

    #region Properties

    public int EnemyPoolCount
    {
        get { return _enemyPool.Count; }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// It initializes the pool instantiating as many clones of "generator" as indicated in "initCapacity", under the
    /// gameObject "folder".
    /// </summary>
    /// <param name="initCapacity">Initial number of clones.</param>
    /// <param name="generator">Original GameObject from which clones will be generated.</param>
    /// <param name="folder">GameObject folder where the clones will be located.</param>
    public void InitPool(int initCapacity, GameObject generator, GameObject folder)
    {
        _enemyPool = new List<GameObject>(initCapacity);
        for(int i = 0; i < initCapacity; ++i)
        {
            GameObject newEnemy = Instanciator.InstantiateGameObject(generator, Vector3.zero, Quaternion.Euler(0, 0, 0));
            newEnemy.transform.parent = folder.transform;
            newEnemy.name = "Enemy" + folder.transform.childCount;
            _enemyPool.Add(newEnemy);            
        }
    }

    /// <summary>
    /// Method to add an enemy to the pool, if is not already in it.
    /// </summary>
    /// <param name="enemy">Enemy to add.</param>
    public void AddEnemy(GameObject enemy)
    {
        if(!_enemyPool .Contains(enemy))
            _enemyPool.Add(enemy);
    }


    /// <summary>
    /// Method to take an enemy from the pool. If the pool is not empty, it revoke the enemy from the pool.
    /// </summary>
    /// <returns>If the pool is not empty, returns an Enemy. Otherwise, returns null.</returns>
    public GameObject TakeEnemy()
    {
        GameObject enemy;
        if (_enemyPool.Count > 0)
        {
            enemy = _enemyPool[_enemyPool.Count - 1];
            _enemyPool.Remove(enemy);
        }
        else
            enemy = null;

        return enemy;
    }

    #endregion

}
