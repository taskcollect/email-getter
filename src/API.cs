/*
xxori - Patrick
Library that takes care of getting emails using .NET core fork of microsoft ews library
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using exchangeapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using exchangeapi.Controllers;

namespace exchangeapi {
    public class API {
        private readonly ILogger _logger;
        private ExchangeService service;
        public API(EmailController controller) {
            _logger = controller._logger;
            service = new ExchangeService(ExchangeVersion.Exchange2013);
        }

        // The exchange library implements its own Task type which breaks it, so you need to manually refer the system type
        // Since our library web functions are async this function is async and returns a templated task of type email array
        public Email[] getMail(string user, string pass, int amount) {
            service.Credentials =  new WebCredentials(user, pass);

            service.TraceEnabled = false; // For SOAP request tracing
            service.Url = new Uri(Environment.GetEnvironmentVariable("EXCHANGE_URL")); // Ews endpoint
            // Grab the specified amount of items from the default inbox folder
            var task = service.FindItems(new FolderId(WellKnownFolderName.Inbox), new ItemView(amount));
            try {
                task.Wait();
                // Cascading task errors
            } catch (AggregateException ae) {
                throw ae.InnerException;
            } catch (Exception e) {
                throw e;
            }
            var items = task.Result;

            // Convert the ews api EmailCollection and EmailMessage type to our own Email type
            List<Email> emails = new List<Email>();
            foreach (EmailMessage msg in items) {
                // Parsing emailMessages
                emails.Add(new Email() {Id = msg.Id.UniqueId, From = msg.From.Name, isRead = msg.IsRead,
                                        Subject = msg.Subject,  TimeSent = ((DateTimeOffset)msg.DateTimeSent).ToUnixTimeSeconds()});
                                        // Have to cast the DateTime object to DateTimeOffset because reasons
            }
            return emails.ToArray(); // Return as an array instead of list because reasons
        }
        public string getBody(string user, string pass, string id) {
            // Same thing but getting body using email.Load(), takes along time because the body is big html
            ExchangeService service= new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(user, pass);
            service.TraceEnabled = false;
            service.Url = new Uri(Environment.GetEnvironmentVariable("EXCHANGE_URL")); // Ews endpoint
            
            //  Only grab 1 email
            ItemView view = new ItemView(1);
            view.PropertySet = PropertySet.FirstClassProperties;
            view.Traversal = ItemTraversal.Shallow;
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            // This is the filter which searches for emails with Ids equal to requsted one, with this it would just grab the most recent one
            SearchFilter filter = new SearchFilter.IsEqualTo(ItemSchema.Id, id);
            var job = service.FindItems(new FolderId(WellKnownFolderName.Inbox), filter, view);
            try {
                job.Wait();
            } catch (AggregateException ae) {
                throw ae.InnerException;
            } catch (Exception e) {
                throw e;
            }
            var items = job.Result;
            // TODO(Patrick) - Speed up this junk
            foreach (EmailMessage msg in items) {
                var task = msg.Load(new PropertySet(ItemSchema.TextBody));
                try {
                    task.Wait();
                } catch (AggregateException ae) {
                    throw ae.InnerException;
                } catch (Exception e) {
                    throw e;
                }
                return msg.TextBody;
            } 
            // If items array is empty
            return "No Body Found";
        }
    }
}
