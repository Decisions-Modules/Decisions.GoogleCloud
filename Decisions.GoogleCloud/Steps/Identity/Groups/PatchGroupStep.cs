using System.Collections.Generic;
using Decisions.GoogleCloud.Data.Groups;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("Update Group", Constants.STEP_CATEGORY_IDENTITY)]
public class PatchGroupStep : BaseCredentialsStep
{
    private const string INPUT_GROUP_KEY = "Group Key";
    private const string INPUT_UPDATED_GROUP = "Updated Group";
    private const string OUTPUT_GROUP_SUCCESS = "Success";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_GROUP_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(GoogleCloudGroup)), INPUT_UPDATED_GROUP, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(bool)), OUTPUT_GROUP_SUCCESS, false, true, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        string groupKey = data[INPUT_GROUP_KEY] as string;
        GoogleCloudGroup updatedGroup = data[INPUT_UPDATED_GROUP] as GoogleCloudGroup;

        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        var success = UpdateGroup(credentials, groupKey, updatedGroup);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_GROUP_SUCCESS, success}
        });
    }

    public static bool UpdateGroup(CredentialsJson credentials, string groupKey, GoogleCloudGroup updatedGroup)
    {
        var client = GoogleCloudUtility.GetCloudIdentityService(credentials);
        var result = client.Groups.Patch(updatedGroup.ToGroup(), groupKey).Execute();
        return result.Done ?? false;
    }
}