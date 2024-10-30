using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Data.Users;

[Writable, DataContract]
public class GoogleCloudUserAddress
{
    [WritableValue, DataMember]
    public string Country { get; set; }
    
    [WritableValue, DataMember]
    public string CountryCode { get; set; }
    
    [WritableValue, DataMember]
    public string CustomType { get; set; }
    
    [WritableValue, DataMember]
    public string ExtendedAddress { get; set; }
    
    [WritableValue, DataMember]
    public string Formatted { get; set; }
    
    [WritableValue, DataMember]
    public string Locality { get; set; }
    
    [WritableValue, DataMember]
    public string PoBox { get; set; }
    
    [WritableValue, DataMember]
    public string Postalcode { get; set; }
    
    [WritableValue, DataMember]
    public bool? Primary { get; set; }
    
    [WritableValue, DataMember]
    public string Region { get; set; }
    
    [WritableValue, DataMember]
    public bool? SourceIsStructured { get; set; }
    
    [WritableValue, DataMember]
    public string StreetAddress { get; set; }
    
    [WritableValue, DataMember]
    public string Type { get; set; }
    
    [WritableValue, DataMember]
    public string ETag { get; set; }
    
    public static GoogleCloudUserAddress FromUserAddress(UserAddress userAddress)
    {
        return new GoogleCloudUserAddress()
        {
            Country = userAddress.Country,
            CountryCode = userAddress.CountryCode,
            CustomType = userAddress.CustomType,
            ExtendedAddress = userAddress.ExtendedAddress,
            Formatted = userAddress.Formatted,
            Locality = userAddress.Locality,
            PoBox = userAddress.PoBox,
            Postalcode = userAddress.PostalCode,
            Primary = userAddress.Primary,
            Region = userAddress.Region,
            SourceIsStructured = userAddress.SourceIsStructured,
            StreetAddress = userAddress.StreetAddress,
            Type = userAddress.Type,
            ETag = userAddress.ETag
        };
    }

    public UserAddress ToUserAddress()
    {
        return new UserAddress()
        {
            Country = Country,
            CountryCode = CountryCode,
            CustomType = CustomType,
            ExtendedAddress = ExtendedAddress,
            Formatted = Formatted,
            Locality = Locality,
            PoBox = PoBox,
            PostalCode = Postalcode,
            Primary = Primary,
            Region = Region,
            SourceIsStructured = SourceIsStructured,
            StreetAddress = StreetAddress,
            Type = Type,
            ETag = ETag
        };
    }
    
    
    
    
}