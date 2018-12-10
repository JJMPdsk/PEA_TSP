using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace TSP
{
    public class TabuSearch
    {
        #region Fields

        private readonly int _cadency;
        private readonly double _aspiration;
        private readonly Timer _timer;

        private bool _continueRunning = true;
        private List<Solution> _sortedNeighbors = new List<Solution>();

        #endregion

        #region Properties

        public Solution CurrentSolution { get; set; } = new Solution();
        public Solution BestSolution { get; set; } = new Solution();
        public List<TabuElement> TabuList { get; set; } = new List<TabuElement>();

        #endregion

        #region Constructors

        public TabuSearch(int cadency, double aspiration, int seconds, int[] initialPath)
        {
            _cadency = cadency;
            _aspiration = aspiration;

            CurrentSolution.Path = new List<int>(initialPath).ToArray();
            CurrentSolution.Distance = Helper.CalculateDistance(initialPath).Item1;
            CurrentSolution.LastChange = new TabuElement() { Cadency = 0, To = 0, From = 0 };

            BestSolution.Path = new List<int>(initialPath).ToArray();
            BestSolution.Distance = Helper.CalculateDistance(initialPath).Item1;
            BestSolution.LastChange = new TabuElement { Cadency = 0, To = 0, From = 0 };


            _timer = new Timer(seconds * 1000);
            _timer.Elapsed += TimerTick;
        }

        #endregion

        #region Main methods

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
            _sortedNeighbors = new List<Solution>(neighborSolutions.OrderBy(n => n.Distance).ToList());
            MakeMove();
            UpdateTabuList();
        }

        private void MakeMove()
        {
            for (int i = 0; i < _sortedNeighbors.Count; i++)
            {
                if (!IsOnTabuList(_sortedNeighbors[i].LastChange) || ((double)_sortedNeighbors[i].Distance / (double)BestSolution.Distance) < _aspiration)
                {
                    CurrentSolution.Distance = _sortedNeighbors[i].Distance;
                    CurrentSolution.Path = new List<int>(_sortedNeighbors[i].Path).ToArray();
                    CurrentSolution.LastChange = _sortedNeighbors[i].LastChange;

                    CurrentSolution.LastChange.Cadency = _cadency;
                    TabuList.Add(CurrentSolution.LastChange);

                    _sortedNeighbors.RemoveAt(i);

                    if (CurrentSolution.Distance < BestSolution.Distance)
                    {
                        BestSolution.Distance = CurrentSolution.Distance;
                        BestSolution.Path = new List<int>(CurrentSolution.Path).ToArray();
                        BestSolution.LastChange = CurrentSolution.LastChange;
                    }

                    return;
                }
            }
            _continueRunning = false;
        }

        #endregion

        #region Helper methods

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

        private void TimerTick(Object obj, ElapsedEventArgs e)
        {
            _continueRunning = false;
        }

        #endregion

        public void Run()
        {
            _timer.Start();

            while (_continueRunning)
            {
                GenerateNeighbors();
            }
        }
    }
}