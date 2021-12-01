/*
xxori - Patrick
This is the api controller mapping routes to methods
*/

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Models;
using exchangeapi;
using System;
using Microsoft.Extensions.Logging;

namespace exchangeapi.Controllers
{

    [Produces("application/json")] 
    [Route("v1")] // 0.0.0.0/api
    public class EmailController: ControllerBase
    {
        private readonly ILogger _logger;
        public EmailController(ILogger<EmailController> logger) {
            _logger = logger;
        }

        [HttpGet("mail")] // 0.0.0.0/api/mail?amount=X
        // Takes headers username and password
        public IActionResult Get(int amount)
        {
            if (!Request.Headers.Keys.Contains("username") || !Request.Headers.Keys.Contains("password")) {
                Response.StatusCode = 400;
                // Return bad request
                return new JsonResult(new {status=400, message="Invalid Request", error="No email credentials were provided, use the username and password headers with username in format CURRIC\\XXXXXX"});
            }
            if (amount == 0) {
                Response.StatusCode = 400;
                return new JsonResult(new {status=400, message= "Invalid Request", error="No amount of emails was requested, use the ?amount= query parameter"});
            }
            try {
                Response.StatusCode = 200;
                return new JsonResult(new {status=200, messages=API.Get_Emails(Request.Headers["username"], Request.Headers["password"], amount)});
            }
            catch (System.AggregateException ae) {
                _logger.LogError(ae.ToString());
                if (ae.InnerException is Microsoft.Exchange.WebServices.Data.ServiceRequestException && ae.InnerException.Message == "The request failed. Name or service not known Name or service not known") {
                     _logger.LogCritical("The Exchange server URL is invalid, set with EXCHANGE_URL environment variable");
                     Response.StatusCode = 500;
                     return new JsonResult(new {status = 500, message = "Internal Server Error", error = "Exchange Server URL Invalid"});
                } 
                else if (ae.InnerException is Microsoft.Exchange.WebServices.Data.ServiceRequestException && ae.InnerException.Message == "The request failed. The remote server returned an error: (401) Unauthorized.") {
                    Response.StatusCode = 401;
                    return new JsonResult(new {status = 401, message = "Authorization Error", error = "Invalid username or password"});
                }
                else {
                    _logger.LogError(ae.InnerException.ToString());
                    Response.StatusCode = 500;
                    // Return internal server error with the exception message
                    return new JsonResult(new {status = 500, message = "Internal Server Error", error = "Something went wrong"});
                }
            }
            catch (Exception e) {
                _logger.LogError(e.ToString());
                Response.StatusCode = 500;
                // Return internal server error with the exception message
                return new JsonResult(new {message = "Internal Server Error", error = "An Unhandled Internal Server Error Occured"});
            }
        }

        [HttpGet("body")] // 0.0.0.0/api/getbody?id=XXXXXXXXXXXX
        // The id is a long base64 string from Email.Id
        public IActionResult Get(string id) {
            if (!Request.Headers.Keys.Contains("username") || !Request.Headers.Keys.Contains("password")) {
                Response.StatusCode = 400;
                return new JsonResult(new {message="Invalid Request", error="No email credentials were provided, use the username and password headers with username in format CURRIC\\XXXXXX"});
            }
            if (id == null) {
                Response.StatusCode = 400;
                return new JsonResult(new {message= "Invalid Request", error="No email id was requests, use the ?id= query parameter"});
            }
            try {
                Response.StatusCode = 200;
                return new JsonResult(API.Get_Body(Request.Headers["username"], Request.Headers["password"], id));
            }
            catch (Exception e) {
                Response.StatusCode = 500;
                return new JsonResult(new {message = "Internal Server Error", error=e.Message});
            }
        }
    }
}