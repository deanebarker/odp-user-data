using DeaneBarker.Optimizely;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace DeaneBarker.Optimizely.OptiData.Controllers
{
    [Route("profile")]
    public class ProfileController : Controller
    {
        private readonly IHttpContextAccessor _getHttpContext;

        public ProfileController(IHttpContextAccessor getHttpContext)
        {
            _getHttpContext = getHttpContext;

            // Set these for your own instance
            // OptiDataProfile.KeyField = "email";
            // OptiDataProfile.ApiKey = "whatever";

            OptiDataProfile.IdProvider = () =>
            {
                var appUser = _getHttpContext.HttpContext.User;

                if (!appUser.Identity.IsAuthenticated)
                {
                    return null;
                }

                var email = appUser.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
                return email;
            };
        }


        public ActionResult Index()
        {
            if(!_getHttpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return View("NotLoggedIn");
            }

            var profile = OptiDataProfile.GetCurrentProfile();
            var model = new ProfileViewModel
            {
                Email = profile.GetString("email"),
                Address1 = profile["street1"],
                Address2 = profile["street2"],
                City = profile["city"],
                State = profile["state"],
                Zip = profile["zip"]
            };

            return View(model);
        }

        [HttpPost]
        public RedirectToActionResult Index(IFormCollection form)
        {
            var profile = OptiDataProfile.GetCurrentProfile();
            profile.SetValues(new {
                street1 = form["address1"].FirstOrDefault(),
                street2 = form["address2"].FirstOrDefault(),
                city = form["city"].FirstOrDefault(),
                state = form["state"].FirstOrDefault(),
                zip = form["zip"].FirstOrDefault(),
            });
            return RedirectToAction("Index");
        }
    }


    public class ProfileViewModel
    {
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}
