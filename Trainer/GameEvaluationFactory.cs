using ArtificialNeuralNetwork;
using BasicGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.GeneticAlgorithm;
using NeuralNetwork.GeneticAlgorithm.Evaluatable;
using Newtonsoft.Json;
using System.IO;

namespace Trainer
{
    public class GameEvaluationFactory : IEvaluatableFactory
    {
     
        public static List<double> LoadJson()
        {
            using (StreamReader r = new StreamReader("C:/Users/Hutli/Documents/BlackBox/javascript/src/data/INTC.json"))
            {
                string json = r.ReadToEnd();
                firstItem array = JsonConvert.DeserializeObject<firstItem>(json);
                IEnumerable<Item> arr = array.Prices;
                return arr.Select(itm => itm.Value).ToList();
            }
        }

        public class firstItem
        {
            public String Ticker;
            public List<Item> Prices;
        }

        public class Item
        {
            public DateTime Date;
            public double Value;
        }

        public IEvaluatable Create(INeuralNetwork neuralNetwork)
        {
            return new GameEvaluation(new Game(LoadJson()), neuralNetwork);
        }
    }
}
