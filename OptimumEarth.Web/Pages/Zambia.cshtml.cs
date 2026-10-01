using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class ZambiaModel : DestinationPageModel
{
    public ZambiaModel(ContentReader content, InquiryService inquiries)
        : base(content, inquiries, "zambia", "Zambia")
    {
    }
}
