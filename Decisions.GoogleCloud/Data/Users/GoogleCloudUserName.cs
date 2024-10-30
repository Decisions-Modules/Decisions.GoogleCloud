using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Data.Users;

[Writable, DataContract]
public class GoogleCloudUserName
{
    [WritableValue, DataMember]
    public string FamilyName { get; set; }
    
    [WritableValue, DataMember]
    public string FullName { get; set; }
    
    [WritableValue, DataMember]
    public string GivenName { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }

    public static GoogleCloudUserName FromUserName(UserName userName)
    {
        return new GoogleCloudUserName()
        {
            FamilyName = userName.FamilyName,
            FullName = userName.FullName,
            GivenName = userName.GivenName,
            ETag = userName.ETag
        };
    }

    public UserName ToUserName()
    {
        return new UserName()
        {
            FamilyName = FamilyName,
            FullName = FullName,
            GivenName = GivenName,
            ETag = ETag
        };
    }
}