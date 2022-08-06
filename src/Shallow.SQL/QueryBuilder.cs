using Shallow.SQL.Enums;
using Shallow.SQL.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shallow.SQL
{
    public class QueryBuilder<T>
    {
        protected ModelQuery<T> query;
        internal string queryText;

        public string QueryText => queryText.Replace("%INNER%", "");

        internal QueryBuilder(ModelQuery<T> modelQuery, string queryText)
        {
            query = modelQuery;
            this.queryText = queryText;
        }

        public QueryBuilder<T> Where(string Column, Operators Operator, string value)
            => whereOrWhere((!queryText.Contains("WHERE") ? " WHERE" : "AND"), Column, Operator, value);

        public QueryBuilder<T> OrWhere(string Column, Operators Operator, string value)
            => whereOrWhere((!queryText.Contains("WHERE") ? "WHERE" : "OR"), Column, Operator, value);

        public QueryBuilder<T> Where(Action action)
            => whereOrWhere((!queryText.Contains("WHERE") ? "WHERE" : "AND"), action);

        public QueryBuilder<T> OrWhere(Action action)
            => whereOrWhere((!queryText.Contains("WHERE") ? "WHERE" : "OR"), action);

        private QueryBuilder<T> whereOrWhere(string type, string Column, Operators Operator, string value)
        {
            queryText += $" {type} {Column} {Operator.Parse()} '{value}'";
            return this;
        }

        private QueryBuilder<T> whereOrWhere(string type, Action action)
        {
            string _temp = queryText;
            queryText = "";
            action.Invoke();
            queryText = $"{_temp} {type} ({queryText.Replace("WHERE", "")})";
            return this;
        }

        public QueryBuilder<T> InnerJoin(string table1, string ColumnLeft, string ColumnRight, Operators joinOperator = Operators.Equals, string table2 = null)
        {
            if (table2 == null)
                table2 = query._table;

            queryText = queryText.Replace("%INNER%", $"INNER JOIN `{table1}` {table1.Alias()} " +
                $"ON {table2.Alias()}.{ColumnLeft} {joinOperator.Parse()} {table1.Alias()}.{ColumnRight} %INNER%");
            return this;
        }

        /*public QueryBuilder<T> WhereRelation<U>(ModelQuery<U> ObjectQuery, Relations Relation, string Column, Operators Operator, string value, 
            string ColumnLeft = null, Operators joinOperator = Operators.Equals, string ColumnRight = null)
        {
            switch (Relation)
            {
                case Relations.hasOne:
                    return mQuery.Where(thisColumn, Operators.Equals, id.ToString()).Get();
                    break;
                case Relations.hasMany:
                    break;
                case Relations.belongsTo:
                    break;
                case Relations.belongsToMany:
                    break;
                default:
                    break;
            }
        }*/

        public T[] Get() 
            => query.GetObjects(QueryText);
        internal T Get(object Object)
            => query.GetObjects(QueryText, Object).FirstOrDefault();
    }
}
