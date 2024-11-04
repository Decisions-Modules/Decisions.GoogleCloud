using System.Collections.Generic;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Apis.CloudIdentity.v1;
using Google.Apis.CloudIdentity.v1.Data;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("Add User To Group", Constants.STEP_CATEGORY_IDENTITY)]
public class AddUserToGroupStep : BaseCredentialsStep
{
    private const string INPUT_USER_KEY = "User Key";
    private const string INPUT_GROUP_KEY = "Group Key";
    private const string INPUT_ROLE = "Role";
    private const string OUTPUT_SUCCESS = "Success";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_USER_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_GROUP_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_ROLE, false, false, false),
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
        string role = data.Data[INPUT_ROLE] as string;
        string[] scopes = data.Data[INPUT_OVERRIDE_SCOPES] as string[];
        string impersonate  = data.Data[INPUT_OVERRIDE_IMPERSONATE] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        bool success = AddUserToGroup(credentials, userKey, groupKey, role, scopes, impersonate);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_SUCCESS, success}
        });
    }

    public static bool AddUserToGroup(CredentialsJson credentials, string userKey, string groupKey, string role, string[] scopes = null, string impersonate = null)
    {
        CloudIdentityService client = GoogleCloudUtility.GetCloudIdentityService(credentials, 
            scopes ?? new [] { "https://www.googleapis.com/auth/admin.directory.group.member" }, impersonate);
        var groupResourceName = $"groups/{groupKey}";

        var membership = new Membership()
        {
            PreferredMemberKey = new EntityKey()
            {
                Id = userKey,
            },
            Roles = new List<MembershipRole>()
            {
                new MembershipRole() { Name = role }
            }
        };

        var request = client.Groups.Memberships.Create(membership, groupResourceName);
        var response = request.Execute();
        return response.Done ?? false;
    }
}