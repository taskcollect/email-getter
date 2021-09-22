/*
xxori - Patrick
Library that takes care of getting emails using .NET core fork of microsoft ews library
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using Models;

namespace exchangeapi {
    public class API {

        // The exchange library implements its own Task type which breaks it, so you need to manually refer the system type
        // Since our library web functions are async this function is async and returns a templated task of type email array
        private static async System.Threading.Tasks.Task<Email[]> ewsget(string user, string pass, int amount) {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013); // Our server is exchange2013 (source: trust me guys)
            service.Credentials =  new WebCredentials(user, pass);

            service.TraceEnabled = false; // For SOAP request tracing
            // service.TraceFlags = TraceFlags.All; // No more tracing
            service.Url = new Uri("https://webmail.gihs.sa.edu.au/ews/Exchange.asmx"); // Ews endpoint
            // Grab the specified amount of items from the default inbox folder
            var items = await service.FindItems(new FolderId(WellKnownFolderName.Inbox), new ItemView(amount));

            // Convert the ews api EmailCollection and EmailMessage type to our own Email type
            List<Email> emails = new List<Email>();

            foreach (EmailMessage msg in items) {
                // Parsing emailMessages
                emails.Add(new Email() {Id = msg.Id.UniqueId, From = msg.From.Name, isRead = msg.IsRead,
                                        Subject = msg.Subject,  TimeSent = msg.DateTimeSent.ToUniversalTime().Ticks});
            }
            return emails.ToArray(); // Return as an array instead of list because reasons
        }
        private static async System.Threading.Tasks.Task<string> getbody(string user, string pass, string id) {
            // Same thing but getting body using email.Load(), takes along time because the body is big html
            ExchangeService service= new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(user, pass);
            service.TraceEnabled = false;
            service.Url = new Uri("https://webmail.gihs.sa.edu.au/ews/Exchange.asmx");
            
            //  Only grab 1 email
            ItemView view = new ItemView(1);
            view.PropertySet = PropertySet.FirstClassProperties;
            view.Traversal = ItemTraversal.Shallow;
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            // This is the filter which searches for emails with Ids equal to requsted one, with this it would just grab the most recent one
            SearchFilter filter = new SearchFilter.IsEqualTo(ItemSchema.Id, id);
            var items = await service.FindItems(new FolderId(WellKnownFolderName.Inbox), filter, view);
            // TODO(Patrick) - Speed up this junk
            foreach (EmailMessage msg in items) {
                await msg.Load();
                return msg.Body;
            } 
            // If items array is empty
            return "No Body Found";
        }
        public static Email[] Get_Emails(string user, string pass, int amount) {
            // Public method which runs the async method and waits for it to finish
            var task = ewsget(user, pass, amount);
            task.Wait();
            return task.Result;
        }

        public static string Get_Body(string user, string pass, string id) {
            var task = getbody(user, pass, id);
            task.Wait();
            return task.Result;
        }
    }
}
