using System.Collections.Generic;
using Decisions.GoogleCloud.Data.Groups;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("Create Group", Constants.STEP_CATEGORY_IDENTITY)]
public class InsertGroupStep : BaseCredentialsStep
{
    private const string INPUT_GROUP_DATA = "New Group";
    private const string OUTPUT_GROUP_SUCCESS = "Success";
    private const string PATH_DONE  = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(GoogleCloudGroup)), INPUT_GROUP_DATA, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(bool)), OUTPUT_GROUP_SUCCESS, false, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        GoogleCloudGroup newGroup = data[INPUT_GROUP_DATA] as GoogleCloudGroup;
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);

        var success = CreateGroup(credentials, newGroup);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_GROUP_SUCCESS, success}
        });
    }

    public static bool CreateGroup(CredentialsJson credentials, GoogleCloudGroup newGroup)
    {
        var client = GoogleCloudUtility.GetCloudIdentityService(credentials);
        var resp = client.Groups.Create(newGroup.ToGroup()).Execute();
        return resp.Done ?? false;
    }
}