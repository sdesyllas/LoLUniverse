using System.Web.Mvc;
using hbehr.recaptcha;
using LoLUniverse.Models;

namespace LoLUniverse.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us";

            return View();
        }

        public ActionResult SendContactMessage(ContactModel contactModel)
        {
            string userResponse = HttpContext.Request.Params["g-recaptcha-response"];
            bool validCaptcha = ReCaptcha.ValidateCaptcha(userResponse);

            if (!ModelState.IsValid)
            {
                return View("Contact");
            }
            if (!validCaptcha)
            {
                ViewBag.CatchaError = "Captcha not provided";
                return View("Contact");
            }

            string subject = $"{contactModel.Name} {contactModel.Email}";

            string body =
                $@"Name : {contactModel.Name} <br/> Surname : {contactModel.Surname} <br/> Email : {
                    contactModel.Email
                    } <br/> Phone : {contactModel.PhoneNumber}
                            <br/> Message : <br/> {
                    contactModel.Message}";

            //send email to admins
            var response =
                Utilities.EmailSender.SendEmail(System.Configuration.ConfigurationManager.AppSettings["appEmail"],
                    subject,
                    body);

            //send a copy to user
            var response2 =
                Utilities.EmailSender.SendEmail(contactModel.Email, subject, body);

            contactModel.Success = response && response2;
            // using the POST/REDIRECT/GET Pattern to prevent form resubmission
            // https://en.wikipedia.org/wiki/Post/Redirect/Get
            return RedirectToAction("ThankYou");
        }

        public ActionResult ThankYou(ContactModel contactModel)
        {
            return View(contactModel);
        }
    }
}