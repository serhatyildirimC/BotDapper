using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface ICoinRepository
    {
        List<Coin> GetAllCoins();

        Coin GetCoinById(int id);

        Coin CrateCoin(Coin coin);

        Coin UpdateCoin(Coin coin);

        bool DeleteCoin(int id);
    }
}
