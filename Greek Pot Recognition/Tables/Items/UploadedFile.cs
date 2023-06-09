﻿using System;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Greek_Pot_Recognition.Tables.Items
{
	public class UploadedFile
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("fileId")]
        public string? FileID { get; set; }

        [BsonElement("resultList")]
        public IList<PredictionModel>? FileResult { get; set; }

        [BsonElement("uploadGUID")]
        public string? UploadGuid { get; set; }

        [BsonElement("mime")]
        public string? MimeType { get; set; }
    }
}

