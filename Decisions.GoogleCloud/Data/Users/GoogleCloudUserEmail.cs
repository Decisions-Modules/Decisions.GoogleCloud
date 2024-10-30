using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Data.Users;

[Writable, DataContract]
public class GoogleCloudUserEmail
{
    [WritableValue, DataMember]
    public string Address { get; set; }
    
    [WritableValue, DataMember]
    public string CustomType { get; set; }
    
    [WritableValue, DataMember]
    public bool? Primary { get; set; }
    
    [WritableValue, DataMember]
    public string Type { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }

    public static GoogleCloudUserEmail FromUserEmail(UserEmail userEmail)
    {
        return new GoogleCloudUserEmail()
        {
            Address = userEmail.Address,
            CustomType = userEmail.CustomType,
            Primary = userEmail.Primary,
            Type = userEmail.Type,
            ETag = userEmail.ETag
        };
    }

    public UserEmail ToUserEmail()
    {
        return new UserEmail()
        {
            Address = Address,
            CustomType = CustomType,
            Primary = Primary,
            Type = Type,
            ETag = ETag
        };
    }
}