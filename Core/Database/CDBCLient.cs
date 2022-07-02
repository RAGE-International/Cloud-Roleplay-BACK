using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Database
{
    public class CDBCLient
    {
        private readonly string connectionString = "mongodb://127.0.0.1:27017";
        private readonly string databaseName = "cloud";

        internal readonly IMongoDatabase database;

        public IMongoDatabase GetMongoClient()
        {

            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);
            return db;
        }

        public IMongoCollection<T> GetCollection<T>(string collection)
        {
            return GetMongoClient().GetCollection<T>(collection);
        }

        public async Task<List<T>> GetAllFromCollection<T>(string collection)
        {
            var result = await GetCollection<T>(collection).FindAsync(_ => true);
            return await result.ToListAsync<T>();
        }

        public async Task<T> GetOneFromCollection<T>(string collection, Expression<Func<T, bool>> condition)
        {
            return await GetCollection<T>(collection).Find(condition).FirstOrDefaultAsync();
        }

        public async Task Update<T>(string coll, T document, Expression<Func<T, bool>> condition)
        {
            var collection = GetCollection<T>(coll);
            collection.ReplaceOne(condition, document);
        }

        public async Task AddOneToCollection <T>(string collection, T document)
        {
            var collectionToAdd = GetCollection<T>(collection);
            await collectionToAdd.InsertOneAsync(document);
        }

        public CDBCLient()
        {
            database = GetMongoClient();
        }

    }
}
