using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Greek_Pot_Recognition.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
    public ActionResult OnPost(string uppyResult, string guid)
    {
        Console.WriteLine("Uppy:"+uppyResult);
        Console.WriteLine("Guid: "+guid);
        return LocalRedirect("/results?guid="+ HttpUtility.UrlEncode(guid));
    }
}

