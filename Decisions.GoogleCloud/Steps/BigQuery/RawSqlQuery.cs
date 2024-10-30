using System.Collections.Generic;
using Decisions.GoogleCloud.Steps;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.CoreSteps.StandardSteps;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.StepImplementations;

namespace Decisions.GoogleCloud.BigQuery;

[Writable]
[AutoRegisterStep("Raw SQL Query", Constants.STEP_CATEGORY_BIGQUERY)]
[ShapeImageAndColorProvider(Constants.GCP_STEP_COLOR,Constants.STEP_ICON_BIG_QUERY)]
public class RawSqlQuery : BaseCredentialsStep
{
    private const string INPUT_PROJECT_ID = "Project ID";
    private const string INPUT_QUERY = "Query";
    private const string INPUT_USE_LEGACY = "Use Legacy SQL";
    private const string OUTPUT_QUERY_RESULTS = "Query Results";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new(new DecisionsNativeType(typeof(string)), INPUT_PROJECT_ID, false, false, false),
        new(new DecisionsNativeType(typeof(string)), INPUT_QUERY, false, false, false),
        new(new DecisionsNativeType(typeof(bool)), INPUT_USE_LEGACY, false, false, false)
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(DynamicDataRow[])), OUTPUT_QUERY_RESULTS)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        string projectId = data[INPUT_PROJECT_ID] as string;
        string query = data[INPUT_QUERY] as string;
        bool useLegacy = (bool) data[INPUT_USE_LEGACY];

        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        DynamicDataRow[] results = GoogleCloudUtility.RunQueryWithReturn(projectId, query, credentials, useLegacy);
        
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_QUERY_RESULTS, results},
        });
    }
}