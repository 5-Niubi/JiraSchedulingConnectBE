namespace RcpspAlgorithmLibrary.GA
{
    internal class GeneticAlgorithm
    {
        public Population Evolve(Population population, Data data)
        {
            return MutatePopulation(CrossoverrPopulation(population, data), data);
        }
        private Population CrossoverrPopulation(Population population, Data data)
        {
            Population crossoverPopulation = new Population(population.Chromosomes.Length);
            for (int e = 0; e < GAHelper.NUM_OF_ELITE_CHOMOSOMES; ++e)
            {
                crossoverPopulation.Chromosomes[e] = population.Chromosomes[e];
            }
            for (int x = GAHelper.NUM_OF_ELITE_CHOMOSOMES; x < population.Chromosomes.Length; ++x)
            {
                Chromosome chromosome1 = SelectTournamentPopulation(population, data).Chromosomes[0];
                Chromosome chromosome2 = SelectTournamentPopulation(population, data).Chromosomes[0];
                crossoverPopulation.Chromosomes[x] = CrossoverChromosome(chromosome1, chromosome2, data);
            }
            return crossoverPopulation;
        }
        private Population MutatePopulation(Population population, Data data)
        {
            Population mutatePopulation = new Population(population.Chromosomes.Length);
            for (int e = 0; e < GAHelper.NUM_OF_ELITE_CHOMOSOMES; ++e)
            {
                mutatePopulation.Chromosomes[e] = population.Chromosomes[e];
            }
            for (int e = GAHelper.NUM_OF_ELITE_CHOMOSOMES; e < population.Chromosomes.Length; ++e)
            {

                mutatePopulation.Chromosomes[e] = MutateChromosome(population.Chromosomes[e], data);


            }
            return mutatePopulation;
        }

        private Chromosome CrossoverChromosome(Chromosome chromosome1, Chromosome chromosome2, Data data)
        {
            Random rand = new Random();
            Chromosome crossChromosome = new Chromosome(data.NumOfTasks);
            for (int e = 0; e < chromosome1.Genes.Length; ++e)
            {
                if (rand.NextDouble() < 0.5) crossChromosome.Genes[e] = chromosome1.Genes[e];
                else crossChromosome.Genes[e] = chromosome2.Genes[e];
            }
            return crossChromosome;
        }

        private Chromosome MutateChromosome(Chromosome chromosome, Data data)
        {
            Random rand = new Random();
            Chromosome mutateChromosome = new Chromosome(data.NumOfTasks);
            for (int x = 0; x < chromosome.Genes.Length; ++x)
            {
                if (rand.NextDouble() < GAHelper.MUTATION_RATE)
                {
                    int z = data.SuitableWorkers.ElementAt(x).Count;
                    int c = (int)(rand.NextDouble() * z);
                    mutateChromosome.Genes[x] = data.SuitableWorkers.ElementAt(x).ElementAt(c);
                }
                else mutateChromosome.Genes[x] = chromosome.Genes[x];
            }
            return mutateChromosome;
        }

        private Population SelectTournamentPopulation(Population population, Data data)
        {
            Random rand = new Random();
            Population tournamentPopulation = new Population(GAHelper.TOURNAMET_SELECTION_SIZE);
            for (int x = 0; x < GAHelper.TOURNAMET_SELECTION_SIZE; ++x)
            {
                int c = (int)(rand.NextDouble() * population.Chromosomes.Length);
                tournamentPopulation.Chromosomes[x] = population.Chromosomes[c];
            }
            tournamentPopulation.SortChromosomesByFitness(data);
            return tournamentPopulation;
        }
    }
}