using System.Collections.Generic;
using Decisions.GoogleCloud.Data.Groups;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Apis.CloudIdentity.v1;
using Google.Apis.CloudIdentity.v1.Data;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("Get Group User Details", Constants.STEP_CATEGORY_IDENTITY)]
public class GetGroupUserDetailsStep : BaseCredentialsStep
{
    private const string INPUT_USER_KEY = "User Key";
    private const string INPUT_GROUP_KEY = "Group Key";
    private const string OUTPUT_MEMBERSHIP = "Membership";
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
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(GoogleCloudMembership)), OUTPUT_MEMBERSHIP, false, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        string userKey = data.Data[INPUT_USER_KEY] as string;
        string groupKey = data.Data[INPUT_GROUP_KEY] as string;
        string[] scopes = data.Data[INPUT_OVERRIDE_SCOPES] as string[];
        string impersonate  = data.Data[INPUT_OVERRIDE_IMPERSONATE] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        var membership = GetGroupUserDetails(credentials, userKey, groupKey, scopes, impersonate);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_MEMBERSHIP, membership}
        });
    }

    public static GoogleCloudMembership GetGroupUserDetails(CredentialsJson credentials, string userKey, string groupKey, string[] scopes = null, string impersonate = null)
    {
        CloudIdentityService client = GoogleCloudUtility.GetCloudIdentityService(credentials, scopes, impersonate);
        string groupResourceName = $"groups/{groupKey}";

        var lookupRequest = client.Groups.Memberships.Lookup(groupResourceName);
        lookupRequest.MemberKeyId = userKey;
        lookupRequest.MemberKeyNamespace = "identitysources";
        
        var lookupResponse = lookupRequest.Execute();
        string membershipName = lookupResponse.Name;
        Membership getRequest = client.Groups.Memberships.Get(membershipName).Execute();
        
        return GoogleCloudMembership.FromMembership(getRequest);
    }
}