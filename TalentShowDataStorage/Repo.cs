﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TalentShowDataStorage.Helpers;
using TalentShow.Repos;

namespace TalentShowDataStorage
{
    public abstract class Repo<T> where T : IIdentity
    {
        protected const string ID = "id";

        public void Add(T item)
        {
            var fieldNamesAndValues = GetFieldNamesAndValuesForInsertOrUpdate(item);
            SqlCommand command = SqlServerCommandHelper.GetInsertCommand(GetTableName(), fieldNamesAndValues, ID);
            item.SetId( SqlServerCommandHelper.ExecuteSqlCommand(command, outputIdValue: true));
        }

        protected abstract string GetTableName();

        public void Update(T item)
        {
            int id = item.Id;
            var fieldNamesAndValues = GetFieldNamesAndValuesForInsertOrUpdate(item);
            var whereClause = WhereIdEquals(id);
            var whereClauseParameterNamesAndValues = new Dictionary<string, object>();
            whereClauseParameterNamesAndValues.Add(ID, id);

            SqlServerCommandHelper.GetUpdateCommand(GetTableName(), fieldNamesAndValues, whereClause, whereClauseParameterNamesAndValues);
        }

        protected abstract Dictionary<string, object> GetFieldNamesAndValuesForInsertOrUpdate(T item);

        public ICollection<T> GetAll()
        {
            string sql = GetSelectStatement() + ";";
            SqlCommand command = new SqlCommand(sql);
            IDataReader reader = SqlServerCommandHelper.ExecuteSqlQuery(command);

            var items = new List<T>();

            while (reader.Read())
            {
                T item = GetItemFromDataReader(reader);
                items.Add(item);
            }

            return items;
        }

        protected abstract T GetItemFromDataReader(IDataReader reader);

        public T Get(int id)
        {
            string sql = GetSelectStatement() + WhereIdEquals(id);
            SqlCommand command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@" + ID, id);
            IDataReader reader = SqlServerCommandHelper.ExecuteSqlQuery(command);      
            reader.Read();
            return GetItemFromDataReader(reader);
        }

        public void Delete(T item)
        {
            Delete(item.Id);
        }

        public void Delete(int id)
        {
            string sql = SqlServerCommandHelper.GetSimpleDeleteStatement(GetTableName()) + WhereIdEquals(id);
            SqlCommand command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@" + ID, id);
            SqlServerCommandHelper.ExecuteSqlCommand(command);
        }

        public void DeleteAll()
        {
            string sql = SqlServerCommandHelper.GetSimpleDeleteStatement(GetTableName());
            SqlCommand command = new SqlCommand(sql);
            SqlServerCommandHelper.ExecuteSqlCommand(command);
        }

        private string GetSelectStatement()
        {
            var fieldNames = GetFieldNamesForSelectStatement();
            return SqlServerCommandHelper.GetSimpleSelectStatement(GetTableName(), fieldNames);
        }

        protected abstract ICollection<string> GetFieldNamesForSelectStatement();

        private static string WhereIdEquals(int id)
        {
            return " where [" + ID + "] = @" + ID + ";";
        }
    }
}