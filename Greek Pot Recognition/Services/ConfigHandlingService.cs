using System;
namespace Greek_Pot_Recognition.Services
{
    /// <summary>
    /// Stores all of the configurable variables.
    /// </summary>
    public class ConfigHandlingService
    {
        /// <summary>
        /// The mongo DB connection string
        /// </summary>
        private readonly string? _MongoDBConnectionString;
        private readonly string? _Endpoint;
        private readonly string? _Key;
        private readonly string? _ProjectId;
        private readonly string? _ProjectName;


        /// <summary>
        /// Initialize the secrets:
        /// </summary>
        /// <returns></returns>
        public ConfigHandlingService()
        {
            // Load the secrets:
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

            // Get and set the MongoDBConnectionString:
            _MongoDBConnectionString = (config["MongoDBConnectionString"] == null) ? (Environment.GetEnvironmentVariable("MongoDBConnectionString")) : (config["MongoDBConnectionString"]);

            // Azure Custom Vision:
            _Endpoint = (config["ENDPOINT"] == null) ? (Environment.GetEnvironmentVariable("ENDPOINT")) : (config["ENDPOINT"]);
            _Key = (config["KEY"] == null) ? (Environment.GetEnvironmentVariable("KEY")) : (config["KEY"]);
            _ProjectId = (config["PROJECTID"] == null) ? (Environment.GetEnvironmentVariable("PROJECTID")) : (config["PROJECTID"]);
            _ProjectName = (config["PROJECTNAME"] == null) ? (Environment.GetEnvironmentVariable("PROJECTNAME")) : (config["PROJECTNAME"]);
        }

        /// <summary>
        /// The MongoDB connection string
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown if the connection string is not set</exception>
        public string MongoDBConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_MongoDBConnectionString))
                {
                    throw new NullReferenceException("The MongoDB connection string is not set.");
                }
                return _MongoDBConnectionString;
            }
        }
        public string Endpoint
        {
            get
            {
                if (string.IsNullOrEmpty(_Endpoint))
                {
                    throw new NullReferenceException("The endpoint connection string is not set.");
                }
                return _Endpoint;
            }
        }
        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(_Key))
                {
                    throw new NullReferenceException("The key is not set.");
                }
                return _Key;
            }
        }
        public string ProjectId
        {
            get
            {
                if (string.IsNullOrEmpty(_ProjectId))
                {
                    throw new NullReferenceException("The project ID is not set.");
                }
                return _ProjectId;
            }
        }
        public string ProjectName
        {
            get
            {
                if (string.IsNullOrEmpty(_ProjectName))
                {
                    throw new NullReferenceException("The project name is not set.");
                }
                return _ProjectName;
            }
        }
    }
}

