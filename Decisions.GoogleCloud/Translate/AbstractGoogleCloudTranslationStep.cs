using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decisions.GoogleCloud.Data;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.CoreSteps;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translate.V3;

namespace Decisions.GoogleCloud.Translate
{
    [Writable]
    public abstract class AbstractGoogleCloudTranslationStep : BaseFlowAwareStep, ISyncStep, IDataConsumer
    {
        private const string INPUT_PROJECT_ID = "Project ID";
        private const string INPUT_REGION = "Region";
       
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
        
        [PropertyHidden]
        public string[] AllowedLanguages {
            get => new[] {
                "af", "sq", "ar", "hy", "az", "eu", "be", "bn", "bs", "bg", "ca", "ceb", "ny", "zh-TW", "hr",
                "cs", "da", "nl", "en", "eo", "et", "tl", "fi", "fr", "gl", "ka", "de", "el", "gu", "ht", "ha",
                "iw", "hi", "hmn", "hu", "is", "ig", "id", "ga", "it", "ja", "jw", "kn", "kk", "km", "ko", "lo",
                "la", "lv", "lt", "mk", "mg", "ms", "ml", "mt", "mi", "mr", "mn", "my", "ne", "no", "fa", "pl",
                "pt", "ro", "ru", "sr", "st", "si", "sk", "sl", "so", "es", "su", "sw", "sv", "tg", "ta", "te",
                "th", "tr", "uk", "ur", "uz", "vi", "cy", "yi", "yo", "zu" };
            set
            {
                // Do Nothing
            }
        }

        internal string GetProjectId(StepStartData data)
        {
            return data[INPUT_PROJECT_ID] as string;
        }
        
        internal string GetRegion(StepStartData data)
        {
            return data[INPUT_REGION] as string;
        }
        
        internal TranslationServiceClient GetTranslationServiceClient(StepStartData data)
        {
           GoogleCloudSettings settings = ModuleSettingsAccessor<GoogleCloudSettings>.Instance;
            CredentialsJson credentialsJson = settings.Credentials?.FirstOrDefault(entry => entry.DisplayName == Credentials);
            if (credentialsJson?.JsonFile.Contents == null || credentialsJson.JsonFile.Contents.Length == 0)
                throw new ArgumentNullException(nameof(CredentialsJson), ErrorStringConstants.JsonNotConfigured);

            // Authentication uses Json File to log in service account.
            var contents = Encoding.UTF8.GetString(credentialsJson.JsonFile.Contents, 0,
                credentialsJson.JsonFile.Contents.Length);
            if (string.IsNullOrEmpty(contents))
                throw new ArgumentNullException(nameof(CredentialsJson),
                    ErrorStringConstants.JsonNotConfigured);
            
            string endpoint = $"translate.googleapis.com:443";
            
            TranslationServiceClientBuilder b = new TranslationServiceClientBuilder();
            b.Endpoint = endpoint;
            b.GoogleCredential = GoogleCredential.FromJson(contents);
        
            TranslationServiceClient newClient = b.Build();
            return newClient;
        }
        
        public DataDescription[] InputData 
        {
            get
            {
                List<DataDescription> data = new List<DataDescription>
                {
                    new (new DecisionsNativeType(typeof(string)), INPUT_PROJECT_ID, false, false, false) { Categories = new[] {"Settings"}},
                    new (new DecisionsNativeType(typeof(string)), INPUT_REGION, false, false, false) { Categories = new[] {"Settings"}},
                };

                data.AddRange(GetAdditionalInputs());
                
                return data.ToArray();
            }
        }

        internal abstract IEnumerable<DataDescription> GetAdditionalInputs();

        public override OutcomeScenarioData[] OutcomeScenarios { get; }

        public abstract ResultData Run(StepStartData data);
    }
}