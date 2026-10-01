using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class UgandaModel : DestinationPageModel
{
    public UgandaModel(ContentReader content, InquiryService inquiries)
        : base(content, inquiries, "uganda", "Uganda")
    {
    }
}
