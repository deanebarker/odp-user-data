using DeaneBarker.Optimizely;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace Alloy.Liquid.Controllers
{
    [Route("profile")]
    public class ProfileController : Controller
    {
        private readonly IHttpContextAccessor _getHttpContext;

        public ProfileController(IHttpContextAccessor getHttpContext)
        {
            _getHttpContext = getHttpContext;

            OptiDataProfile.KeyField = "email";
            OptiDataProfile.ApiKey = "W4WzcEs-ABgXorzY7h1LCQ.aqx2ho-xztHtfG0A0McOMxA6_AUGUPF09H-aDGLOkzM";

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

            var profile = OptiDataProfile.GetForCurrentUser();
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
            var profile = OptiDataProfile.GetForCurrentUser();
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
