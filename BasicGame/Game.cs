using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicGame
{
    public class CheatGame : Game
    {
        public double next { get { return _data[index + 1]; } }

        public CheatGame(List<double> data): base(data) { }

    }

    public class Game
    {
        protected List<double> _data;
        protected double _profit = 0;
        protected int index = 0;
        protected bool inStock = false;

        public double profit { get { return _profit; } }
        public Game(List<double> data)
        {
            _data = data;
        }

        public void reset()
        {
            _profit = 0;
            inStock = false;
            index = 0;
        }

        public double price { get { return _data[index]; }  }

        public bool canBuy { get { return (!IsGameLost() && !IsGameWon() && !inStock ); } }
        public bool canSell { get { return (!IsGameLost() && !IsGameWon() && inStock); } }

        public bool IsGameWon()
        {
            if (index == _data.Count - 2)
            {
                _profit += inStock ? _data.Last() : 0.0;
                if (profit > 0) return true;
            }
            return false;
        }

        public bool IsGameLost()
        {
            if (index == _data.Count - 2)
            {
                _profit += inStock ? _data.Last() : 0.0;
                if (profit <= 0) return true;
            }
            return false;
        }

        public void UseTurn(TradeAction a)
        {
            switch (a)
            {
                case TradeAction.DO_NOTHING:
                    break;
                case TradeAction.BUY:
                    if (!canBuy)
                        throw new Exception("broke rules");
                    inStock = true;
                    _profit -= price;
                    break;
                case TradeAction.SELL:
                    if (!canSell)
                        throw new Exception("broke rules");
                    inStock = false;
                    _profit += price;
                    break;
            }
            index++;
        }

        public enum TradeAction { DO_NOTHING, BUY, SELL }
    }

}
