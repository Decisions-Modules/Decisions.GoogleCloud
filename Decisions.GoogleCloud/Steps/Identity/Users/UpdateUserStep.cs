using System.Collections.Generic;
using Decisions.GoogleCloud.Data.Users;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;

namespace Decisions.GoogleCloud.Steps.Identity.Users;

[Writable]
[AutoRegisterStep("Update User", Constants.STEP_CATEGORY_ADMIN)]
public class UpdateUserStep : BaseCredentialsStep
{
    private const string INPUT_USER_KEY = "User Key";
    private const string INPUT_UPDATED_USER = "Updated User";
    private const string OUTPUT_USER = "User";
    private const string PATH_DONE = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_USER_KEY, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(GoogleCloudUser)), INPUT_UPDATED_USER, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(GoogleCloudUser)), OUTPUT_USER, false, true, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        string userKey = data[INPUT_USER_KEY] as string;
        GoogleCloudUser updatedUser = data[INPUT_UPDATED_USER] as GoogleCloudUser;

        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        var user = UpdateUser(credentials, userKey, updatedUser);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_USER, user}
        });
    }

    public static GoogleCloudUser UpdateUser(CredentialsJson credentials, string userKey, GoogleCloudUser updatedUser)
    {
        var client = GoogleCloudUtility.GetAdminDirectoryClient(credentials);
        var resp = client.Users.Update(updatedUser.ToUser(), userKey).Execute();
        return GoogleCloudUser.FromUser(resp);
    }
}