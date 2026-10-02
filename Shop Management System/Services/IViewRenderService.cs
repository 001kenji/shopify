namespace Shop_Management_System.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewPath, object model);
    }
}