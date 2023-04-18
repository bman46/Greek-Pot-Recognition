﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Greek_Pot_Recognition.Tables.Items
{
	public class UploadedFile
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("file")]
        public string? FileBase64 { get; set; }

        [BsonElement("result")]
        public string? FileResult { get; set; }

        [BsonElement("uploadGUID")]
        public string? UploadGuid { get; set; }
    }
}
