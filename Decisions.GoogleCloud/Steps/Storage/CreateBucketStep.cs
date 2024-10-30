using System.Collections.Generic;
using Decisions.GoogleCloud.Steps;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;

namespace Decisions.GoogleCloud.Storage;

[Writable]
[AutoRegisterStep("Create Bucket", Constants.STEP_CATEGORY_STORAGE)]
public class CreateBucketStep : BaseCredentialsStep
{
    private const string INPUT_BUCKET_NAME = "Bucket Name";
    private const string INPUT_PROJECT_ID = "Project Id";
    private const string OUTPUT_NEW_BUCKET_NAME = "New Bucket Name";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_BUCKET_NAME, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_PROJECT_ID, false, false, false)
    ];

    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), OUTPUT_NEW_BUCKET_NAME, false, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        string bucketName = data[INPUT_BUCKET_NAME] as string;
        string projectId = data[INPUT_PROJECT_ID] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        string newBucketName = CreateBucket(credentials, projectId, bucketName);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_NEW_BUCKET_NAME, newBucketName},
        });
    }
    
    public static string CreateBucket(CredentialsJson credentialsJson, string projectId, string bucketName)
    {
        StorageClient client = GoogleCloudUtility.GetStorageClient(credentialsJson);
        Bucket bucket = client.CreateBucket(projectId, bucketName);
        return bucket.Name;
    }
}