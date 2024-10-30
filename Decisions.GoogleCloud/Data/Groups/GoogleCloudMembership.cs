using System.Linq;
using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.CloudIdentity.v1.Data;

namespace Decisions.GoogleCloud.Data.Groups;

[Writable, DataContract]
public class GoogleCloudMembership
{
    [WritableValue, DataMember]
    public string Name { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }
    
    [WritableValue, DataMember]
    public GoogleCloudEntityKey PreferredMemberKey { get; set; }
    
    [WritableValue, DataMember]
    public GoogleCloudMembershipRole[] Roles { get; set; }
    
    [WritableValue, DataMember]
    public string CreatedOn { get; set; }
    
    [WritableValue, DataMember]
    public string UpdatedOn { get; set; }

    public static GoogleCloudMembership FromMembership(Membership membership)
    {
        return new GoogleCloudMembership()
        {
            Name = membership.Name,
            ETag = membership.ETag,
            PreferredMemberKey = GoogleCloudEntityKey.FromEntityKey(membership.PreferredMemberKey),
            Roles = membership.Roles.Select(GoogleCloudMembershipRole.FromMembershipRole).ToArray(),
            CreatedOn = membership.CreateTimeRaw,
            UpdatedOn = membership.UpdateTimeRaw,
        };
    }

    public Membership ToMembership()
    {
        return new Membership()
        {
            Name = Name,
            ETag = ETag,
            PreferredMemberKey = PreferredMemberKey.ToEntityKey(),
            Roles = Roles.Select(m => m.ToMembershipRole()).ToArray(),
        };
    }
}