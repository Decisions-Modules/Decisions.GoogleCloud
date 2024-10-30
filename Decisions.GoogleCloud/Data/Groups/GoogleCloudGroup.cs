using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.CloudIdentity.v1.Data;

namespace Decisions.GoogleCloud.Data.Groups;

[Writable, DataContract]
public class GoogleCloudGroup
{
    [WritableValue, DataMember]
    public string CreatedOn { get; set; }
    
    [WritableValue, DataMember]
    public string Description { get; set; }
    
    [WritableValue, DataMember]
    public string DisplayName { get; set; }
    
    [WritableValue, DataMember]
    public GoogleCloudEntityKey GroupKey { get; set; }
    
    [WritableValue, DataMember]
    public string Name { get; set; }
    
    [WritableValue, DataMember]
    public string Parent { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }
    
    public static GoogleCloudGroup FromGroup(Group group)
    {
        return new GoogleCloudGroup()
        {
            CreatedOn = group.CreateTimeRaw,
            Description = group.Description,
            DisplayName = group.DisplayName,
            GroupKey = GoogleCloudEntityKey.FromEntityKey(group.GroupKey),
            Name = group.Name,
            Parent = group.Parent,
            ETag = group.ETag
        };
    }

    public Group ToGroup()
    {
        return new Group()
        {
            Description = Description,
            DisplayName = DisplayName,
            GroupKey = GroupKey.ToEntityKey(),
            Name = Name,
            Parent = Parent,
            ETag = ETag
        };
    }
    
}