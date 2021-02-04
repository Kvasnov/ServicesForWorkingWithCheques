using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using ServicesForWorkingWithCheques.ChequeService.Log4net;
using ServicesForWorkingWithCheques.Domain.Core;

namespace ServicesForWorkingWithCheques.ChequeService
{
    public sealed class ChequeService : IChequeService
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings[ "DefaultConnection" ].ConnectionString;

        public IEnumerable< Cheque > GetChequeList( int chequeCount )
        {
            Logger.InitLogger();
            Logger.Log.Info( "Запуск GetChequeList" );

            if ( chequeCount < 0 )
            {
                Logger.Log.Warn( $"Запрос не выполнен. Клиент ввел {chequeCount}. Число чеков должно быть положительным" );

                return null;
            }

            var chequeList = new List< Cheque >();

            try
            {
                using ( IDbConnection db = new SqlConnection( connectionString ) )
                {
                    Logger.Log.Info( $"Запрос {chequeCount} чеков из бд {connectionString}" );
                    chequeList = db.Query< Cheque >( "get_cheques_pack", new { pack_size = chequeCount }, commandType: CommandType.StoredProcedure ).ToList();
                }
            }
            catch ( Exception e )
            {
                Logger.Log.Error( $"Произошла ошибка запроса чеков из бд: {e.Message}" );
                Logger.Log.Error( e.StackTrace );
            }

            return chequeList;
        }

        public void SaveCheque( Cheque cheque )
        {
            try
            {
                Logger.InitLogger();
                Logger.Log.Info( "Запуск SaveCheque" );

                using ( IDbConnection db = new SqlConnection( connectionString ) )
                {
                    Logger.Log.Info( $"Добавление нового чека id = {cheque.Id} в бд {connectionString}" );
                    db.Query( "save_cheques", new
                                              {
                                                  cheque_id = cheque.Id,
                                                  cheque_number = cheque.Number,
                                                  summ = cheque.Summ,
                                                  discount = cheque.Discount,
                                                  articles = string.Join( ";", cheque.Articles )
                                              }, commandType: CommandType.StoredProcedure );

                    Logger.Log.Info( "Чек успешно добавлен" );
                }
            }
            catch ( Exception e )
            {
                Logger.Log.Error( $"Произошла ошибка добавления чека: {e.Message}" );
                Logger.Log.Error( e.StackTrace );
            }
        }
    }
}