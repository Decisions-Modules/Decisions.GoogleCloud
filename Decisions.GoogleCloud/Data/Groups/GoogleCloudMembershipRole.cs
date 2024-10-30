using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.CloudIdentity.v1.Data;

namespace Decisions.GoogleCloud.Data.Groups;

[Writable, DataContract]
public class GoogleCloudMembershipRole
{
    [WritableValue, DataMember]
    public string Name { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }

    public static GoogleCloudMembershipRole FromMembershipRole(MembershipRole membershipRole)
    {
        return new GoogleCloudMembershipRole()
        {
            Name = membershipRole.Name,
            ETag = membershipRole.ETag
        };
    }

    public MembershipRole ToMembershipRole()
    {
        return new MembershipRole()
        {
            Name = Name,
            ETag = ETag,
        };
    }
}