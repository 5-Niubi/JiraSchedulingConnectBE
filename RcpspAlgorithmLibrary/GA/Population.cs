namespace RcpspAlgorithmLibrary.GA

{
    public class Population
    {
        private Chromosome[] chromosomes = new Chromosome[GAHelper.NUM_OF_POPULATION];

        public Population(int len)
        {
            chromosomes = new Chromosome[len];
        }

        public Population InitializePopulation(Data data)
        {
            for (int c = 0; c < GAHelper.NUM_OF_POPULATION; ++c)
            {
                chromosomes[c] = new Chromosome(data).InitializeChromosome(data);
            }

            SortChromosomesByFitness(data);
            return this;
        }

        public void SortChromosomesByFitness(Data data)
        {
            Array.Sort(chromosomes, (chromosome1, chromosome2) =>
            {
                int flag = 0;
                if (chromosome1.GetFitness(data) < chromosome2.GetFitness(data)) flag = -1;
                if (chromosome1.GetFitness(data) > chromosome2.GetFitness(data)) flag = 1;
                return flag;
            });
        }

        public Chromosome[] Chromosomes
        {
            get => chromosomes;
        }
    }
}
