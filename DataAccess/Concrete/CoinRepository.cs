using Dapper;
using DataAccess.Abstract;
using Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class CoinRepository : ICoinRepository
    {
        SqlConnection sqlCon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;initial Catalog=TelegramDatabase;Integrated Security=true;");

        public Coin CrateCoin(Coin coin)
        {
            try
            {
                sqlOpen();
                sqlCon.Query<Coin>(@"INSERT INTO [dbo].[Table]([Name],[Price],[Change]) VALUES (@Name,@Price,@Change", coin);
                return coin;

            }
            catch (Exception ex)
            {

                throw new Exception("Crate hatası" + ex.Message.ToString());
            }
            finally
            {
                sqlClose();
            }
        }

        private void sqlOpen()
        {
            if (sqlCon.State == System.Data.ConnectionState.Closed)
                sqlCon.Open();
        }
        private void sqlClose()
        {
            if (sqlCon.State == System.Data.ConnectionState.Open)
                sqlCon.Close();
        }

        public bool DeleteCoin(int id)
        {
            try
            {
                sqlOpen();
                sqlCon.Query<Coin>(@"DELETE FROM [dbo].[Table] WHERE Id =@Id", id);
                return true;

            }
            catch (Exception ex)
            {

                throw new Exception("Delete hatası" + ex.Message.ToString());
            }
            finally
            {
                sqlClose();
            }
        }

        public List<Coin> GetAllCoins()
        {
            try
            {
                sqlOpen();
                List<Coin> list = sqlCon.Query<Coin>("SELECT * FROM [dbo].[binance]").ToList();
                return list;

            }
            catch (Exception ex)
            {

                throw new Exception("GetAll hatası" + ex.Message.ToString());
            }
            finally
            {
                sqlClose();
            }
        }

        public Coin GetCoinById(int id)
        {
            try
            {
                sqlOpen();
                
                var coin = sqlCon.Query<Coin>("SELECT * FROM [dbo].[Binance] where id=@id order by id ", new { id });
                var ere = coin.FirstOrDefault();
                return (Coin)ere;

            }
            catch (Exception ex)
            {

                throw new Exception("GetbyId hatası" + ex.Message.ToString());
            }
            finally
            {
                sqlClose();
            }

        }

        public Coin UpdateCoin(Coin coin)
        {
            try
            {
                sqlOpen();
                sqlCon.Query<Coin>(@"INSERT INTO [dbo].[Table]([Name]=@Name,[Price]=@Price,[Change]=@Change)", coin);
                return coin;

            }
            catch (Exception ex)
            {

                throw new Exception("Update hatası" + ex.Message.ToString());
            }
            finally
            {
                sqlClose();
            }
        }
    }  
}
