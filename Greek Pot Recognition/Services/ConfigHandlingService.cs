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
    }
}

