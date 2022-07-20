using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Decisions.GoogleCloud.Data;
using DecisionsFramework.Design.Flow.CoreSteps.StandardSteps;
using DecisionsFramework.ServiceLayer;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;

namespace Decisions.GoogleCloud
{
    public static class GoogleCloudUtility
    {
        public static BigQueryClient GetBigQueryClient(string projectId)
        {
            var settings = ModuleSettingsAccessor<GoogleCloudSettings>.GetSettings();

            GoogleCredential credential;
            BigQueryClient client;
            if (settings.UseJsonFile)
            {
                if (settings.CredentialsJson == null || settings.CredentialsJson.Contents == null || settings.CredentialsJson.Contents.Length == 0)
                {
                    throw new ArgumentNullException(nameof(settings.CredentialsJson), ErrorStringConstants.JsonNotConfigured);
                }
                
                // Authentication uses Json File to log in service account.
                var contents = Encoding.UTF8.GetString(settings.CredentialsJson.Contents, 0, settings.CredentialsJson.Contents.Length);
                if (string.IsNullOrEmpty(contents))
                {
                    throw new ArgumentNullException(nameof(settings.CredentialsJson),
                        ErrorStringConstants.JsonNotConfigured);
                }
                credential = GoogleCredential.FromJson(contents);
                client = BigQueryClient.Create(projectId, credential);
            }
            else
            {
                // Authentication configured through environment variable
                client = BigQueryClient.Create(projectId);
            }

            return client;
        }

        public static DynamicDataRow[] RunQueryWithReturn(string projectId, string query, bool useLegacySql = false)
        {
            
            // Execute Query
            BigQueryClient client = GoogleCloudUtility.GetBigQueryClient(projectId);

            // Use Legacy SQL syntax?
            QueryOptions? queryOptions = null;
            if (useLegacySql)
            {
                queryOptions = new() { UseLegacySql = useLegacySql };
            }

            // Execute the query
            BigQueryResults results = client.ExecuteQuery(query, null, queryOptions ?? new());

            // Build output columns and rows
            DataTable dataTable = new DataTable("Query Result");
            List<string> columnNames = new();
            List<DynamicDataRow> returnRows = new();
            foreach (BigQueryRow result in results)
            {
                DataRow newRow = dataTable.NewRow();
                foreach (TableFieldSchema r in result.Schema.Fields)
                {
                    if (!columnNames.Any(m => m.Equals(r.Name)))
                    {
                        columnNames.Add(r.Name);
                        dataTable.Columns.Add(r.Name, typeof(string));
                    }
                    
                    newRow[r.Name] = result[r.Name];
                }
                returnRows.Add(new DynamicDataRow(newRow));
            }

            return returnRows.ToArray();
        }
        
    }
}