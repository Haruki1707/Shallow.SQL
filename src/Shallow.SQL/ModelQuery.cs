using Shallow.SQL.Exceptions;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Shallow.SQL.Structs;
using Shallow.SQL.Extensions;
using Shallow.SQL.Attributes;
using Shallow.SQL.Enums;

namespace Shallow.SQL
{
    public class ModelQuery<T>
    {
        internal Type _type;
        internal string _table;
        internal Column[] _props;
        internal string[] _relations;
        private string _paramsUpdate = "";
        private string _paramsInsert;

        /// <summary>
        /// Returns a new generic object of ModelQuery type
        /// </summary>
        /// <exception cref="TableNotFoundException">Occurs when the table is not found in the database</exception>
        internal ModelQuery()
        {
            _type = typeof(T);
            _props = _type.GetProps();
            _relations = _type.GetRelations();

            var tempProps = _props.Where(p => p.VarName != "ID");
            foreach (Column item in tempProps)
                _paramsUpdate += $"{item.ColumnName} = @{item.ColumnName}{(tempProps.Last().ColumnName != item.ColumnName ? ", " : " ")}";

            _paramsInsert = $"( {string.Join(", ", tempProps.Select(c => c.ColumnName))} ) VALUES ( @{string.Join(", @", tempProps.Select(c => c.ColumnName))} )";

            TableName attribute = (TableName)typeof(T).GetCustomAttribute(typeof(TableName));
            string tempTable = attribute != null ? attribute.Name : typeof(T).Name;

            if (SQL.TableExists(tempTable.Pluralize(false)))
                _table = tempTable.Pluralize(false);
            else if (SQL.TableExists(tempTable.Singularize(false)))
                _table = tempTable.Singularize(false);
            else
                throw new TableNotFoundException($"Make sure that either `{tempTable.Singularize(false)}` or `{tempTable.Pluralize(false)}` table exist on `{SQL.Database}` database");
        }

        /// <summary>
        /// Executes a query to get all the records of the specified table to objects
        /// </summary>
        /// <returns>Many objects/records of type: <typeparamref name="T"/></returns>
        public T[] All() 
            => GetObjects($"SELECT * FROM `{_table}`");

        /// <summary>
        /// Executes a query to get all the records that equals the specified id to objects
        /// </summary>
        /// <param name="id">The id of the records you want to retrieve</param>
        /// <returns>Many objects/records of type: <typeparamref name="T"/></returns>
        public T[] FindById(string id)
            => GetObjects($"SELECT * FROM `{_table}` WHERE {_props.Where(p => p.VarName == "ID").FirstOrDefault().ColumnName} = '{id}'");

        /// <inheritdoc cref="FindById(string)"/>
        public T[] FindById(int id)
            => FindById(id.ToString());

        /// <inheritdoc cref="FindById(string)"/>
        public T[] FindById(ModelTable<T> Object)
            => Object.ID != null ? FindById(Object.ID.ToString()) : new T[0];


        /// <summary>
        /// A query builder with SELECT * FROM thisTable, which you can add InnerJoin's, Where's, OrWhere's, etc
        /// </summary>
        /// <returns>QueryBuilder</returns>
        public QueryBuilder<T> Builder()
            => new QueryBuilder<T>(this, $"SELECT {_table.Alias()}.* FROM `{_table}` {_table.Alias()} %INNER%");

        /// <summary>
        /// Initializes a query builder with a `WHERE` added
        /// </summary>
        /// <param name="Column">The column you want to condition</param>
        /// <param name="operator">The operator of the condition</param>
        /// <param name="value">The value of the constraint</param>
        /// <returns>QueryBuilder</returns>
        public QueryBuilder<T> Where(string Column, Operators @operator, string value) 
            => Builder().Where(Column, @operator, value);
        /// <summary>
        /// Initializes a query builder with a `INNER JOIN` added
        /// </summary>
        /// <param name="table">The table you want to join</param>
        /// <param name="columnLeft">The condition column of the left table</param>
        /// <param name="columnRight">The condition column of the right table</param>
        /// <returns>QueryBuilder</returns>
        public QueryBuilder<T> InnerJoin(string table, string columnLeft, string columnRight)
            => Builder().InnerJoin(table, columnLeft, columnRight);

