using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAdaptor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TSManager
{
    public class TsManager
    {
        private readonly DataAdaptorDb _dataAdaptor;

       
        public TsManager()
        {
            _dataAdaptor = new DataAdaptorDb();
        }
       
        public async Task<List<string>> ParseJson(object jsonvalue)
        {
            List<string> assetsCreatedList = new List<string>();
            var serialized = JsonConvert.SerializeObject(jsonvalue);
            JToken entireJson = JToken.Parse(serialized);

            var collectionLink = string.Empty;

            foreach (var item in entireJson)
            {
                if (item is JProperty)
                {
                    var key = (item as JProperty).Name ;
                    var value = (item as JProperty).Value.ToString();
                    if (key == "TagName")
                    {
                        string[] tokens = value.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        var baseCollectionName = tokens[0];
                        collectionLink = await _dataAdaptor.CreateDocumentCollection(baseCollectionName);
                    }
                }

            }

            _dataAdaptor.CreateDocument(collectionLink, jsonvalue);
            return assetsCreatedList;
        }

        public async Task<List<string>>  GetAllTags()
        {
           return await _dataAdaptor.GetAllCollections();
        }

        public async Task<string> GetDataPoints(TSQueryModel query)
        {
            var _tag = query.Tags;
            var _limit = query.limit;
            var start = query.Start;
            var end = query.End;
            var collectionLink = await _dataAdaptor.GetCollectionLink(_tag.Name);
            return await _dataAdaptor.GetAllDocuments(collectionLink, _tag.Start, _tag.End, _tag.limit);
        }
    }
}
