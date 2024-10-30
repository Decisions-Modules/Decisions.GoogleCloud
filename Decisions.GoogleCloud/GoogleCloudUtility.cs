using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Decisions.GoogleCloud.Data;
using DecisionsFramework.Design.Flow.CoreSteps.StandardSteps;
using DecisionsFramework.ServiceLayer;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.CloudIdentity.v1;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Storage.V1;

namespace Decisions.GoogleCloud;

public static class GoogleCloudUtility
{
    /// <summary>
    /// Runs a raw SQL query using the BigQuery API
    /// </summary>
    public static DynamicDataRow[] RunQueryWithReturn(string projectId, string query, CredentialsJson credentialsJson, bool useLegacySql = false)
    {
        // Execute Query
        BigQueryClient client = GetBigQueryClient(projectId, credentialsJson);

        // Use Legacy SQL syntax?
        QueryOptions queryOptions = null;
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
    
    #region Client and Credential Getters
    
    /// <summary>
    /// Creates a client for use with the Storage API
    /// </summary>
    public static StorageClient GetStorageClient(CredentialsJson credentialsJson)
    {
        GoogleCredential credentials = GetGoogleCredentials(credentialsJson);
        return credentials != null ? StorageClient.Create(credentials) : StorageClient.Create();
    }
    
    /// <summary>
    /// Creates a client for use with the BigQuery API
    /// </summary>
    public static BigQueryClient GetBigQueryClient(string projectId, CredentialsJson credentialsJson)
    {
        GoogleCredential credentials = GetGoogleCredentials(credentialsJson);
        return credentials != null ? BigQueryClient.Create(projectId, credentials) : BigQueryClient.Create(projectId);
    }
    
    /// <summary>
    /// Creates a client for use with the Identity API for Group management
    /// </summary>
    public static CloudIdentityService GetCloudIdentityService(CredentialsJson credentialsJson)
    {
        GoogleCredential credentials = GetGoogleCredentials(credentialsJson);
        return credentials != null
            ? new CloudIdentityService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = "Decisions.GoogleCloud",
            })
            : new CloudIdentityService(new BaseClientService.Initializer
            {
                ApplicationName = "Decisions.GoogleCloud",
            });
    }

    /// <summary>
    ///  Creates a client for use with the Admin API for User management
    /// </summary>
    public static DirectoryService GetAdminDirectoryClient(CredentialsJson credentialsJson)
    {
        GoogleCredential credentials = GetGoogleCredentials(credentialsJson);
        return credentials != null
            ? new DirectoryService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = "Decisions.GoogleCloud",
            })
            : new DirectoryService(new BaseClientService.Initializer
            {
                ApplicationName = "Decisions.GoogleCloud",
            });
    }
    
    /// <summary>
    /// Returns a Decisions GoogleCloud Credentials object by name.
    /// These are created in System > Settings.
    /// </summary>
    public static CredentialsJson? GetCredentialsByName(string credentialsName)
    {
        GoogleCloudSettings settings = ModuleSettingsAccessor<GoogleCloudSettings>.Instance;
        return settings.Credentials?.FirstOrDefault(entry => entry.DisplayName == credentialsName);
    }

    /// <summary>
    /// Creates GoogleCredentials based on Decisions GoogleCloud Credentials object.
    /// </summary>
    private static GoogleCredential? GetGoogleCredentials(CredentialsJson credentialsJson)
    {
        GoogleCloudSettings settings = ModuleSettingsAccessor<GoogleCloudSettings>.GetSettings();
        if (!settings.UseJsonFile)
            return null;
            
        if (credentialsJson?.JsonFile.Contents == null || credentialsJson.JsonFile.Contents.Length == 0)
            throw new ArgumentNullException(nameof(credentialsJson), ErrorStringConstants.JsonNotConfigured);
            
        // Authentication uses Json File to log in service account.
        string contents = Encoding.UTF8.GetString(credentialsJson.JsonFile.Contents, 0, credentialsJson.JsonFile.Contents.Length);
        if (string.IsNullOrEmpty(contents))
        {
            throw new ArgumentNullException(nameof(CredentialsJson),
                ErrorStringConstants.JsonNotConfigured);
        }
        GoogleCredential credential = GoogleCredential.FromJson(contents);
        credential.CreateWithHttpClientFactory(new HttpClientFactory());
        return credential;

    }
    
    #endregion
}