        internal bool UpdateObject(ModelTable<T> tableObject) 
            => ExecuteNonQuery($"UPDATE `{_table}` SET {_paramsUpdate} WHERE id = '{tableObject.ID}'", tableObject);
        internal bool CreateObject(ModelTable<T> tableObject)
            => ExecuteNonQuery($"INSERT INTO `{_table}` {_paramsInsert}", tableObject);
        internal bool DeleteObject(ModelTable<T> tableObject)
            => ExecuteNonQuery($"DELETE FROM `{_table}` WHERE id = '{tableObject.ID}'", tableObject);
        internal bool RefreshObject(ModelTable<T> tableObject)
            => Builder().Where($"{_table.Alias()}.id", Operators.Equals, tableObject.ID.ToString()).Get(tableObject) != null ? true : false;

        internal U HasOne<U, V>(ModelQuery<U> objectQuery, ModelTable<V> modelTable, string foreignKey, string localKey)
        {
            return HasMany(objectQuery, modelTable, foreignKey, localKey).LastOrDefault();
        }

        internal U[] HasMany<U, V>(ModelQuery<U> objectQuery, ModelTable<V> modelTable, string foreignKey, string localKey)
        {
            IsNotNullOrDefaultByRef(ref foreignKey, $"{_table.Singularize(false)}_id");
            IsNotNullOrDefaultByRef(ref localKey, "id");

            ColumnExistsOrFail(objectQuery._table, foreignKey);
            ColumnExistsOrFail(_table, localKey);

            return objectQuery.Where(foreignKey, Operators.Equals, modelTable.GetValueOrFail(localKey, _props).ToString()).Get();
        }
        internal U BelongsTo<U, V>(ModelQuery<U> objectQuery, ModelTable<V> modelTable, string foreignKey, string ownerKey)
        {
            IsNotNullOrDefaultByRef(ref foreignKey, $"{objectQuery._table.Singularize(false)}_id");
            IsNotNullOrDefaultByRef(ref ownerKey, "id");

            ColumnExistsOrFail(_table, foreignKey);
            ColumnExistsOrFail(objectQuery._table, ownerKey);

            return objectQuery.Where(ownerKey, Operators.Equals, modelTable.GetValueOrFail(foreignKey, _props).ToString()).Get().LastOrDefault();
        }

        internal U[] BelongsToMany<U, V>(ModelQuery<U> objectQuery, ModelTable<V> modelTable, string relationTable, string thisForeignKey, string thatForeignKey, string localKey, string ownerKey)
        {
            IsNotNullOrDefaultByRef(ref thisForeignKey, $"{_table.Singularize(false)}_id");
            IsNotNullOrDefaultByRef(ref thatForeignKey, $"{objectQuery._table.Singularize(false)}_id");
            if (string.IsNullOrWhiteSpace(relationTable))
            {
                string[] tables = new string[] {_table.Singularize(false), objectQuery._table.Singularize(false) };
                Array.Sort(tables);
                relationTable = String.Join("_", tables);
            }
            IsNotNullOrDefaultByRef(ref localKey, "id");
            IsNotNullOrDefaultByRef(ref ownerKey, "id");

            TableExistsOrFai(relationTable);
            ColumnExistsOrFail(_table, localKey);
            ColumnExistsOrFail(objectQuery._table, ownerKey);
            ColumnExistsOrFail(relationTable, thatForeignKey);
            ColumnExistsOrFail(relationTable, thatForeignKey);

            return objectQuery.Builder().InnerJoin(relationTable, ownerKey, thatForeignKey)
                .Where($"{relationTable.Alias()}.{thisForeignKey}", Operators.Equals, modelTable.GetValueOrFail(localKey, _props).ToString()).Get();
        }

        internal bool ExecuteNonQuery(string query, ModelTable<T> ModelQuery)
            => SQL.ExecuteNonQuery(ModelQuery, query, _props, _type, _table);

        internal T[] GetObjects(string query, object insObject = null)
            => SQL.GetObjects<T>(query, _props, insObject);

        internal static void IsNotNullOrDefaultByRef(ref string var, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(var))
                var = defaultValue;
        }

        internal static void TableExistsOrFai(string table)
        {
            if (!SQL.TableExists(table))
                throw new TableNotFoundException($"`{table}` table doesn't exist on `{SQL.Database}` database");
        }

        internal static void ColumnExistsOrFail(string table, string column)
        {
            if (!SQL.ColumnExists(table, column))
                throw new ColumnNotFoundException($"`{column}` column on `{table}` table doesn't exist");
        }
    }
}
