using Decisions.GoogleCloud.Steps;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Cloud.Storage.V1;

namespace Decisions.GoogleCloud.Storage;

[Writable]
[AutoRegisterStep("Copy Object", Constants.STEP_CATEGORY_STORAGE)]
public class CopyObjectStep : BaseCredentialsStep
{
    private const string INPUT_SOURCE_BUCKET_NAME = "Source Bucket Name";
    private const string INPUT_SOURCE_OBJECT_NAME = "Source Object Name";
    private const string INPUT_DEST_BUCKET_NAME = "Destination Bucket Name";
    private const string INPUT_DEST_OBJECT_NAME = "Destination Object Name";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_SOURCE_BUCKET_NAME, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_SOURCE_OBJECT_NAME, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_DEST_BUCKET_NAME, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_DEST_OBJECT_NAME, false, false, false),
    ];

    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE)
    ];

    public override ResultData Run(StepStartData data)
    {
        string sourceBucketName = data[INPUT_SOURCE_BUCKET_NAME] as string;
        string sourceObjectName = data[INPUT_SOURCE_OBJECT_NAME] as string;
        string destinationBucketName = data[INPUT_DEST_BUCKET_NAME] as string;
        string destinationObjectName = data[INPUT_DEST_OBJECT_NAME] as string;

        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        CopyObject(credentials, sourceBucketName, sourceObjectName, destinationBucketName, destinationObjectName);

        return new ResultData(PATH_DONE);
    }
    
    public static void CopyObject(CredentialsJson credentialsJson, string sourceBucketName, string sourceObjectName, string destinationBucketName, string destinationObjectName)
    {
        StorageClient client = GoogleCloudUtility.GetStorageClient(credentialsJson);
        client.CopyObject(sourceBucketName, sourceObjectName, destinationBucketName, destinationObjectName);
    }
}