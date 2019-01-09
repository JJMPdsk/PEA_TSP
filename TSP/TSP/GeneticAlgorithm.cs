using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    public class GeneticAlgorithm
    {
        // Number of solutions in Population list
        public int PopulationCount { get; set; }
        public int GenerationCount { get; set; }
        public double MutationChance { get; set; }
        public double CrossoverChance { get; set; }

        public List<Solution> Population { get; set; } = new List<Solution>();
        public List<Solution> NextPopulation { get; set; } = new List<Solution>();

        private int _firstParent = Int32.MaxValue;
        private int _secondParent = Int32.MaxValue;


        public GeneticAlgorithm(int populationCount, int generationCount, double mutationChance, double crossoverChance)
        {
            PopulationCount = populationCount;
            GenerationCount = generationCount;
            MutationChance = mutationChance;

            InitializePopulation();
        }

        private void InitializePopulation()
        {
            // fill the list with default routes [ 0, 1, 2, 3, (...) ]
            for (int i = 0; i < PopulationCount; i++)
            {
                Population.Add(new Solution { Path = Helper.FillRoadArray() });
                Helper.Shuffle(new Random(Guid.NewGuid().GetHashCode()), Population[i].Path);
                Population[i].Distance = Helper.CalculateDistance(Population[i].Path).Item1;
            }

            // order to make it more comfortable to assign probabilities later on
            var orderedPopulation = Population.OrderBy(p => p.Distance);
            Population = orderedPopulation.ToList();
        }

        private void GeneratePopulation()
        {
            for (int i = 0; i < PopulationCount; i++)
                ChooseParents();
        }


        private void ChooseParents()
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            // list of tuples containing a solution and a probability 
            var weightedPopulation = new List<Tuple<Solution, double>>();

            // assign probabilities to solutions - the worse solution - the lower chance of being picked
            for (int i = 0; i < PopulationCount; i++)
            {
                double probability = 1f / Math.Pow(2, i);
                weightedPopulation.Add(new Tuple<Solution, double>(Population[i], probability));
            }

            double roll = rnd.NextDouble();

            //foreach (var tuple in weightedPopulation)
            //Console.WriteLine($"solution: {tuple.Item1.Distance}, chance: {tuple.Item2}");

            for (int i = PopulationCount - 1; i >= 0; i--)
                if (weightedPopulation[i].Item2 >= roll)
                {
                    _firstParent = i;
                    break;
                }


            while (_secondParent == Int32.MaxValue)
            {
                roll = rnd.NextDouble();
                for (int i = PopulationCount - 1; i >= 0; i--)
                    if (weightedPopulation[i].Item2 >= roll && i != _firstParent)
                    {
                        _secondParent = i;
                        break;
                    }
            }

            //Console.WriteLine($"First selected: {Population[firstFound].Distance}");
            //Console.WriteLine($"Second selected: {Population[secondFound].Distance}");

            ////////////////////////////////////////////////
            Cross();
            ////////////////////////////////////////////////

        }

        private void Cross()
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            int initializingSolution = rnd.Next(Population.Count);

            var childSolution = new Solution
            {
                Distance = Population[initializingSolution].Distance,
                Path = new List<int>(Population[initializingSolution].Path).ToArray(),
            };


            if (rnd.NextDouble() <= CrossoverChance)
            {
                // random how long will the be replacement segment from first parent
                int lengthOfRandomlyChosenSegment = rnd.Next(Population[_firstParent].Path.Length);

                // random the starting index which we will start from our replacement (simplified, so we don't have like 7 elements to replace while being started on last element)
                int firstElementStartingIndex;
                do
                {
                    firstElementStartingIndex = rnd.Next(Population[_firstParent].Path.Length);
                }
                while ((firstElementStartingIndex + lengthOfRandomlyChosenSegment >= Population[_firstParent].Path.Length));

                // create new path which we will assign to child and initialize it with int32.maxval
                int[] newPath = new int[Program.TotalCities - 1];
                for (int i = 0; i < newPath.Length; i++) newPath[i] = Int32.MaxValue;

                // inject elements from first parent to it's child
                for (int i = firstElementStartingIndex; i < lengthOfRandomlyChosenSegment; i++)
                    newPath[i] = Population[_firstParent].Path[i];

                // now we need to know where to start filling the rest
                int secondElementStartingIndex = firstElementStartingIndex + lengthOfRandomlyChosenSegment;

                // helper variables for the loop
                int childIndex = secondElementStartingIndex, parentIndex = secondElementStartingIndex;

                while (newPath.Contains(Int32.MaxValue))
                {
                    // if we reached the end of array, reset the indexes
                    if (childIndex == newPath.Length) childIndex = 0;
                    if (parentIndex == newPath.Length) parentIndex = 0;

                    // if child with this index is our initial value
                    if (newPath[childIndex] == Int32.MaxValue)
                    {
                        // if our child contains a value of second parent on this index
                        if (newPath.Contains(Population[_secondParent].Path[parentIndex]))
                        {
                            parentIndex++;
                        }
                        else
                        {
                            // assign from second parent to child
                            newPath[childIndex] = Population[_secondParent].Path[parentIndex];
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

            NextPopulation.Add(childSolution);
        }

        private void Mutate(Solution childSolution)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            if (rnd.NextDouble() <= MutationChance)
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

        public void Run()
        {
            int bestDistance = Int32.MaxValue;
            for (int i = 0; i < GenerationCount; i++)
            {
                NextPopulation.Clear();
                GeneratePopulation();
                Population = new List<Solution>(NextPopulation);

                var orderedPopulation = Population.OrderBy(p => p.Distance);
                Population = orderedPopulation.ToList();

                //Console.WriteLine($"{i} generation best distance: {Population[0].Distance}");
                if (Population[0].Distance < bestDistance)
                {
                    bestDistance = Population[0].Distance;
                    Console.WriteLine($"{i} generation found new best solution, {Population[0].Distance}");
                }
            }


            // Console.WriteLine("###########################");
            Console.WriteLine($"Best distance: {bestDistance}");
            //Console.WriteLine("###########################");
        }
    }
}
