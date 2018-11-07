using System;

namespace TSP
{
    public static class Helper
    {
        #region Methods

        /// <summary>
        /// Function that exchanges 2 elements of an array
        /// </summary>
        /// <param name="path"></param>
        /// <param name="el1"></param>
        /// <param name="el2"></param>
        public static void Swap(int[] path, int el1, int el2)
        {
            var tmp = path[el1];
            path[el1] = path[el2];
            path[el2] = tmp;
        }

        /// <summary>
        /// Function calculating the distance for specific path
        /// </summary>
        /// <param name="path">Road array</param>
        /// <returns>Returns tuple containing distance(int) and path(string)</returns>
        public static Tuple<int, string> CalculateDistance(int[] path)
        {
            int distance = 0, tmpdist = 0;
            string strPath = $"0 -> {path[0]}";

            //dist from 0 -> X
            distance += Program.CitiesArray[0, path[0]];

            for (int i = 0; i < path.Length-1; i++)
            {
                distance += Program.CitiesArray[path[i], path[i+1]];
                strPath += $" -> {path[i + 1]}";
            }

            //dist from Z -> 0
            strPath += $" -> 0";
            distance += Program.CitiesArray[path[path.Length-1], 0];

            return new Tuple<int, string>(distance, strPath);
        }

        #endregion
    }
}
