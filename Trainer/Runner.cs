using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.WeightInitializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.GeneticAlgorithm;
using NeuralNetwork.GeneticAlgorithm.Evaluatable;
using NeuralNetwork.GeneticAlgorithm.Evolution;
using NeuralNetwork.GeneticAlgorithm.Utils;
using BasicGame;

namespace Trainer
{
    class Runner
    {
        public static List<Game.TradeAction> bestStrat = new List<Game.TradeAction>();

        static void Main(string[] args)
        {
            List<double> data = GameEvaluationFactory.LoadJson();
            CheatGame game = new CheatGame(data);
            for(int i = 0; i < data.Count - 1; i++)
            {
                Game.TradeAction action = Game.TradeAction.DO_NOTHING;
                if(game.price >= game.next && game.canSell) { action = Game.TradeAction.SELL; }
                if (game.price <= game.next && game.canBuy) { action = Game.TradeAction.BUY; }
                game.UseTurn(action);
                bestStrat.Add(action);
            }

            Game godGame = new Game(data);
            double lastPrice = 0;
            while(!godGame.IsGameLost() && !godGame.IsGameWon())
            {
                Game.TradeAction action = Game.TradeAction.DO_NOTHING;
                if (godGame.price <= lastPrice && godGame.canSell) action = Game.TradeAction.SELL;
                else if (godGame.price >= lastPrice && godGame.canBuy) action = Game.TradeAction.BUY;
                lastPrice = godGame.price;
                godGame.UseTurn(action);
            }


            Console.WriteLine(game.profit);
            Console.WriteLine(godGame.profit);

            NeuralNetworkConfigurationSettings networkConfig = new NeuralNetworkConfigurationSettings
            {
                NumInputNeurons = GameEvaluation.INPUTSIZE,
                NumOutputNeurons = 1,
                NumHiddenLayers = 2,
                NumHiddenNeurons = GameEvaluation.INPUTSIZE,
                SummationFunction = new SimpleSummation(),
                ActivationFunction = new TanhActivationFunction()
            };
            GenerationConfigurationSettings generationSettings = new GenerationConfigurationSettings
            {
                UseMultithreading = true,
                GenerationPopulation = 10
            };
            EvolutionConfigurationSettings evolutionSettings = new EvolutionConfigurationSettings
            {
                NormalMutationRate = 0.1,
                HighMutationRate = 0.5,
                GenerationsPerEpoch = 1000,
                NumEpochs = 10000
            };
            MutationConfigurationSettings mutationSettings = new MutationConfigurationSettings
            {
                MutateAxonActivationFunction = true,
                MutateNumberOfHiddenLayers = true,
                MutateNumberOfHiddenNeuronsInLayer = true,
                MutateSomaBiasFunction = true,
                MutateSomaSummationFunction = true,
                MutateSynapseWeights = true
            };
            var random = new RandomWeightInitializer(new Random());
            INeuralNetworkFactory factory = NeuralNetworkFactory.GetInstance(SomaFactory.GetInstance(networkConfig.SummationFunction), AxonFactory.GetInstance(networkConfig.ActivationFunction), SynapseFactory.GetInstance(new RandomWeightInitializer(new Random())), SynapseFactory.GetInstance(new ConstantWeightInitializer(1.0)), random);
            IBreeder breeder = BreederFactory.GetInstance(factory, random).Create();
            IMutator mutator = MutatorFactory.GetInstance(factory, random).Create(mutationSettings);
            IEvalWorkingSet history = EvalWorkingSetFactory.GetInstance().Create(100);
            IEvaluatableFactory evaluatableFactory = new GameEvaluationFactory();

            var GAFactory = GeneticAlgorithmFactory.GetInstance(evaluatableFactory);
            IGeneticAlgorithm evolver = GAFactory.Create(networkConfig, generationSettings, evolutionSettings, factory, breeder, mutator, history, evaluatableFactory);
            evolver.RunSimulation();
        }
    }
}
