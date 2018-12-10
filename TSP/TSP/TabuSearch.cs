using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    public class TabuSearch
    {
        private readonly int _cadency;
        private readonly double _aspiration;
        private readonly int _seconds;

        private bool _continueRunning = true;

        public Solution CurrentSolution { get; set; } = new Solution();
        public Solution BestSolution { get; set; } = new Solution();

        public List<TabuElement> TabuList = new List<TabuElement>();


        public TabuSearch(int cadency, double aspiration, int seconds, int[] initialPath)
        {
            _cadency = cadency;
            _aspiration = aspiration;
            _seconds = seconds;

            CurrentSolution.Path = new List<int>(initialPath).ToArray();
            CurrentSolution.Distance = Helper.CalculateDistance(initialPath).Item1;
            CurrentSolution.LastChange = new TabuElement() { Cadency = 0, To = 0, From = 0 };

            BestSolution.Path = new List<int>(initialPath).ToArray();
            BestSolution.Distance = Helper.CalculateDistance(initialPath).Item1;
            BestSolution.LastChange = new TabuElement {Cadency = 0, To = 0, From = 0}; 
        }


        private void GenerateNeighbors()
        {

            List<Solution> neighborSolutions = new List<Solution>();

            for (int i = 0; i < CurrentSolution.Path.Length; i++)
            {
                for (int j = i + 1; j < CurrentSolution.Path.Length; j++)
                {
                    int[] pathBeforeSwap = new List<int>(CurrentSolution.Path).ToArray();
                    Helper.Swap(CurrentSolution.Path, i, j);
                    neighborSolutions.Add(new Solution
                    {
                        Distance = Helper.CalculateDistance(CurrentSolution.Path).Item1,
                        Path = new List<int>(CurrentSolution.Path).ToArray(),
                        LastChange = new TabuElement
                        {
                            Cadency = 0,
                            From = pathBeforeSwap[i],
                            To = pathBeforeSwap[j]
                        }
                    });
                    Helper.Swap(CurrentSolution.Path, i, j); // Swap back
                }
            }

            MakeMove(neighborSolutions);
            UpdateTabuList();
        }

        private void MakeMove(List<Solution> neighborSolutions)
        {
            //int i = 0;

            var sortedNeighbors = neighborSolutions.OrderBy(n => n.Distance).ToList();

            for (int i = 0; i < sortedNeighbors.Count; i++)
            {
                //if (!IsOnTabuList(sortedNeighbors[i].LastChange) && sortedNeighbors[i].Distance <= CurrentSolution.Distance)
                if (!IsOnTabuList(sortedNeighbors[i].LastChange) || ((double)sortedNeighbors[i].Distance / (double)BestSolution.Distance) < _aspiration)
                {
                    CurrentSolution.Distance = sortedNeighbors[i].Distance;
                    CurrentSolution.Path = new List<int>(sortedNeighbors[i].Path).ToArray();
                    CurrentSolution.LastChange = sortedNeighbors[i].LastChange;

                    CurrentSolution.LastChange.Cadency = _cadency;
                    TabuList.Add(CurrentSolution.LastChange);


                    if (CurrentSolution.Distance < BestSolution.Distance)
                    {
                        BestSolution.Distance = CurrentSolution.Distance;
                        BestSolution.Path = new List<int>(CurrentSolution.Path).ToArray();
                        BestSolution.LastChange = CurrentSolution.LastChange;

                        Console.WriteLine(BestSolution.Distance);
                    }

                    return;
                }
            }

            _continueRunning = false;

        }


        public void Run()
        {
            while (_continueRunning)
            {
                GenerateNeighbors();
                //Console.WriteLine(BestSolution.Distance);
            }
        }


        private bool IsOnTabuList(TabuElement tabuElement)
        {
            foreach (var element in TabuList)
            {
                if (element.From == tabuElement.From && element.To == tabuElement.To)
                    return true;

                if (element.From == tabuElement.To && element.To == tabuElement.From)
                    return true;
            }
            return false;
        }

        private void UpdateTabuList()
        {
            foreach (var tabuElement in TabuList)
                tabuElement.Cadency--;


            TabuList.RemoveAll(t => t.Cadency == 0);

        }

    }
}