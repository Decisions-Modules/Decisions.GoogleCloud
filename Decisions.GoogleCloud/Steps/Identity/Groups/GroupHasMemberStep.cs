using System.Collections.Generic;
using System.Net;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google;
using Google.Apis.CloudIdentity.v1;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("Group Has Member", Constants.STEP_CATEGORY_IDENTITY)]
public class GroupHasMemberStep : BaseCredentialsStep
{
    private const string INPUT_GROUP_KEY = "Group Key";
    private const string INPUT_USER_KEY = "User Key";
    private const string OUTPUT_HAS_MEMBER = "Has Member";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_GROUP_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_USER_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string[])), INPUT_OVERRIDE_SCOPES, false, true, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_OVERRIDE_IMPERSONATE, false, true, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(bool)), OUTPUT_HAS_MEMBER, false, false, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        string groupKey = data[INPUT_GROUP_KEY] as string;
        string userKey = data[INPUT_USER_KEY] as string;
        string[] scopes = data.Data[INPUT_OVERRIDE_SCOPES] as string[];
        string impersonate  = data.Data[INPUT_OVERRIDE_IMPERSONATE] as string;
        
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        bool resp = HasMember(credentials, groupKey, userKey, scopes, impersonate);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_HAS_MEMBER, resp}
        });
    }

    public static bool HasMember(CredentialsJson credentials, string groupKey, string userKey, string[] scopes = null, string impersonate = null)
    {
        try
        {
            CloudIdentityService client = GoogleCloudUtility.GetCloudIdentityService(credentials,
                scopes ?? new string[] { "https://www.googleapis.com/auth/admin.directory.group.member" }, impersonate);
            var fullGroupKey = $"groups/{groupKey}";
            var request = client.Groups.Memberships.Lookup(fullGroupKey);
            request.MemberKeyId = userKey;

            var resp = request.Execute();
            return resp != null;
        }
        catch (GoogleApiException e) when (e.HttpStatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}