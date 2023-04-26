using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace Greek_Pot_Recognition.Services.ML
{
	public class ImageRecognizer
	{
        private readonly string _endpoint;
        private readonly string _key;
        private readonly Guid _projectId;
        private readonly string _projectName;

        public ImageRecognizer(string endpoint, string key, string projectId, string projectName)
        {
            _endpoint = endpoint;
            _key = key;
            _projectId = new Guid(projectId);
            _projectName = projectName;
        }

        public IList<PredictionModel> Classify(Stream stream)
        {
            var client = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(_key))
            {
                Endpoint = _endpoint
            };

            var result = client.ClassifyImage(_projectId, _projectName, stream);

            return result.Predictions;
        }
    }
}