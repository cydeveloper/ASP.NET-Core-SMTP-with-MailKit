# ASP.NET Core 1.0 - SMTP using MailKit 

<h3>Step 1)</h3>

Create a new ASP.NET Core Project. 

For Authenitication, you can choose <b>Individual account</b> where the configuration and classes is already setup for you. 

For this demo, I chose <b>No Authentication</b>. 

Here is a screenshot of the project tree structure: 

![treestructure](https://cloud.githubusercontent.com/assets/11988924/18095025/9e09c836-6ea3-11e6-937b-cfe059d059bd.png)

As you can see, I created the required classes individually just to keep things simple. 

<br/>


<h3>Step 2)</h3>

After creating the project: 

<strong>Install MailKit through Nuget</strong>

![nugetmailkit](https://cloud.githubusercontent.com/assets/11988924/18094332/1c55e236-6ea1-11e6-9d70-922c299051b1.png)

<br/>


<h3>Step 3)</h3>

Create <i>Contact.cs</i> inside the Models folder. 

```C#
    public class Contact
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
```

<br/>

Create <i>IEmailSender.cs</i> and <i>MessageServices.cs</i> inside the Services folder if you chose <b>No Authentication</b>

<i>IEmailSender.cs</i>
```C#
    public interface IEmailSender 
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
```

<br/>

Enter your email address and password. Also setup your SMTP Provider (e.g. gmail, outlook, etc.) inside of client.Connect(). 

<i>MessageServices.cs</i>
```C#
    public class AuthMessageSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("TEST SMTP", "Enter_Sender_EmailAdress"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            await Task.Run(() =>
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.live.com", 587, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("Enter_Sender_EmailAdress", "Enter_Sender_EmailAdress_Password");

                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
            });
        }
    }
```
<br/>

Also, make sure that you add the Email Services to the ConfigureServices() method in <i>Startup.cs</i> 

<i>IEmailSender.cs</i>
```C#
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Email services. 
            services.AddTransient<IEmailSender, AuthMessageSender>();
        }
```

<h3>Step 4)</h3>

Inside <i>HomeController.cs</i>

Create an instance field of IEmailSender 

```C#

    public class HomeController : Controller
    {
        private IEmailSender _emailSender { get; }

        public HomeController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        
        // More Code Below .... 
    }

```
<br/>

To get the contact form working, copy the code below and paste it inside the Contact() method in <i>HomeController.cs</i>
```C#

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

```

Replace the code in <b>~/Home/Contact.cshtml</b> with the code below to create the contact form. 
```HTML
@model AspNetCore_SMTP_MalKit.Models.Contact

@{
    ViewData["Title"] = "Contact Page";
}

<h2>@ViewData["Title"].</h2>

<div asp-validation-summary="ModelOnly" class="container">

    @if (ViewData["Message"] != null)
    {
        <strong style="color:@ViewData["message_color"]"> @ViewData["Message"] </strong>
    }

    <form asp-action="Contact" method="post">
        <div class="form-group">
            <label asp-for="Email">Recipient Email Address: </label>
            <input asp-for="Email" class="form-control" /><br />
            <span asp-validation-for="Email" class="text-danger"></span>
        </div>

        <div class="form-group">
            <label asp-for="Subject">Subject: </label>
            <input asp-for="Subject" class="form-control" /><br />
            <span asp-validation-for="Subject" class="text-danger"></span>
        </div>

        <div class="form-group">
            <label asp-for="Message">Message: </label>
            <input asp-for="Message" class="form-control" /> 
            <br />
            <span asp-validation-for="Message" class="text-danger"></span>
        </div>

        <button type="submit" class="btn btn-primary">Send Message</button>
    </form>
</div>
```

<h3>Step 5)</h3>

Testing - Press CTRL + F5 to lanuch your application

Navigate to the Contact page.

Enter in the recipient's emaill address, a subject and a message. 

Once you hit Send Message, check the recipient's email inbox to confirm that your message has been sent. 

<h3>REFERENCES</h3>
https://github.com/jstedfast/MailKit <br/>
https://components.xamarin.com/gettingstarted/mailkit
