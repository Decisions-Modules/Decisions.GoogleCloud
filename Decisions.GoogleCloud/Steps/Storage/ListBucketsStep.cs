using System.Collections.Generic;
using System.Linq;
using Decisions.GoogleCloud.Steps;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Cloud.Storage.V1;

namespace Decisions.GoogleCloud.Storage;

[Writable]
[AutoRegisterStep("List Buckets", Constants.STEP_CATEGORY_STORAGE)]
public class ListBucketsStep : BaseCredentialsStep
{   
    private const string INPUT_PROJECT_ID = "Project Id";

    private const string OUTPUT_BUCKETS = "Buckets";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_PROJECT_ID, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_PROJECT_ID, false, false, false)
    ];

    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), OUTPUT_BUCKETS, true, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        string projectId = data[INPUT_PROJECT_ID] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        string[] bucketsList = ListBuckets(credentials, projectId);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_BUCKETS, bucketsList},
        });
    }
    
    public static string[] ListBuckets(CredentialsJson credentialsJson, string projectId)
    {
        StorageClient client = GoogleCloudUtility.GetStorageClient(credentialsJson);
        var buckets = client.ListBuckets(projectId).ToList();
        return buckets.Select(b => b.Name).ToArray();
    }
}