using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greek_Pot_Recognition.Tables.Items;
using Greek_Pot_Recognition.Tables.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace Greek_Pot_Recognition.Pages
{
    public class resultsModel : PageModel
    {
        private IUploadRepository _UploadRepository;
        public UploadedFile? UserFile { get; set; }

        public resultsModel(IUploadRepository uploadRepository)
        {
            _UploadRepository = uploadRepository;
        }
        public async Task<ActionResult> OnGetAsync(string guid)
        {
            if (String.IsNullOrEmpty(guid))
            {
                return LocalRedirect("/");
            }
            UserFile = await _UploadRepository.GetByFileGuidAsync(guid);
            if(UserFile == null)
            {
                return LocalRedirect("/");
            }
            return Page();
        }
        public string PredictionString(IList<PredictionModel> predictions)
        {
            string tag = predictions.First().TagName.Replace("_", " ").Replace("Negative", "neither red nor black figure");
            tag += ".\nConfidence: " + (int)(predictions.First().Probability*100)+"%";
            return tag;
        }
    }
}
