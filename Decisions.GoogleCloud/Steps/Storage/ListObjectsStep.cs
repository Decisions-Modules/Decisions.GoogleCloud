using System.Collections.Generic;
using System.Linq;
using Decisions.GoogleCloud.Steps;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Cloud.Storage.V1;

namespace Decisions.GoogleCloud.Storage;

[Writable]
[AutoRegisterStep("List Objects", Constants.STEP_CATEGORY_STORAGE)]
public class ListObjectsStep : BaseCredentialsStep
{   
    private const string INPUT_BUCKET_NAME = "Bucket Name";

    private const string OUTPUT_OBJECTS = "Buckets";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_BUCKET_NAME, false, false, false),
    ];

    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), OUTPUT_OBJECTS, true, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        string bucketName = data[INPUT_BUCKET_NAME] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        string[] objectsList = ListObjects(credentials, bucketName);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_OBJECTS, objectsList},
        });
    }
    
    public static string[] ListObjects(CredentialsJson credentialsJson, string bucketName)
    {
        StorageClient client = GoogleCloudUtility.GetStorageClient(credentialsJson);
        var items = client.ListObjects(bucketName).ToList();
        return items.Select(i => i.Name).ToArray();
    }


}