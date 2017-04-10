using Microsoft.Office365.OutlookServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication.Factories;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        // GET: Contacts
        public async Task<ActionResult> Index()
        {
            var contactList = new List<ContactViewModel>();

            var client = await ClientFactory.GreateOutlookServicesClientAsync("Contacts");

            Contact newContact = new Contact
            {
                GivenName = "Katie",
                Surname = "Jordan",
                JobTitle = "Auditor",
                Department = "Finance",
                BusinessPhones = { "+1 412 555 0109" },
                MobilePhone1 = "+1 412 555 9010",
                EmailAddresses = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Address = "katiej@a830edad9050849NDA1.onmicrosoft.com"
                    }
                }
            };

            await client.Me.Contacts.AddContactAsync(newContact);

            // Get the contact ID.
            string contactId = newContact.Id;

            // Fetch
            var contactsResult = await client.Me.Contacts.ExecuteAsync();

            do
            {
                var contacts = contactsResult.CurrentPage;
                foreach (var contact in contacts)
                {
                    contactList.Add(new ContactViewModel
                    {
                        Name = contact.DisplayName
                    });
                }

                contactsResult = await contactsResult.GetNextPageAsync();
            } while (contactsResult != null);

            return this.View(contactList);
        }
    }
}
