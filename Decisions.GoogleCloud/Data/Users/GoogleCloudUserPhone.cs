using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Data.Users;

[Writable, DataContract]
public class GoogleCloudUserPhone
{
    [WritableValue, DataMember]
    public string CustomType { get; set; }
    
    [WritableValue, DataMember]
    public bool? Primary { get; set; }
    
    [WritableValue, DataMember]
    public string Type { get; set; }
    
    [WritableValue, DataMember]
    public string Value { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }
    
    public static GoogleCloudUserPhone FromUserPhone(UserPhone userPhone)
    {
        return new GoogleCloudUserPhone()
        {
            CustomType = userPhone.CustomType,
            Primary = userPhone.Primary,
            Type = userPhone.Type,
            Value = userPhone.Value,
            ETag = userPhone.ETag
        };
    }

    public UserPhone ToUserPhone()
    {
        return new UserPhone()
        {
            CustomType = CustomType,
            Primary = Primary,
            Type = Type,
            Value = Value,
            ETag = ETag
        };
    }
}