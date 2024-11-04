using System.Linq;
using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Data.Groups;

[Writable, DataContract]
public class GoogleCloudAdminGroup
{
    [WritableValue, DataMember]
    public bool? AdminCreated { get; set; }
    
    [WritableValue, DataMember]
    public string[] Aliases { get; set; }
    
    [WritableValue, DataMember]
    public string Description { get; set; }
    
    [WritableValue, DataMember]
    public long DirectMembersCount { get; set; }
    
    [WritableValue, DataMember]
    public string Email { get; set; }
    
    [WritableValue, DataMember]
    public string Id { get; set; }
    
    [WritableValue, DataMember]
    public string Kind { get; set; }
    
    [WritableValue, DataMember]
    public string Name { get; set; }
    
    [WritableValue, DataMember]
    public string[] NonEditableAliases { get; set; }
    
    public static GoogleCloudAdminGroup FromGroup(Group group)
    {
        return new GoogleCloudAdminGroup()
        {
            AdminCreated = group.AdminCreated,
            Aliases = group.Aliases?.ToArray(),
            Description = group.Description,
            DirectMembersCount = group.DirectMembersCount ?? 0,
            Email = group.Email,
            Id = group.Id,
            Kind = group.Kind,
            Name = group.Name,
            NonEditableAliases = group.NonEditableAliases?.ToArray(),
        };
    }

    public Group ToGroup()
    {
        return new Group()
        {
            AdminCreated = AdminCreated,
            Aliases = Aliases,
            Description = Description,
            DirectMembersCount = DirectMembersCount,
            Email = Email,
            Id = Id,
            Kind = Kind,
            Name = Name,
            NonEditableAliases = NonEditableAliases.ToArray(),
        };
    }
}