using System;
using System.Collections.Generic;
using System.Linq;
using Decisions.GoogleCloud.Data.Users;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Apis.Admin.Directory.directory_v1;

namespace Decisions.GoogleCloud.Steps.Identity.Users;

[Writable]
[AutoRegisterStep("List Users", Constants.STEP_CATEGORY_ADMIN)]
public class ListUsersStep : BaseCredentialsStep
{
    private const string INPUT_DOMAIN = "Domain";
    private const string INPUT_QUERY = "Query";
    private const string INPUT_MAX_RESULTS = "Max Results";
    private const string OUTPUT_USERS = "Users";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_DOMAIN, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_QUERY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(int)), INPUT_MAX_RESULTS, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(GoogleCloudUser)), OUTPUT_USERS, true, true, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        string domain = data.Data[INPUT_DOMAIN] as string;
        string query = data.Data[INPUT_QUERY] as string;
        int maxResults = (int)data.Data[INPUT_MAX_RESULTS];

        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        var users = ListUsers(credentials, domain, query, maxResults);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_USERS, users}
        });
    }
    
    public static GoogleCloudUser[] ListUsers(CredentialsJson credentials, string domain, string query, int maxResults)
    {
        var client = GoogleCloudUtility.GetAdminDirectoryClient(credentials);
        var request = client.Users.List();
        request.Domain = domain;
        request.Query = query;
        request.MaxResults = maxResults;
        request.OrderBy = UsersResource.ListRequest.OrderByEnum.Email;
        var resp = request.Execute();
        return resp.UsersValue.Select(GoogleCloudUser.FromUser).ToArray();
    }
}