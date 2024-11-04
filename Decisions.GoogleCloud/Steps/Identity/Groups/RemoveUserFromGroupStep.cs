using System.Collections.Generic;
using System.Linq;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("Remove User From Group", Constants.STEP_CATEGORY_IDENTITY)]
public class RemoveUserFromGroupStep : BaseCredentialsStep
{
    private const string INPUT_USER_KEY = "User Key";
    private const string INPUT_GROUP_KEY = "Group Key";
    private const string OUTPUT_SUCCESS = "Success";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_USER_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_GROUP_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string[])), INPUT_OVERRIDE_SCOPES, false, true, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_OVERRIDE_IMPERSONATE, false, true, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(bool)), OUTPUT_SUCCESS, false, false, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        string userKey = data.Data[INPUT_USER_KEY] as string;
        string groupKey = data.Data[INPUT_GROUP_KEY] as string;
        string[] scopes = data.Data[INPUT_OVERRIDE_SCOPES] as string[];
        string impersonate  = data.Data[INPUT_OVERRIDE_IMPERSONATE] as string;

        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        bool success = RemoveUserFromGroup(credentials, userKey, groupKey, scopes, impersonate);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_SUCCESS, success}
        });
    }

    public static bool RemoveUserFromGroup(CredentialsJson credentials, string userKey, string groupKey, string[] scopes = null, string impersonate = null)
    {
        var client = GoogleCloudUtility.GetCloudIdentityService(credentials, scopes, impersonate);
        var groupResourceName = $"groups/{groupKey}";
        var listRequest = client.Groups.Memberships.List(groupResourceName);
        listRequest.PageSize = 100;

        var listResponse = listRequest.Execute();
        var membership = listResponse.Memberships?.FirstOrDefault(m => m.PreferredMemberKey.Id == userKey);

        // not a member of group
        if (membership == null)
            return false;

        var membershipResourceName = membership.Name;
        var deleteRequest = client.Groups.Memberships.Delete(membershipResourceName).Execute();
        return true;
    }
    
    
}