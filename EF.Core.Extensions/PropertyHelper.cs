using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Data.SqlClient;

namespace EF.Core.Extensions
{
    public class PropertyHelper<TEntity> where TEntity : class
    {
        public PropertyHelper(DbContext dbContext)
        {
            DbContext = dbContext;
        }
        internal string Sql { get; set; }
        internal DbContext DbContext { get; set; }
        internal SqlParameterCollection SqlParameters { get; set; }
        internal IEntityType EntityType => DbContext.Model.FindEntityType(typeof(TEntity));
        internal string Schema => EntityType.GetSchema();
        internal string TableName => EntityType.GetSchemaQualifiedTableName();
        internal StoreObjectIdentifier StoreObjectIdentifier => StoreObjectIdentifier.Table(TableName, Schema);

        public void SetProperty<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value) where TProperty : IConvertible
        {
            try
            {
                if (DbContext is null)
                    throw new Exception("DbContext is required");

                string propertyName = GetPropertyNameFromExpression(propertyExpression);
                string columnName = EntityType.FindProperty(propertyName).GetColumnName(StoreObjectIdentifier);

                string sqlParameterKey = "@" + propertyName;
                if (SqlParameters.Contains(sqlParameterKey))
                    throw new Exception(propertyName + " property is set more than once");

                // Sql handling
                SqlParameter sqlParameter = new SqlParameter(sqlParameterKey, value);
                SqlParameters.Add(sqlParameter);
                Sql += "," + columnName + "=" + sqlParameterKey;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }

        internal void InitSql()
        {
            Sql = "Update " + TableName + " set ";
        }

        #region GetPropertyNameFromExpression

        internal string GetPropertyNameFromExpression<TProperty>(Expression<Func<TEntity, TProperty>> expression) where TProperty : IConvertible
        {
            try
            {
                LambdaExpression lambda = (LambdaExpression)expression;
                MemberExpression memberExpression;

                if (lambda.Body is UnaryExpression)
                {
                    UnaryExpression unaryExpression = (UnaryExpression)(lambda.Body);
                    memberExpression = (MemberExpression)(unaryExpression.Operand);
                }
                else
                    memberExpression = (MemberExpression)(lambda.Body);

                PropertyInfo property = memberExpression.Member as PropertyInfo;
                return property.Name;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion
    }
}
