using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DecisionsFramework;
using DecisionsFramework.Data.DataTypes;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Properties;

namespace Decisions.GoogleCloud
{
    [DataContract]
    [Writable]
    public class CredentialsJson : INotifyPropertyChanged, IValidationSource
    {
        [WritableValue]
        private string displayName;

        [DataMember]
        [PropertyClassification(0, "Display Name", "Credentials")]
        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
                OnPropertyChanged();
            }
        }

        [ORMField(typeof(ORMBinaryFieldConverter))]
        private FileData jsonFile;
        
        [DataMember]
        [PropertyClassification(1, "JSON File", "Credentials")]
        public FileData JsonFile
        {
            get { return jsonFile; }
            set
            {
                jsonFile = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public ValidationIssue[] GetValidationIssues()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();

            if (displayName == null )
            {
                issues.Add(new ValidationIssue("Credentials must have a display name."));
            }
            
            if (jsonFile == null )
            {
                issues.Add(new ValidationIssue("Credentials must have a JSON File."));
            }

            return issues.ToArray();
        }
        
        public override string ToString()
        {
            return displayName;
        }
    }
}