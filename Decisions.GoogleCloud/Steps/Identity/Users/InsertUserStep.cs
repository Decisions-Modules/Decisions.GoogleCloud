using System.Collections.Generic;
using Decisions.GoogleCloud.Data.Users;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;

namespace Decisions.GoogleCloud.Steps.Identity.Users;

[Writable]
[AutoRegisterStep("Create User", Constants.STEP_CATEGORY_ADMIN)]
public class InsertUserStep : BaseCredentialsStep
{
    private const string INPUT_USER_DATA = "New User";
    private const string OUTPUT_NEW_USER = "New User";
    private const string PATH_DONE = "Done";

    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(GoogleCloudUser)), INPUT_USER_DATA, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(GoogleCloudUser)), OUTPUT_NEW_USER, false, true, false)})
    ];
    
    public override ResultData Run(StepStartData data)
    {
        GoogleCloudUser newUser = data[INPUT_USER_DATA] as GoogleCloudUser;
        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        
        var user = CreateUser(credentials, newUser);

        return new ResultData(PATH_DONE, new Dictionary<string, object>()
        {
            {OUTPUT_NEW_USER, user}   
        });
    }

    public static GoogleCloudUser CreateUser(CredentialsJson credentials, GoogleCloudUser newUser)
    {
        var client = GoogleCloudUtility.GetAdminDirectoryClient(credentials);
        var resp = client.Users.Insert(newUser.ToUser()).Execute();

        return GoogleCloudUser.FromUser(resp);
    }
}