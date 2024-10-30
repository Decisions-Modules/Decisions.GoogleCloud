using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.CloudIdentity.v1.Data;

namespace Decisions.GoogleCloud.Data.Groups;

[Writable, DataContract]
public class GoogleCloudEntityKey
{
    [WritableValue, DataMember]
    public string Id { get; set; }
    
    [WritableValue, DataMember]
    public string Namespace { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }
    
    public static GoogleCloudEntityKey FromEntityKey(EntityKey entityKey)
    {
        return new GoogleCloudEntityKey()
        {
            Id = entityKey.Id,
            Namespace = entityKey.Namespace__,
            ETag = entityKey.ETag,
        };
    }

    public EntityKey ToEntityKey()
    {
        return new EntityKey()
        {
            Id = Id,
            Namespace__ = Namespace,
            ETag = ETag,
        };
    }
}