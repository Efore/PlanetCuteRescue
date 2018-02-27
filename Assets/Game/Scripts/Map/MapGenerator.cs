using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Map
{

    /// <summary>
    /// Static class in charge of generating an altitude matrix, which defines the shape of the map by assigning to each MapTile 
    /// a number of blocks to instantiate and the type of the top one.
    /// <seealso cref="GenerateMap"/>
    /// <seealso cref="MapTile"/>
    /// </summary>
    public static class MapGenerator
    {
        #region Private variables

        //The minimum amount of consecutive visible tiles
        private const int MIN_PATHWAY_VISIBLE = 3;

        //Visibility matrix used to check how visible is every pathway
        private static bool[][] _visibilityMatrix;

        #endregion

        #region Public methods

        /// <summary>
        /// Method in charge of generating a matrix of altitudes. Each altitude data contains the number of block Sprites to 
        /// storage in the same MapTile and the block type of the top one. It is called by MapManager.
        /// <seealso cref="MapManager"/>
        /// </summary>
        /// <param name="rows">Number of rows of the matrix.</param>
        /// <param name="cols">Number of cols of the matrix.</param>
        /// <returns>Matrix result after all the requirements are matched.</returns>
        public static Tuple<int, BlockType>[][] GenerateMap(int rows, int cols)
        {
            //Both return and visibilityMatrix are initialized.
            Tuple<int, BlockType>[][] matrix = new Tuple<int, BlockType>[rows][];
            _visibilityMatrix = new bool[rows - 1][];

            for (int i = 0; i < rows; ++i)
            {
                matrix[i] = new Tuple<int, BlockType>[cols];

                if (i < rows - 1)
                    _visibilityMatrix[i] = new bool[cols];

                for (int j = 0; j < cols; ++j)
                {
                    matrix[i][j] = new Tuple<int, BlockType>(1, BlockType.Floor);
                    if (i < rows - 1)
                        _visibilityMatrix[i][j] = true;
                }
            }

            //There is a little chance that the generator creates a matrix that is not valid. If so, tries again.
            while (!ShapeMap(ref matrix, rows, cols)) ;

            return matrix;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// It creates the basic altitude matrix, limited by the requirements especified in the test project PDF.
        /// After having it created, it assign ramps and, if possible, rivers.
        /// </summary>
        /// <param name="matrix">Matrix to fill with altitude data.</param>
        /// <param name="rows">Number of rows of the matrix.</param>
        /// <param name="cols">Number of colums of the matrix.</param>
        /// <returns>True if a valid matrix has been generated. False otherwise.</returns>
        private static bool ShapeMap(ref Tuple<int, BlockType>[][] matrix, int rows, int cols)
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    //There are three possible altitudes: 0, 1 and 2. Which are the number of blocks that will
                    //be stored over the ground level.
                    List<int> possibilities = new List<int>(3);
                    possibilities.Add(0);
                    possibilities.Add(1);
                    possibilities.Add(2);
                    
                    //The method checks which are the altitudes left after having into account the current state of the matrix
                    //along with the No-Steping policy
                    SteppingCheck(matrix, i, j, ref possibilities);

                    //If there is no more possible altitudes, return false
                    if (possibilities.Count == 0)
                        return false;

                    //The method checks which are the altitudes left after having into account the current state of the matrix
                    //along with the "At least 3 consecutive visible tiles" policy
                    VisibilityCheck(matrix, cols, i, j, ref possibilities);

                    int value;

                    if (possibilities.Count == 0)
                        return false;
                    else
                    {
                        //If there is still possible altitudes left, the method chooses one randomly
                        value = possibilities[Random.Range(0, possibilities.Count)];
                    }

                    matrix[i][j].First = value;

                    //After the value has been asigned to the matrix, we check if this altitude 
                    //conceals any tile of the previous rows and update the _visibilityMatrix if so.
                    if (i > 0 && value - matrix[i - 1][j].First >= 1)
                        _visibilityMatrix[i - 1][j] = false;

                    if (i > 1 && value - matrix[i - 2][j].First > 1)
                        _visibilityMatrix[i - 2][j] = false;
                }
            }
            AssignBlockType(ref matrix, rows, cols);
            CheckRiver(ref matrix, rows, cols);
            return true;
        }


        /// <summary>
        /// Method in charge of assigning the block type of the altitude's top block.
        /// </summary>
        /// <param name="matrix">The matrix to modify.</param>
        /// <param name="rows">Number of rows of the matrix.</param>
        /// <param name="cols">Number of colums of the matrix.</param>
        private static void AssignBlockType(ref Tuple<int, BlockType>[][] matrix, int rows, int cols)
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (j > 0)
                    {
                        matrix[i][j] = CheckBlockType(matrix, i, j);
                    }
                }
            }
        }

        /// <summary>
        /// Method that returns the corresponding block type.
        /// </summary>
        /// <param name="matrix">The matrix to modify.</param>
        /// <param name="row">Row position of the current element.</param>
        /// <param name="col">Column position of the current element.</param>
        /// <returns>Altitude data with the assigned block type</returns>
        private static Tuple<int, BlockType> CheckBlockType(Tuple<int, BlockType>[][] matrix, int row, int col)
        {
            //Ensuring that the previous block is not already a ramp
            if (matrix[row][col - 1].Second != BlockType.RightRamp && matrix[row][col - 1].Second != BlockType.LeftRamp)
            {
                //If the previous block in this same row is lower than the current one, LeftRamp is assigned
                if (matrix[row][col - 1].First - matrix[row][col].First == -1)
                    return new Tuple<int, BlockType>(matrix[row][col].First, BlockType.LeftRamp);
                //Otherwise, if the previous block is higher, RightRamp is assigned and since it will be need
                //one block more, the altitude is increased.
                else if (matrix[row][col - 1].First - matrix[row][col].First == 1)
                    return new Tuple<int, BlockType>(matrix[row][col].First + 1, BlockType.RightRamp);
            }
            //Otherwise, a Floor type is returned
            return new Tuple<int, BlockType>(matrix[row][col].First, BlockType.Floor); ;
        }

        /// <summary>
        /// Method to check the possibility of having a river in the map according to a simple patron: having an entire column
        /// with the same altitude and altitude 0.
        /// </summary>
        /// <param name="matrix">The matrix to modify.</param>
        /// <param name="rows">Number of rows of the matrix.</param>
        /// <param name="cols">Number of colums of the matrix.</param>
        private static void CheckRiver(ref Tuple<int, BlockType>[][] matrix, int rows, int cols)
        {
            int numRivers = 0;
            for(int j = 0; j < cols && numRivers < 2; ++j)
            {
                if(matrix[0][j].First == 0)
                {
                    bool possibleRiver = true;
                    for(int i = 1; i < rows && possibleRiver; ++i)
                    {
                        if (matrix[i][j].First != 0)
                            possibleRiver = false;
                    }
                    if (possibleRiver)
                    {
                        numRivers++;
                        for (int i = 0; i < rows && possibleRiver; ++i)
                        {
                            matrix[i][j].Second = BlockType.Water;
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Method responsible for discarding possible altitudes if they conceals other pathways and the pathway is already too concealed.
        /// </summary>
        /// <param name="matrix">Altitude matrix with the current state of the map</param>
        /// <param name="cols">Number of columns of the matrix.</param>
        /// <param name="row">Row position of the current element.</param>
        /// <param name="col">Column position of the current element.</param>
        /// <param name="possibilities">Remaining possible altitudes.</param>
        private static void VisibilityCheck(Tuple<int, BlockType>[][] matrix, int cols, int row, int col, ref List<int> possibilities)
        {
            foreach (int i in possibilities.ToList())
            {
                if (row > 0 && i - matrix[row - 1][col].First >= 1 && !VisibilityRow(row - 1, cols))
                {
                    possibilities.Remove(i);
                }
                else if (row > 1 && i - matrix[row - 2][col].First >= 1 && !VisibilityRow(row - 2, cols))
                {
                    possibilities.Remove(i);
                }
            }
        }

        /// <summary>
        /// Method that checks if the pathway is already too concealed.
        /// </summary>
        /// <param name="row">Row position of the pathway</param>
        /// <param name="cols">Number of colums of the visibilityMatrix.</param>
        /// <returns>True if it is too concealed. False otherwise.</returns>
        private static bool VisibilityRow(int row, int cols)
        {
            int visibleTiles = 0;
            for (int i = 0; i < cols; ++i)
            {
                if (_visibilityMatrix[row][i])
                    visibleTiles++;
                else
                    visibleTiles = 0;

                if (visibleTiles > MIN_PATHWAY_VISIBLE)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Method in charge of discard possible altitudes if they created "stepping" according to the current
        /// map state.
        /// </summary>
        /// <param name="matrix">Altitude matrix with the current state of the map.</param>
        /// <param name="row">Row position of the current element.</param>
        /// <param name="col">Column position of the current element.</param>
        /// <param name="possibilities">Remaining possible altitudes.</param>
        private static void SteppingCheck(Tuple<int, BlockType>[][] matrix, int row, int col, ref List<int> possibilities)
        {
            foreach (int i in possibilities.ToList())
            {
                if (col > 0 && Mathf.Abs(matrix[row][col - 1].First - i) == 2)
                    possibilities.Remove(i);
                else if (col > 1 && Mathf.Abs(matrix[row][col - 1].First - i) == 1)
                {
                    if (Mathf.Abs(matrix[row][col - 2].First - i) == 2 || matrix[row][col - 2].First == i)
                        possibilities.Remove(i);
                }
            }
        }

        #endregion

    }
}

