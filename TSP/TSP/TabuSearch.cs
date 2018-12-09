using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    public class TabuSearch
    {
        public List<TabuElement> TabuList = new List<TabuElement>();
        public List<Solution> TopSolutions = new List<Solution>();


        #region Methods

        /// <summary>
        /// Adds new element to tabu list
        /// </summary>
        /// <param name="tabuElement"></param>
        public void AddTabuElement(TabuElement tabuElement)
        {
            TabuList.Add(tabuElement);
        }

        /// <summary>
        ///  Updates the tabu list.
        /// Decreases cadency of each element in tabu list and checks if the some elements shouldn't be removed
        /// </summary>
        public void UpdateTabuList()
        {
            foreach (var tabuElement in TabuList)
            {
                tabuElement.Cadency--;
                if (tabuElement.Cadency == 0)
                    TabuList.Remove(tabuElement);
            }
        }

        public void GenerateMovements(int[] path)
        {
            // List of calculated solutions
            List<Solution> Solutions = new List<Solution>();

            // we calculate each with each
            for (int i = 0; i < path.Length; i++)
            {
                for (int j = i + 1; j < path.Length; j++)
                {
                    Helper.Swap(path, i, j);
                    var tmp = Helper.CalculateDistance(path);
                    Solutions.Add(new Solution{ Distance = tmp.Item1, Path = path});
                    Helper.Swap(path, i, j); // swap back
                }
            }

            TopSolutions = Solutions.OrderByDescending(s => s.Distance).ToList();

        }
        #endregion
    }
}