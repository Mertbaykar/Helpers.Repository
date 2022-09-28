using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EF.Core.Extensions
{
    public static class DbContextExtensions
    {
        public static void BulkUpdate<TEntity>(this DbContext dbcontext, Action<PropertyHelper<TEntity>> propertySettings, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
        {
            try
            {
                PropertyHelper<TEntity> propertyHelper = new PropertyHelper<TEntity>(dbcontext);
                propertyHelper.InitSql();
                propertySettings.Invoke(propertyHelper);

                // Sql set kısımları üst satırda handle edildi
                string totalSql = propertyHelper.Sql;

                // Sql'in from kısmı oluştu
                string sqlFrom = dbcontext.Set<TEntity>().Where(whereExpression).ToQueryString();

                // Query'de 'set' sonrası ilk virgülü kaldır (sql hatası olmaması için)
                int sqlFirstCommaIndex = totalSql.IndexOf(',');
                totalSql = totalSql.Remove(sqlFirstCommaIndex, 1);

                // from kısmı query'ye eklendi
                totalSql += " from " + "(" + sqlFrom + ")";

                propertyHelper.Sql = totalSql;
                dbcontext.Database.ExecuteSqlRaw(propertyHelper.Sql, propertyHelper.SqlParameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }


        public static void BulkUpdate<TEntity>(this DbContext dbcontext, Action<PropertyHelper<TEntity>> propertySettings) where TEntity : class
        {
            try
            {
                dbcontext.BulkUpdate(propertySettings, x => 1 == 1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
