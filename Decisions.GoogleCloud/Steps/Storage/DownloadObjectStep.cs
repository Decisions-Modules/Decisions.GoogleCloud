using System.Collections.Generic;
using System.IO;
using Decisions.GoogleCloud.Steps;
using DecisionsFramework.Data.DataTypes;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Cloud.Storage.V1;

namespace Decisions.GoogleCloud.Storage;

[Writable]
[AutoRegisterStep("Download Object", Constants.STEP_CATEGORY_STORAGE)]
public class DownloadObjectStep : BaseCredentialsStep
{
    private const string INPUT_BUCKET_NAME = "Bucket Name";
    private const string INPUT_OBJECT_NAME = "Object Name";
    private const string OUTPUT_FILE = "File";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_BUCKET_NAME, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_OBJECT_NAME, false, false, false)
    ];

    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(FileData)), OUTPUT_FILE, false, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        string bucketName = data[INPUT_BUCKET_NAME] as string;
        string objectName = data[INPUT_OBJECT_NAME] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        FileData newBucketName = DownloadObject(credentials, bucketName, objectName);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_FILE, newBucketName},
        });
    }
    
    public static FileData DownloadObject(CredentialsJson credentialsJson, string bucketName, string objectName)
    {
        StorageClient client = GoogleCloudUtility.GetStorageClient(credentialsJson);
        using MemoryStream memOutput = new MemoryStream();
        client.DownloadObject(bucketName, objectName, memOutput);
        
        return new FileData(objectName, memOutput.ToArray());
    }
}