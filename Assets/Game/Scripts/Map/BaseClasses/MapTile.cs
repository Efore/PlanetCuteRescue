using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Map
{
    /// <summary>
    /// Class responsible for representing the position of a block Sprite in the matrix created by MapManager.
    /// <seealso cref="Block"/>
    /// <see cref="MapManager"/>
    /// </summary>
    public class MapTile {

        #region Private variables


        //Block storage
        private Vector3 _topBlockPosition;
        private const float VERTICAL_DIST_TILE = 0.415f;

        #endregion

        #region Properties

        public Vector3 TopBlockPosition
        {
            get { return _topBlockPosition; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// It also is in charge of instantiating the corresponding Sprite in the world position corresponding to the position
        /// in the matrix.
        /// After instantiating a block Sprite, it calls its Block component to assign its required shadows.
        /// <seealso cref="BlockType"/>
        /// <seealso cref="Tuple{T1, T2}"/>
        /// </summary>
        /// <param name="blockPosition">World position base which will be used as reference to instantiate block Sprite.</param>
        /// <param name="matrixCoord">Position of the current tile in the matrix, used only to naming the instantiated block Sprite.</param>
        /// <param name="altitude">Number of blocks to storage in the current tile and the type of the top one. From 0 to 2</param>
        /// <param name="mapBlocks">Dictionary with the actual block Sprite to instantiate corresponding to each type of block.</param>
        /// <param name="mapFolder">GameObject folder under which instantiate the block Sprite.</param>
        /// <param name="waterBlock">Block Sprite with water.</param>
        /// <param name="shapeMap">Matrix with all altitudes, used by storage's top Block Component to assign shadows.</param>
        public MapTile(Vector3 blockPosition, Vector2 matrixCoord, Tuple<int, BlockType> altitude,
            Dictionary<BlockType, GameObject> mapBlocks, GameObject mapFolder, GameObject waterBlock, Tuple<int, BlockType>[][] shapeMap)
        {
            //First we create a base of two sprites representing the ground level.
           
            GameObject newBlock = Instanciator.InstantiateGameObject(mapBlocks[BlockType.Base], blockPosition, mapBlocks[BlockType.Base].transform.rotation);

            //First base sprite
            newBlock.name = matrixCoord.x + "-" + matrixCoord.y + "-0";
            newBlock.transform.parent = mapFolder.transform;
            newBlock.SetActive(true);

            // VERTICAL_DIST_TILE is used to locate following block Sprites in higher positions in order to simulate
            // block storage.
            blockPosition.y += VERTICAL_DIST_TILE;

            // Z world position must be also closer to the camera for the higher blocks to conceal partially the lower ones.
            blockPosition.z -= 0.1f;

            //For the second base sprite, the type of the altitude's top block is checked. In case that it is water, 
            // a water Sprite is instantiated          
            if (altitude.Second == BlockType.Water)
            {
                newBlock = Instanciator.InstantiateGameObject(waterBlock, blockPosition, waterBlock.transform.rotation);
            }
            else
                newBlock = Instanciator.InstantiateGameObject(mapBlocks[BlockType.Base], blockPosition, mapBlocks[BlockType.Base].transform.rotation);

            newBlock.name = matrixCoord.x + "-" + matrixCoord.y + "-1";
            newBlock.transform.parent = mapFolder.transform;
            newBlock.SetActive(true);

            //Finally, the class instantiates block Sprites until reaching the top one, whose type is defined by altitude.Second
            for (int i = 0; i < altitude.First; ++i)
            {
                blockPosition.y += VERTICAL_DIST_TILE;
                blockPosition.z -= 0.1f;

                //Between the ground and the top there must be always blocks of Floor type.
                if (i < altitude.First - 1)
                    newBlock = Instanciator.InstantiateGameObject(mapBlocks[BlockType.Floor], blockPosition, mapBlocks[BlockType.Floor].transform.rotation);
                else
                    newBlock = Instanciator.InstantiateGameObject(mapBlocks[altitude.Second], blockPosition, mapBlocks[altitude.Second].transform.rotation);

                newBlock.transform.parent = mapFolder.transform;
                newBlock.name = matrixCoord.x + "-" + matrixCoord.y + "-" + (2 + i);
                newBlock.SetActive(true);                
            }

            //The method GenerateShadows is called to assign every needed shadow Sprite.
            newBlock.GetComponent<Block>().GenerateShadows(shapeMap, shapeMap.Length, shapeMap[(int)matrixCoord.x].Length, (int)matrixCoord.x, (int)matrixCoord.y);

            //The position of the top block is storage for future uses.
            _topBlockPosition = newBlock.transform.position;
        }

        #endregion

    }
}