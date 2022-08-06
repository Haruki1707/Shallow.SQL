using Shallow.SQL.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shallow.SQL
{
    public abstract class ModelTable<T>
    {
        public virtual string ID { get; internal set; }
        internal bool AlreadyOnDB { get; set; }

        internal ModelQuery<T> _query = ModelQuery.GetInstance<T>();
        internal Dictionary<string, object> _hiddenColumns = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Allows you to retrieve a column that you haven't declared as property on the object
        /// </summary>
        /// <param name="columnName">The name of the column that you want to retrieve that isn't already a property</param>
        /// <returns>A object (int, string, bool, etc) containing the data of the column</returns>
        /// <exception cref="ColumnNotFoundException"></exception>
        public object GetObjectOfNotDeclaredColumn(string columnName)
        {
            if(_hiddenColumns.ContainsKey(columnName))
                return _hiddenColumns[columnName];
            throw new ColumnNotFoundException($"{columnName} column not found");
        }

        /// <summary>
        /// Let you update/save the record in the database with the data of the object. Record has to be in database, else will return false.
        /// </summary>
        /// <returns>True if record could be updated, else false</returns>
        public bool Update() => AlreadyOnDB ? _query.UpdateObject(this) : false;

        /// <summary>
        /// Let you create the record in the database with the data of the object. Record has not to be in database, else will return false.
        /// </summary>
        /// <returns>True if record could be created, else false</returns>
        public bool Create() => !AlreadyOnDB ? (_query.CreateObject(this) ? AlreadyOnDB = true : AlreadyOnDB = false) : false;

        /// <summary>
        /// Let you delete the record in the database that matches the id of the object. Record has to be in database, else will return false.
        /// </summary>
        /// <returns>True if record could be deleted, else false</returns>
        public bool Delete() => AlreadyOnDB ? (_query.DeleteObject(this) ? AlreadyOnDB = true : AlreadyOnDB = false) : false;

        /// <summary>
        /// Let you refresh the object with the data in the database, deleting any modified data that was not updated/saved. Record has to be in database, else will return false.
        /// </summary>
        /// <returns>True if object could be refreshed with data of the database, else false</returns>
        public bool Refresh() => AlreadyOnDB ? _query.RefreshObject(this) : false;

        /// <summary>
        /// Allows to get child related record that has a foreign key with this table
        /// </summary>
        /// <typeparam name="U">The type returned from this method</typeparam>
        /// <param name="objectQuery">ModelQuery of the ModelTable you want to relation with</param>
        /// <param name="foreignKey">The column on child table that relates with this table. <c>Default: thisTableName_id</c></param>
        /// <param name="localKey">The column on this table that relates with child table. <c>Default: id</c></param>
        /// <returns>One object/record of type: <typeparamref name="U"/></returns>
        protected U HasOne<U>(ModelQuery<U> objectQuery, string foreignKey = null, string localKey = null)
            => _query.HasOne(objectQuery, this, foreignKey, localKey);

        /// <summary>
        /// Allows to get child related records that has a foreign key with this table
        /// </summary>
        /// <typeparam name="U">The type returned from this method</typeparam>
        /// <param name="objectQuery">ModelQuery of the ModelTable you want to relation with</param>
        /// <param name="foreignKey">The column on child table that relates with this table. <c>Default: thisTableName_id</c></param>
        /// <param name="localKey">The column on this table that relates with child table. <c>Default: id</c></param>
        /// <returns>Many objects/records of type: <typeparamref name="U"/>[]</returns>
        protected U[] HasMany<U>(ModelQuery<U> objectQuery, string foreignKey = null, string localKey = null) 
            => _query.HasMany(objectQuery, this, foreignKey, localKey);

        /// <summary>
        /// Inverse relation of hasMany/hasOne, allows to get parent related record
        /// </summary>
        /// <typeparam name="U">The type returned from this method</typeparam>
        /// <param name="objectQuery">ModelQuery of the ModelTable you want to relation with</param>
        /// <param name="foreignKey">The column on this table that relates with the parent. <c>Default: parentTableName_id</c></param>
        /// <param name="ownerKey">The column on parent that relates with this table. <c>Default: id</c></param>
        /// <returns>Many objects/records of type: <typeparamref name="U"/></returns>
        protected U BelongsTo<U>(ModelQuery<U> objectQuery, string foreignKey = null, string ownerKey = null) 
            => _query.BelongsTo(objectQuery, this, foreignKey, ownerKey);

        /// <summary>
        /// Allows to get the records from another table related with this table trough a pivot table
        /// </summary>
        /// <typeparam name="U">The type returned from this method</typeparam>
        /// <param name="objectQuery">ModelQuery of the ModelTable you want to relation with</param>
        /// <param name="relationTable">The name of the pivot table, usually the singular of the tables name ordered by ASC. <c>Default: table1Name_table2Name</c></param>
        /// <param name="thisForeignKey">The foreign key on pivot table that relates it with this table. <c>Default: thisTableName_id</c></param>
        /// <param name="thatForeignKey">The foreign key on pivot table that relates it with the other table. <c>Default: thatTableName_id</c></param>
        /// <param name="localKey">The column on this table that relates it with <paramref name="thisForeignKey"/> on pivot table. <c>Default: id</c></param>
        /// <param name="ownerKey">The column on the other table that relates it with <paramref name="thatForeignKey"/> on pivot table. <c>Default: id</c></param>
        /// <returns>Many objects/record of type: <typeparamref name="U"/>[]</returns>
        protected U[] BelongsToMany<U>(ModelQuery<U> objectQuery, string relationTable = null, string thisForeignKey = null, string thatForeignKey = null, string localKey = null, string ownerKey = null) 
            => _query.BelongsToMany(objectQuery, this, relationTable, thisForeignKey, thatForeignKey, localKey, ownerKey);
    }
}
