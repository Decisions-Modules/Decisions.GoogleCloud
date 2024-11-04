using System.Collections.Generic;
using Decisions.GoogleCloud.Data.Groups;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("Get Group", Constants.STEP_CATEGORY_ADMIN)]
public class GetGroupStep : BaseCredentialsStep
{
    private const string INPUT_GROUP_ID = "Group Id";
    private const string OUTPUT_GROUP = "Group";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_GROUP_ID, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(GoogleCloudAdminGroup)), OUTPUT_GROUP, false, true, false)})
    ];
 
    public override ResultData Run(StepStartData data)
    {
        string groupId = data.Data[INPUT_GROUP_ID] as string;

        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        var group = GetGroup(credentials, groupId);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_GROUP, group}
        });
    }

    public static GoogleCloudAdminGroup GetGroup(CredentialsJson credentials, string groupId)
    {
        DirectoryService client = GoogleCloudUtility.GetAdminDirectoryClient(credentials,
            new string[] { "https://www.googleapis.com/auth/admin.directory.group" });
        Group response = client.Groups.Get(groupId).Execute();

        return GoogleCloudAdminGroup.FromGroup(response);
    }
}