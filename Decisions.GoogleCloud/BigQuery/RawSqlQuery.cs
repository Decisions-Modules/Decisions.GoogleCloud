using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DecisionsFramework.Annotations;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.CoreSteps.StandardSteps;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;

namespace Decisions.GoogleCloud.BigQuery
{
    [Writable]
    [AutoRegisterStep("Raw SQL Query", "Integration", "Google Cloud", "BigQuery")]
    public class RawSqlQuery : ISyncStep, IDataConsumer, INotifyPropertyChanged
    {
        private const string INPUT_PROJECT_ID = "Project ID";
        private const string INPUT_QUERY = "Query";
        private const string INPUT_USE_LEGACY = "Use Legacy SQL";
        private const string OUTPUT_QUERY_RESULTS = "Query Results";
        private const string PATH_DONE = "Done";

        private string credentials;
        
        [WritableValue]
        [SelectStringEditor(nameof(CredentialsNames))]
        [PropertyClassification(0, "JSON Credentials", "Settings")]
        public string Credentials
        {
            get
            {
                return credentials;
            }
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
        
        public DataDescription[] InputData 
        {
            get
            {
                List<DataDescription> data = new List<DataDescription>
                {
                    new (new DecisionsNativeType(typeof(string)), INPUT_PROJECT_ID, false, false, false),
                    new (new DecisionsNativeType(typeof(string)), INPUT_QUERY, false, false, false),
                    new (new DecisionsNativeType(typeof(bool)), INPUT_USE_LEGACY, false, false, false)
                };
                
                return data.ToArray();
            }
        }

        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                OutcomeScenarioData outcomeScenarioData = new OutcomeScenarioData(PATH_DONE);
                outcomeScenarioData.AddOutputData(new DataDescription(new DecisionsNativeType(typeof(DynamicDataRow[])), OUTPUT_QUERY_RESULTS));
                return new []
                {
                    outcomeScenarioData
                };
            }
        }
        
        public ResultData Run(StepStartData data)
        {
            string projectId = data[INPUT_PROJECT_ID] as string;
            string query = data[INPUT_QUERY] as string;
            bool useLegacy = (bool) data[INPUT_USE_LEGACY];
            
            GoogleCloudSettings settings = ModuleSettingsAccessor<GoogleCloudSettings>.Instance;
            CredentialsJson credentialsJson = settings.Credentials?.FirstOrDefault(entry => entry.DisplayName == Credentials);

            DynamicDataRow[] results = GoogleCloudUtility.RunQueryWithReturn(projectId, query, credentialsJson, useLegacy);
            
            Dictionary<string, object> resultData = new Dictionary<string, object>
            {
                [OUTPUT_QUERY_RESULTS] = results
            };

            return new ResultData(PATH_DONE, resultData);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}