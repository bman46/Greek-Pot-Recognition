using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greek_Pot_Recognition.Tables.Items;
using Greek_Pot_Recognition.Tables.Repository;
using Greek_Pot_Recognition.Tables.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Greek_Pot_Recognition.Pages.API
{
    public class GetImageModel : PageModel
    {
        private readonly IFilesRepository _FilesRepository;
        private readonly IUploadRepository _UploadRepository;
        public GetImageModel(IFilesRepository filesRepo, IUploadRepository uploadRespository)
        {
            _FilesRepository = filesRepo;
            _UploadRepository = uploadRespository;
        }
        public async Task<ActionResult> OnGet(string guid)
        {
            if (String.IsNullOrEmpty(guid))
            {
                return LocalRedirect("/");
            }
            UploadedFile fileData = await _UploadRepository.GetByFileGuidAsync(guid);
            byte[] contents = await _FilesRepository.GetFileByNameAsync(fileData.UploadGuid);
            return File(contents, fileData.MimeType);
        }
    }
}
