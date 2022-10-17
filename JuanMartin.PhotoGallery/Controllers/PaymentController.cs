using Microsoft.AspNetCore.Mvc;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
