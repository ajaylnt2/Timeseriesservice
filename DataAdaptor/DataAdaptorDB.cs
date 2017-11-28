using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Linq;

namespace DataAdaptor
{
    public class DataAdaptorDb
    {

        private const string DatabaseName = "TimeSeriesData";
        private const string EndpointUrl = "https://tsservice.documents.azure.com:443/";

        private const string PrimaryKey =
            "ixUNu9piv6Rh5dEfpqikLV7GHjyNQEAVuWtLFJS1krknQLgNKJqq2n6DeQB3DA7Inleh9U2Xll7e2V47VHY2dw==";

        private readonly DocumentClient _client;

        public DataAdaptorDb()
        {
            _client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            GetDbInit().Wait();
        }

        #region Document Queries
        public async void CreateDocument(string collectionLink, object value)
        {
            Document created = await _client.CreateDocumentAsync(collectionLink, value);
        }
        private string GenerateQueryString(string collectionName, string property, string value)
        {
            string query = $"SELECT*FROM{collectionName}f WHERE f.{property}={value}";
            return query;
        }
        private Uri GetDocumentUri(string databaseId, string collectionId, string documentId)
        {
            return UriFactory.CreateDocumentUri(databaseId, collectionId, documentId);
        }
        private async void ReadDocument()
        {
            //var response = await _client.ReadDocumentAsync();
        }

        private async Task<string> ReadDocumentFeed(string collectionLink, int limit, string Start, string End)
        {
            List<dynamic> _documents = new List<dynamic>();

            var sql = $"SELECT TOP {limit} * FROM c WHERE c.TimeStamp BETWEEN {Start} AND {End}";
            var query = _client
               .CreateDocumentQuery(collectionLink, sql)
               .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var documents = await query.ExecuteNextAsync();

                foreach (var doc in documents)
                {
                    _documents.Add(doc);
                    Console.WriteLine(doc);
                }
            }
            //var result = await _client.ReadDocumentFeedAsync(collectionLink, new FeedOptions { MaxItemCount = limit });


            //foreach (Document doc in result)
            //{
            //    Console.WriteLine(doc);
            //    _documents.Add(doc);
            //}
            return JsonConvert.SerializeObject(_documents);
        }

        public async Task<string> GetAllDocuments(string collectionLink, string start, string end, int limit)
        {


            return await ReadDocumentFeed(collectionLink, limit, start, end);
        }

        public async Task<string> GetCollectionLink(string tagName)
        {
            return await GetDbCollectionInit(tagName);
        }
        #endregion

        #region Collection Queries
        private async Task<string> GetDbCollectionInit(string collectionName)
        {
            var result =
                await _client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName),
                    new DocumentCollection { Id = collectionName });
            return result.Resource.SelfLink;
        }
        public async Task<List<string>> GetAllCollections()
        {
            var colls = await _client.ReadDocumentCollectionFeedAsync(UriFactory.CreateDatabaseUri(DatabaseName));
            Console.WriteLine("\n5. Reading all DocumentCollection resources for a database");
            foreach (var coll in colls)
            {
                Console.WriteLine(coll.Id);
            }
            return colls.Select(x => x.Id).ToList();
        }
        public async void DeleteCollection(DocumentCollection collection)
        {
            await _client.DeleteDocumentCollectionAsync(collection.SelfLink);
        }
        public async Task<string> CreateDocumentCollection(string name)
        {
            var selflink = await GetDbCollectionInit(name);
            return selflink;
        }
        #endregion

        #region Database Queries
        private async Task GetDbInit()
        {
            await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
        }
        #endregion
    }
}
