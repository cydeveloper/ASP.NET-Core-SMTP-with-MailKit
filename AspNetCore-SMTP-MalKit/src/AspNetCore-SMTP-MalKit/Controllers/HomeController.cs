using Microsoft.AspNetCore.Mvc;
using SmtpTest.Services;
using AspNetCore_SMTP_MalKit.Models;

namespace AspNetCore_SMTP_MalKit.Controllers
{
    public class HomeController : Controller
    {
        private IEmailSender _emailSender { get; }

        public HomeController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            // If you want the email to be send when you press CTRL + F5,
            // then uncomment _emailSender below 

            // _emailSender.SendEmailAsync("Recipient_EmailAdress", "test", "it works!");

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(Contact contact)
        {
            // This is for the Contact Form view, you can test the form 
            // at http://localhost:41717/Home/Contact
            ViewData["Message"] = null;
            ViewData["Error"] = null;

            if (ModelState.IsValid)
            {
                var email = contact.Email;
                var subject = contact.Subject;
                var message = contact.Message;

                _emailSender.SendEmailAsync(email, subject, message);

                ViewData["message_color"] = "green";
                ViewData["Message"] = "*Your Message Has Been Sent.";
            }
            else
            {
                ViewData["message_color"] = "red";
                ViewData["Message"] = "*Please complete the required fields";
            }

            ModelState.Clear();
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
