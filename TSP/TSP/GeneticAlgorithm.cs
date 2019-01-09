using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace TSP
{
    public class GeneticAlgorithm
    {
        #region Fields

        private readonly Timer _timer;
        private bool _continueRunning;

        // Number of solutions in population list
        private readonly int _populationSize;
        private readonly double _mutationChance;
        private readonly double _crossoverChance;

        private List<Solution> _population = new List<Solution>();
        private readonly List<Solution> _nextPopulation = new List<Solution>();

        private int _firstParent = int.MaxValue;
        private int _secondParent = int.MaxValue;

        #endregion

        #region Constructors

        public GeneticAlgorithm(int populationSize, double mutationChance, double crossoverChance, double time)
        {
            _populationSize = populationSize;
            _mutationChance = mutationChance;
            _crossoverChance = crossoverChance;

            _timer = new Timer(time * 1000);
            _timer.Elapsed += TimerTick;


            InitializePopulation();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Choosing parents for crossing method
        /// </summary>
        private void ChooseParents()
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            // list of tuples containing a solution and a probability 
            var weightedPopulation = new List<Tuple<Solution, double>>();

            // assign probabilities to solutions - the worse solution - the lower chance of being picked
            for (int i = 0; i < _populationSize; i++)
            {
                double probability = 1f / Math.Pow(2, i);
                weightedPopulation.Add(new Tuple<Solution, double>(_population[i], probability));
            }

            double roll = rnd.NextDouble();

            for (int i = _populationSize - 1; i >= 0; i--)
                if (weightedPopulation[i].Item2 >= roll)
                {
                    _firstParent = i;
                    break;
                }

            while (_secondParent == int.MaxValue)
            {
                roll = rnd.NextDouble();
                for (int i = _populationSize - 1; i >= 0; i--)
                    if (weightedPopulation[i].Item2 >= roll && i != _firstParent)
                    {
                        _secondParent = i;
                        break;
                    }
            }
            //Console.WriteLine($"First selected: {Population[firstFound].Distance}");
            //Console.WriteLine($"Second selected: {Population[secondFound].Distance}");

            Cross();
        }

        /// <summary>
        /// Method handling the cross operation
        /// </summary>
        private void Cross()
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            int initializingSolution = rnd.Next(_population.Count);

            var childSolution = new Solution
            {
                Distance = _population[initializingSolution].Distance,
                Path = new List<int>(_population[initializingSolution].Path).ToArray(),
            };

            if (rnd.NextDouble() <= _crossoverChance)
            {
                // random how long will the be replacement segment from first parent
                int lengthOfRandomlyChosenSegment = rnd.Next(_population[_firstParent].Path.Length);

                // random the starting index which we will start from our replacement (simplified, so we don't have like 7 elements to replace while being started on last element)
                int firstElementStartingIndex;
                do
                {
                    firstElementStartingIndex = rnd.Next(_population[_firstParent].Path.Length);
                }
                while ((firstElementStartingIndex + lengthOfRandomlyChosenSegment >= _population[_firstParent].Path.Length));

                // create new path which we will assign to child and initialize it with int.maxvalue
                int[] newPath = new int[Program.TotalCities - 1];
                for (int i = 0; i < newPath.Length; i++) newPath[i] = int.MaxValue;

                // inject elements from first parent to it's child
                for (int i = firstElementStartingIndex; i < lengthOfRandomlyChosenSegment; i++)
                    newPath[i] = _population[_firstParent].Path[i];

                // now we need to know where to start filling the rest
                int secondElementStartingIndex = firstElementStartingIndex + lengthOfRandomlyChosenSegment;

                // helper variables for the loop
                int childIndex = secondElementStartingIndex, parentIndex = secondElementStartingIndex;

                while (newPath.Contains(int.MaxValue))
                {
                    // if we reached the end of array, reset the indexes
                    if (childIndex == newPath.Length) childIndex = 0;
                    if (parentIndex == newPath.Length) parentIndex = 0;

                    // if child with this index is our initial value
                    if (newPath[childIndex] == int.MaxValue)
                    {
                        // if our child contains a value of second parent on this index
                        if (newPath.Contains(_population[_secondParent].Path[parentIndex]))
                        {
                            parentIndex++;
                        }
                        else
                        {
                            // assign from second parent to child
                            newPath[childIndex] = _population[_secondParent].Path[parentIndex];
                            childIndex++;
                            parentIndex++;
                        }
                    }
                    else
                    {
                        childIndex++;
                    }
                }

                // create new tuple to hold information about our child's distance and path
                Tuple<int, string> childPath = Helper.CalculateDistance(newPath);

                // create child's solution object
                childSolution = new Solution
                {
                    Path = newPath,
                    Distance = childPath.Item1
                };
            }

            Mutate(childSolution);
            //Console.WriteLine($"distance: {childSolution.Distance}");
            //Console.WriteLine($"path: {childPath.Item2}");

            _nextPopulation.Add(childSolution);
        }

        /// <summary>
        /// Method handling the mutation
        /// </summary>
        /// <param name="childSolution"></param>
        private void Mutate(Solution childSolution)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            if (rnd.NextDouble() <= _mutationChance)
            {
                int firstCity, secondCity;
                firstCity = rnd.Next(childSolution.Path.Length);

                do
                {
                    secondCity = rnd.Next(childSolution.Path.Length);
                }
                while (firstCity == secondCity);

                Helper.Swap(childSolution.Path, firstCity, secondCity);
            }
        }

        private void InitializePopulation()
        {
            // fill the list with default routes [ 0, 1, 2, 3, (...) ]
            for (int i = 0; i < _populationSize; i++)
            {
                _population.Add(new Solution { Path = Helper.FillRoadArray() });
                Helper.Shuffle(new Random(Guid.NewGuid().GetHashCode()), _population[i].Path);
                _population[i].Distance = Helper.CalculateDistance(_population[i].Path).Item1;
            }

            // order to make it more comfortable to assign probabilities later on
            var orderedPopulation = _population.OrderBy(p => p.Distance);
            _population = orderedPopulation.ToList();
        }

        /// <summary>
        /// Method generating population
        /// </summary>
        private void GeneratePopulation()
        {
            for (int i = 0; i < _populationSize; i++)
                ChooseParents();
        }

        /// <summary>
        /// Method that runs the algorithm, sets up a timer and monitor if we achieved the stopping criteria
        /// </summary>
        public void Run()
        {
            _timer.Start();
            _continueRunning = true;

            int generationCounter = 0;
            int bestDistance = int.MaxValue;

            while (_continueRunning)
            {
                _nextPopulation.Clear();
                GeneratePopulation();
                _population = new List<Solution>(_nextPopulation);

                var orderedPopulation = _population.OrderBy(p => p.Distance);
                _population = orderedPopulation.ToList();

                if (_population[0].Distance < bestDistance)
                {
                    bestDistance = _population[0].Distance;
                    Console.WriteLine($"{generationCounter} generation found new best solution, {_population[0].Distance}");
                }

                generationCounter++;
            }

            Console.WriteLine($"Best distance: {bestDistance}");
        }

        #endregion

        private void TimerTick(object obj, ElapsedEventArgs e) => _continueRunning = false;
    }
}
