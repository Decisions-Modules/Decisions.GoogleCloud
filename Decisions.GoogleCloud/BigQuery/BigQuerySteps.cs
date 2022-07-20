using System;
using System.Data;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.CoreSteps.StandardSteps;

namespace Decisions.GoogleCloud.BigQuery
{
        
    [AutoRegisterMethodsOnClass(true, "Integration/Google Cloud/BigQuery" )]
    public class BigQuerySteps
    {
        /// <summary>
        /// Execute a raw SQL query in BigQuery.
        /// </summary>
        /// <param name="projectId">BigQuery Project Id.</param>
        /// <param name="query">The SQL query to execute.</param>
        /// <returns></returns>
        public DynamicDataRow[] RawSqlQuery(string projectId, string query)
        {
            // Required parameters
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException(nameof(projectId), "Project Id must be specified.");
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query), "Query Id must be specified.");
            
            return GoogleCloudUtility.RunQueryWithReturn(projectId, query);
        }

        /// <summary>
        /// Execute a raw SQL query in BiqQuery, using Legacy SQL syntax.
        /// </summary>
        /// <param name="projectId">BigQuery Project Id.</param>
        /// <param name="query">The SQL query to execute.</param>
        /// <returns></returns>
        public DynamicDataRow[] RawLegacySqlQuery(string projectId, string query)
        {
            // Required parameters
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException(nameof(projectId), "Project Id must be specified.");
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query), "Query Id must be specified.");

            return GoogleCloudUtility.RunQueryWithReturn(projectId, query, true);
        }
        
    }
}
