using UnityEngine;
using System.Collections.Generic;


namespace Utils
{
    /// <summary>
    /// Enum to keep a record of the different kind of blocks.
    /// </summary>
    public enum BlockType : int { Base = 0, Floor = 1, RightRamp = 2, LeftRamp = 3, Water = 4};


    /// <summary>
    /// Enum to keep a record of the different terrain themes for the map generation.
    /// </summary>
    public enum TerrainTheme : int { BlockGrass = 0, BlockStone = 1, Random = 2};

    #region Class Tuple

    /// <summary>
    /// Class to contain two values of different class
    /// </summary>
    /// <typeparam name="T1">Class of the first value</typeparam>
    /// <typeparam name="T2">Class of the second value</typeparam>
    public class Tuple<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }

    #endregion

    #region Class RowData

    /// <summary>
    /// Class to contain the information of a row in the SpawnManager
    /// </summary>
    public class RowData
    {
        public RowData()
        { }

        public float AssignedSpeed
        {
            get;
            set;
        }

        public Vector3 SpawnPosition
        {
            get;
            set;
        }

    }

    #endregion

    #region Class Transform2D

    /// <summary>
    /// Static class to do quick conversions between 2D and 3D positions
    /// </summary>
    public static class Transform2D
    {

        public static Vector2 V3toV2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public static Vector3 V2toV3(Vector2 v2, float z = 0f)
        {
            return new Vector3(v2.x, v2.y, z);
        }
    }

    #endregion

    #region Class Instanciator

    /// <summary>
    /// Class to instantiate gameObjects. Used by classes which do not ihnerit from MonoBehaviour
    /// </summary>
    public class Instanciator : MonoBehaviour
    {
        public static GameObject InstantiateGameObject(GameObject original, Vector3 position, Quaternion rotation)
        {
            return (GameObject)Instantiate(original, position, rotation);
        }
    }

    #endregion

    #region Class GameData

    /// <summary>
    /// Static class to save the score and the level during a game.
    /// </summary>
    public static class GameData
    {

        public static int score = 0;
        public static int level = 0;

    }

    #endregion
}
