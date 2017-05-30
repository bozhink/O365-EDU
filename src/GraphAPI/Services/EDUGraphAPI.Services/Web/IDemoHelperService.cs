namespace EDUGraphAPI.Services.Web
{
    using EDUGraphAPI.Services.Models.Web;

    public interface IDemoHelperService
    {
        DemoPage GetDemoPage(string controller, string action);
    }
}
