using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Data.Groups;

[Writable, DataContract]
public class GoogleCloudMember
{
    [WritableValue, DataMember]
    public string DeliverySettings { get; set; }
    
    [WritableValue, DataMember]
    public string Email { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }
    
    [WritableValue, DataMember]
    public string Id { get; set; }
    
    [WritableValue, DataMember]
    public string Kind { get; set; }
    
    [WritableValue, DataMember]
    public string Role { get; set; }
    
    [WritableValue, DataMember]
    public string Status { get; set; }
    
    [WritableValue, DataMember]
    public string Type { get; set; }
    
    public static GoogleCloudMember FromMember(Member member)
    {
        return new GoogleCloudMember
        {
            DeliverySettings = member.DeliverySettings,
            Email = member.Email,
            ETag = member.ETag,
            Id = member.Id,
            Kind = member.Kind,
            Role = member.Role,
            Status = member.Status,
            Type = member.Type
        };

    }

    public Member ToMember()
    {
        return new Member
        {
            DeliverySettings = DeliverySettings,
            Email = Email,
            ETag = ETag,
            Id = Id,
            Kind = Kind,
            Role = Role,
            Status = Status,
            Type = Type
        };

    }
}