using Decisions.GoogleCloud.Steps;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Cloud.Storage.V1;

namespace Decisions.GoogleCloud.Storage;

[Writable]
[AutoRegisterStep("Delete Bucket", Constants.STEP_CATEGORY_STORAGE)]
public class DeleteBucketStep : BaseCredentialsStep
{
    private const string INPUT_BUCKET_NAME = "Bucket Name";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_BUCKET_NAME, false, false, false),
    ];

    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE)
    ];

    public override ResultData Run(StepStartData data)
    {
        string bucketName = data[INPUT_BUCKET_NAME] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        DeleteBucket(credentials, bucketName);

        return new ResultData(PATH_DONE);
    }
    
    public static void DeleteBucket(CredentialsJson credentialsJson, string bucketName)
    {
        StorageClient client = GoogleCloudUtility.GetStorageClient(credentialsJson);
        client.DeleteBucket(bucketName);
    }
}