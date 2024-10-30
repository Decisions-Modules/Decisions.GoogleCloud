using System.IO;
using Decisions.GoogleCloud.Steps;
using DecisionsFramework.Data.DataTypes;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Cloud.Storage.V1;

namespace Decisions.GoogleCloud.Storage;

[Writable]
[AutoRegisterStep("Upload Object", Constants.STEP_CATEGORY_STORAGE)]
public class UploadObjectStep : BaseCredentialsStep
{
    private const string INPUT_BUCKET_NAME = "Bucket Name";
    private const string INPUT_OBJECT_NAME = "Object Name";
    private const string INPUT_FILE = "File";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_BUCKET_NAME, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_OBJECT_NAME, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(FileData)), INPUT_FILE, false, false, false)
    ];

    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE)
    ];

    public override ResultData Run(StepStartData data)
    {
        string bucketName = data[INPUT_BUCKET_NAME] as string;
        string objectName = data[INPUT_OBJECT_NAME] as string;
        FileData fileData = data[INPUT_FILE] as FileData;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        UploadObject(credentials, bucketName, objectName, fileData);

        return new ResultData(PATH_DONE);
    }
    
    public static void UploadObject(CredentialsJson credentialsJson, string bucketName, string objectName, FileData fileData)
    {
        StorageClient client = GoogleCloudUtility.GetStorageClient(credentialsJson);
        using MemoryStream memoryStream = new MemoryStream(fileData.Contents);
        client.UploadObject(bucketName, objectName, null, memoryStream);
    }
}