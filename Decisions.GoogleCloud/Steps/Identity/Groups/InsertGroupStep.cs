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
        new DataDescription(new DecisionsNativeType(typeof(string[])), INPUT_OVERRIDE_SCOPES, false, true, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_OVERRIDE_IMPERSONATE, false, true, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(bool)), OUTPUT_GROUP_SUCCESS, false, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        GoogleCloudGroup newGroup = data[INPUT_GROUP_DATA] as GoogleCloudGroup;
        string[] scopes = data.Data[INPUT_OVERRIDE_SCOPES] as string[];
        string impersonate  = data.Data[INPUT_OVERRIDE_IMPERSONATE] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        var success = CreateGroup(credentials, newGroup, scopes, impersonate);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_GROUP_SUCCESS, success}
        });
    }

    public static bool CreateGroup(CredentialsJson credentials, GoogleCloudGroup newGroup, string[] scopes = null, string impersonate = null)
    {
        var client = GoogleCloudUtility.GetCloudIdentityService(credentials, scopes, impersonate);
        var resp = client.Groups.Create(newGroup.ToGroup()).Execute();
        return resp.Done ?? false;
    }
}