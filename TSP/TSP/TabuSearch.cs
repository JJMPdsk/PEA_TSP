using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace TSP
{
    public class TabuSearch
    {
        public List<TabuElement> TabuList = new List<TabuElement>();
        public List<Solution> TopSolutions = new List<Solution>();


        // Length in iterations for how long will the move be in tabu list 
        private readonly int _cadency;
        private readonly double _aspiration;
        private readonly int _neighborCap;

        public Solution CurrentSolution { get; set; }
        public Solution BestEver { get; set; }


        private int _bestSolution = Int32.MaxValue;
        public int BestSolution
        {
            get { return _bestSolution; }
            set
            {
                if (value < _bestSolution)
                {
                    _bestSolution = value;
                    Console.WriteLine(value);
                }
            }
        }

        // For controlling the period of program running
        private Timer _timer;

        private bool _continueRunning = true;


        public TabuSearch(int[] path, int cadency, int seconds, double aspiration, int neighborCap)
        {
            CurrentSolution = new Solution() { Distance = Int32.MaxValue, LastChange = new TabuElement(), Path = path };
            BestEver = new Solution() { Distance = Int32.MaxValue, LastChange = new TabuElement(), Path = path };


            _cadency = cadency;
            _aspiration = aspiration;
            _neighborCap = neighborCap;

            _timer = new Timer(seconds * 1000);
            _timer.Elapsed += TimerTick;
        }

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
            }

            TabuList.RemoveAll(t => t.Cadency == 0);

            //if (tabuElement.Cadency == 0)
            //     TabuList.Remove(tabuElement);
        }

        public void GenerateMovements(int[] path)
        {
            // First we need to decide whether we should generate new movements
            if (TopSolutions.Count == 0)
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
                        Solutions.Add(new Solution
                        {
                            Distance = tmp.Item1,
                            Path = path,
                            LastChange = new TabuElement
                            {
                                Cadency = _cadency,
                                From = path[i],
                                To = path[j]
                            }
                        });
                        Helper.Swap(path, i, j); // swap back
                    }
                }
                // We get top solutions found
                TopSolutions = Solutions.OrderBy(s => s.Distance).Take(_neighborCap).ToList();
            }
            // Perform a movement
            MakeMovement();
            //!-----------------------------
            //Console.WriteLine($"tmp dist: {CurrentSolution.Distance}");
            //!-----------------------------
            if (CurrentSolution.Distance < BestEver.Distance)
            {
                BestEver = CurrentSolution;
                BestSolution = CurrentSolution.Distance;
            }
            UpdateTabuList();
        }

        public void MakeMovement()
        {
            // Index of movement we are going to perform
            int i = 0;
            while (i < TopSolutions.Count)
            {
                if (ContainsElement(TabuList, TopSolutions[i].LastChange))
                {
                    if ((double)TopSolutions[i].Distance / (double)CurrentSolution.Distance <= _aspiration)
                    {
                        CurrentSolution = TopSolutions[i];
                        AddTabuElement(TopSolutions[i].LastChange);
                        TopSolutions.RemoveAt(i);
                        return;
                    }
                }
                else
                {
                    CurrentSolution = TopSolutions[i];
                    AddTabuElement(TopSolutions[i].LastChange);
                    TopSolutions.RemoveAt(i);
                    return;
                }
                i++;
            }
            Helper.Shuffle(new Random(), CurrentSolution.Path);
        }


        public void Run()
        {
            _timer.Start();
            while (_continueRunning)
            {
                GenerateMovements(CurrentSolution.Path);
            }

            Console.WriteLine($"Distance: {BestEver.Distance}");
            //Console.Write("Path: ");
            //for (int i = 0; i < CurrentSolution.Path.Length; i++)
            //{
            //    Console.Write(" " + CurrentSolution.Path[i]);
            //}
            //Console.WriteLine();
            //Console.WriteLine();
        }

        private void TimerTick(Object obj, ElapsedEventArgs e)
        {
            _continueRunning = false;
        }


        public bool ContainsElement(List<TabuElement> tabuElements, TabuElement tabuElement)
        {
            foreach (var element in tabuElements)
            {
                if (element.From == tabuElement.From && element.To == tabuElement.To)
                    return true;

                if (element.From == tabuElement.To && element.To == tabuElement.From)
                    return true;
            }
            return false;
        }

        #endregion
    }
}