using System.Collections.Generic;
using System.Net;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google;

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
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(bool)), OUTPUT_HAS_MEMBER, false, true, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        string groupKey = data[INPUT_GROUP_KEY] as string;
        string userKey = data[INPUT_USER_KEY] as string;
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        var resp = HasMember(credentials, groupKey, userKey);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_HAS_MEMBER, resp}
        });
    }

    public static bool HasMember(CredentialsJson credentials, string groupKey, string userKey)
    {
        try
        {
            var client = GoogleCloudUtility.GetCloudIdentityService(credentials);
            var request = client.Groups.Memberships.Lookup(groupKey);
            request.MemberKeyId = userKey;
            request.MemberKeyNamespace = "identitysources";

            var resp = request.Execute();
            return resp != null;
        }
        catch (GoogleApiException e) when (e.HttpStatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}