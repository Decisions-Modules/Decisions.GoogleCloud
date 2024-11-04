using System.Collections.Generic;
using System.Linq;
using Decisions.GoogleCloud.Data.Groups;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Apis.Admin.Directory.directory_v1;

namespace Decisions.GoogleCloud.Steps.Identity.Groups;

[Writable]
[AutoRegisterStep("List Groups", Constants.STEP_CATEGORY_IDENTITY)]
public class ListGroupsStep : BaseCredentialsStep
{
    private const string INPUT_DOMAIN = "Domain";
    private const string OUTPUT_GROUPS = "Groups";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData => 
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_DOMAIN, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(GoogleCloudAdminGroup)), OUTPUT_GROUPS, true, true, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        var domain = data[INPUT_DOMAIN] as string;
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        GoogleCloudAdminGroup[] groups = ListGroups(credentials, domain);
        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_GROUPS, groups}
        });
    }

    public static GoogleCloudAdminGroup[] ListGroups(CredentialsJson credentials, string domain)
    {
        DirectoryService client = GoogleCloudUtility.GetAdminDirectoryClient(credentials,
            new string[] { "https://www.googleapis.com/auth/admin.directory.group" });
        var request = client.Groups.List();
        request.Domain = domain;
        
        var response = request.Execute();
        return response.GroupsValue.Select(GoogleCloudAdminGroup.FromGroup).ToArray();
    }
}