using System.Collections.Generic;
using System.Linq;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Apis.CloudIdentity.v1.Data;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("Update Group Membership", Constants.STEP_CATEGORY_IDENTITY)]
public class UpdateGroupMembershipStep : BaseCredentialsStep
{
    private const string INPUT_USER_KEY = "User Key";
    private const string INPUT_GROUP_KEY = "Group Key";
    private const string INPUT_ROLE = "New Role";
    private const string OUTPUT_SUCCESS = "Success";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_USER_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_GROUP_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_ROLE, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(bool)), OUTPUT_SUCCESS, false, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        string userKey = data.Data[INPUT_USER_KEY] as string;
        string groupKey = data.Data[INPUT_GROUP_KEY] as string;
        string role = data.Data[INPUT_ROLE] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        var success = UpdateGroupMembership(credentials, userKey, groupKey, role);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_SUCCESS, success}
        });
    }

    public static bool UpdateGroupMembership(CredentialsJson credentials, string userKey, string groupKey, string role)
    {
        var client = GoogleCloudUtility.GetCloudIdentityService(credentials);
        var groupResourceName = $"groups/{groupKey}";

        var listRequest = client.Groups.Memberships.List(groupResourceName);
        listRequest.PageSize = 100;
        
        var listResponse = listRequest.Execute();
        var membership = listResponse.Memberships?.FirstOrDefault(m => m.PreferredMemberKey.Id == userKey);

        if (membership == null)
            return false;

        var modifyRolesRequest = new ModifyMembershipRolesRequest()
        {
            UpdateRolesParams = new List<UpdateMembershipRolesParams>()
            {
                new UpdateMembershipRolesParams
                {
                    MembershipRole = new MembershipRole()
                    {
                        Name = role,
                    }
                }
            }
        };

        string membershipResourceName = membership.Name;
        var modifyRequest = client.Groups.Memberships.ModifyMembershipRoles(modifyRolesRequest, membershipResourceName).Execute();
        return true;
    }
}