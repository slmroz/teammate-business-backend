namespace ERP.Services.Shared.CommonServices;
public interface IWebService
{
    Task<string> DownloadAsync(string url);
}