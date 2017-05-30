namespace EDUGraphAPI.Services.Web
{
    public interface ICookieService
    {
        void ClearCookies();

        string GetCookiesOfEmail();

        string GetCookiesOfUsername();
    }
}
