using System.Collections.Generic;
using System.Linq;
using Decisions.GoogleCloud.Data.Groups;
using Decisions.GoogleCloud.Data.Users;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("List Group Members", Constants.STEP_CATEGORY_IDENTITY)]
public class ListGroupMembers : BaseCredentialsStep
{
    private const string INPUT_GROUP_KEY = "Group Key";
    private const string INPUT_DOMAIN = "Domain";
    private const string INPUT_MAX_RESULTS = "Max Results";
    private const string OUTPUT_MEMBERS = "Members";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_DOMAIN, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_GROUP_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(int)), INPUT_MAX_RESULTS, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(GoogleCloudMembership)), OUTPUT_MEMBERS, true, true, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        string groupKey = data[INPUT_GROUP_KEY] as string;
        string domain = data[INPUT_DOMAIN] as string;
        int maxResults = (int)data[INPUT_MAX_RESULTS];
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        var members = ListMembers(credentials, domain, groupKey, maxResults);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_MEMBERS, members}
        });
    }

    public static GoogleCloudMember[] ListMembers(CredentialsJson credentials, string domain, string groupKey, int maxResults)
    {
        DirectoryService client = GoogleCloudUtility.GetAdminDirectoryClient(credentials);
        var request = client.Groups.List();
        request.Domain = domain;
        request.MaxResults = maxResults;

        var groups = request.Execute();
        var targetGroup = groups.GroupsValue.FirstOrDefault(m => m.Id == groupKey);
        if (targetGroup == null)
            return [];
        
        var membersRequest = client.Members.List(targetGroup.Id);
        Members members = membersRequest.Execute();
        return members.MembersValue.Select(GoogleCloudMember.FromMember).ToArray();
    }
}