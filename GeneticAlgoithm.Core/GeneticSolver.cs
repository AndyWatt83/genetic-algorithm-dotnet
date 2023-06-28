namespace GeneticAlgoithm.Core;
public class GeneticSolver
{
    static Random random = new Random();

    public static void Solve()
    {
        int populationSize = 100;
        int chromosomeLength = 20;
        int maxGenerations = 1000;
        double crossoverRate = 0.8;
        double mutationRate = 0.01;

        // Initialize a random population
        var population = Enumerable.Range(0, populationSize)
            .Select(x => RandomChromosome(chromosomeLength))
            .ToList();

        int generation = 0;
        while (generation < maxGenerations)
        {
            // Evaluate the fitness of each individual
            var fitnesses = population.Select(ChromosomeFitness).ToList();
            Console.WriteLine($"Fittest individual in generation { generation } is { fitnesses.Max() }");

            // Check if we found a solution
            if (fitnesses.Max() == chromosomeLength)
            {
                Console.WriteLine($"Solution found in generation { generation }!");
                break;
            }

            // Select parents and create the next generation
            var newPopulation = Enumerable.Range(0, populationSize / 2)
                .SelectMany(x => {
                    var parent1 = SelectParent(population, fitnesses);
                    var parent2 = SelectParent(population, fitnesses);
                    return Crossover(parent1, parent2, crossoverRate);
                })
                .Select(child => Mutate(child, mutationRate))
                .ToList();

            population = newPopulation;
            generation++;
        }
    }

    static string RandomChromosome(int length)
    {
        return string.Concat(Enumerable.Range(0, length).Select(x => random.Next(2).ToString()));
    }

    static int ChromosomeFitness(string chromosome)
    {
        return chromosome.Count(c => c == '1');
    }

    static string SelectParent(IReadOnlyList<string> population, IReadOnlyList<int> fitnesses)
    {
        int totalFitness = fitnesses.Sum();
        int selection = random.Next(totalFitness);
        int runningTotal = 0;

        for (int i = 0; i < population.Count; i++)
        {
            runningTotal += fitnesses[i];
            if (runningTotal >= selection)
            {
                return population[i];
            }
        }
        return population.Last();
    }

    static string[] Crossover(string parent1, string parent2, double rate)
    {
        if (random.NextDouble() < rate)
        {
            int crossoverPoint = random.Next(1, parent1.Length);
            var child1 = parent1.Substring(0, crossoverPoint) + parent2.Substring(crossoverPoint);
            var child2 = parent2.Substring(0, crossoverPoint) + parent1.Substring(crossoverPoint);
            return new[] { child1, child2 };
        }
        else
        {
            return new[] { parent1, parent2 };
        }
    }

    static string Mutate(string chromosome, double rate)
    {
        return string.Concat(chromosome.Select(c => random.NextDouble() < rate ? Flip(c) : c));
    }

    static char Flip(char bit)
    {
        return bit == '0' ? '1' : '0';
    }
}
