using System.Collections.Generic;
using System.Linq;
using Decisions.GoogleCloud.Data.Groups;
using Decisions.GoogleCloud.Data.Users;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("List Group Members", Constants.STEP_CATEGORY_IDENTITY)]
public class ListGroupMembers : BaseCredentialsStep
{
    private const string INPUT_GROUP_KEY = "Group Key";
    private const string OUTPUT_MEMBERS = "Members";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_GROUP_KEY, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(GoogleCloudMembership)), OUTPUT_MEMBERS, true, true, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        string groupKey = data[INPUT_GROUP_KEY] as string;
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        var members = ListMembers(credentials, groupKey);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_MEMBERS, members}
        });
    }

    public static GoogleCloudMembership[] ListMembers(CredentialsJson credentials, string groupKey)
    {
        var client = GoogleCloudUtility.GetCloudIdentityService(credentials);

        var members = new List<GoogleCloudMembership>();
        var groupResourceId = $"groups/{groupKey}";
        string pageToken = null;

        do
        {
            var request = client.Groups.Memberships.List(groupResourceId);
            request.PageToken = pageToken;
            request.PageSize = 100;

            var response = request.Execute();
            if (response.Memberships != null)
            {
                members.AddRange(response.Memberships.Select(GoogleCloudMembership.FromMembership));
            }

            pageToken = response.NextPageToken;
        } while (pageToken != null);

        return members.ToArray();
    }
    
}