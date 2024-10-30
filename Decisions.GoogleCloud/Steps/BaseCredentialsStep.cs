using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DecisionsFramework.Annotations;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;

namespace Decisions.GoogleCloud.Steps;

public abstract class BaseCredentialsStep : ISyncStep, IDataConsumer, INotifyPropertyChanged
{
    private string credentials;
        
    [WritableValue]
    [SelectStringEditor(nameof(CredentialsNames))]
    [PropertyClassification(0, "JSON Credentials", "Settings")]
    public string Credentials
    {
        get => credentials;
        set
        {
            credentials = value;
            OnPropertyChanged();
        }
    }
    
    [PropertyHidden(true)]
    public string[] CredentialsNames
    {
        get
        {
            GoogleCloudSettings defaultSettings = ModuleSettingsAccessor<GoogleCloudSettings>.Instance;
            return defaultSettings.Credentials?.Select(c => c.DisplayName).ToArray();
        }
    }

    #region INotifyPropertyChanged members
    
    public event PropertyChangedEventHandler PropertyChanged;
        
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    #endregion
    
    public abstract OutcomeScenarioData[] OutcomeScenarios { get; }
    public abstract ResultData Run(StepStartData data);
    public abstract DataDescription[] InputData { get; }
}