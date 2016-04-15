using ArtificialNeuralNetwork;
using BasicGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.GeneticAlgorithm;
using NeuralNetwork.GeneticAlgorithm.Evaluatable;

namespace Trainer
{
    public class GameEvaluation : IEvaluatable
    {
        private List<double> his = new List<double>();
        private readonly Game _game;
        private readonly INeuralNetwork _neuralNet;
        public static int INPUTSIZE = 30;
        

        public GameEvaluation(Game game, INeuralNetwork neuralNet)
        {
            _game = game;
            _neuralNet = neuralNet;
        }

        public void RunEvaluation()
        {
            _game.reset();
            
            while (!_game.IsGameWon() && !_game.IsGameLost())
            {
                Game.TradeAction action = Game.TradeAction.DO_NOTHING;
                double priceNow = _game.price;
                his.Add(priceNow);
                double nextPrice = 0;
                double bestPrice = 0;
                double bestProb = 0;

                if (his.Count >= INPUTSIZE - 1)
                {
                    if (his.Count > INPUTSIZE - 1) his.RemoveAt(0);
                    for (double i = 0.75; i < 1.25; i += 0.05)
                    {
                        if (his.Count > INPUTSIZE - 1) his.RemoveAt(0);
                        nextPrice = priceNow * i;
                        his.Add(nextPrice);
                        _neuralNet.SetInputs( his.ToArray() );
                        _neuralNet.Process();
                        double nextProb = _neuralNet.GetOutputs()[0];
                        if (nextProb > bestProb)
                        {
                            bestProb = nextProb;
                            bestPrice = nextPrice;
                        }
                        //if (nextPrice > 0.5 && _game.canBuy) action = Game.TradeAction.BUY;
                        //else if (nextPrice <= 0.5 && _game.canSell) action = Game.TradeAction.SELL;
                    }
                    if (bestPrice > _game.price && _game.canBuy) action = Game.TradeAction.BUY;
                    else if (bestPrice < _game.price && _game.canSell) action = Game.TradeAction.SELL;
                }

                _game.UseTurn(action);
            }

        }

       

        public double GetEvaluation()
        {
            if (!_game.IsGameWon() && !_game.IsGameLost())
            {
                throw new NotSupportedException("GetSessionEvaluation is not supported when game is not finished");
            }
            return _game.profit;
        }
    }
}
