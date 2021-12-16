/*
xxori - Patrick
This is the api controller mapping routes to methods
*/

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using exchangeapi.Models;
using exchangeapi;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Exchange.WebServices.Data;
using System.Text;

namespace exchangeapi.Controllers
{

    [Produces("application/json")] 
    [Route("v1")] // 0.0.0.0/api
    public class EmailController: ControllerBase
    {
        public readonly ILogger _logger;
        private API exchange;
        public EmailController(ILogger<EmailController> logger) {
            _logger = logger;
            exchange = new API(this);
        }

        [HttpGet("mail")] // 0.0.0.0/api/mail?amount=X
        // Takes headers username and password
        public IActionResult Get(int amount)
        {
            if (!Request.Headers.Keys.Contains("Authorization")) {
                Response.StatusCode = 401;
                // Return bad request
                return new JsonResult(new {message="Authentication Error", error="No email credentials were provided, use HTTP basic auth"});
            }
            if (!Request.Headers["Authorization"].ToString().Contains("Basic")){
                Response.StatusCode = 401;
                return new JsonResult(new {message = "Authentication Error", error = "Only basic authorization supported"});
            }
            if (amount == 0) {
                Response.StatusCode = 400;
                return new JsonResult(new {message= "Invalid Request", error="No amount of emails was requested, use the ?amount= query parameter"});
            }

            string auth_encoded = Request.Headers["Authorization"];
            string auth = Encoding.UTF8.GetString(Convert.FromBase64String(auth_encoded.Substring(6)));
            string[] creds = auth.Split(":");
            if (creds.Length != 2) {
                Response.StatusCode = 401;
                return new JsonResult(new {message = "Authentication Error", error = "Malformed Authorization Header"});
            }
            try {
                Response.StatusCode = 200;
                return new JsonResult(new {messages=exchange.getMail(creds[0], creds[1], amount)});
            } catch (ServiceRequestException e){
                if (e.Message.Contains("(401) Unauthorized")) {
                    Response.StatusCode = 401;
                    return new JsonResult(new {message= "Authentication Error", error = "The username and password provided were invalid"});
                }
                return new JsonResult(new {sus = "sus"});
            } catch (Exception e) {
                _logger.LogError(e.ToString());
                Response.StatusCode = 500;
                // Return internal server error with the exception message
                return new JsonResult(new {message = "Internal Server Error", error = "An Unhandled Internal Server Error Occured"});
            }
        }
        [HttpGet("body")] // 0.0.0.0/api/getbody?id=XXXXXXXXXXXX
        // The id is a long base64 string from Email.Id
        public IActionResult Get(string id) {
            if (!Request.Headers.Keys.Contains("Authorization")) {
                Response.StatusCode = 401;
                // Return bad request
                return new JsonResult(new {message="Authentication Error", error="No email credentials were provided, use HTTP basic auth"});
            }
            if (!Request.Headers["Authorization"].ToString().Contains("Basic")){
                Response.StatusCode = 401;
                return new JsonResult(new {message = "Authentication Error", error = "Only basic authorization supported"});
            }
            if (id == null) {
                Response.StatusCode = 400;
                return new JsonResult(new {message= "Invalid Request", error="No email id was requests, use the ?id= query parameter"});
            }

            string auth_encoded = Request.Headers["Authorization"];
            string auth = Encoding.UTF8.GetString(Convert.FromBase64String(auth_encoded.Substring(6)));
            string[] creds = auth.Split(":");
            if (creds.Length != 2) {
                Response.StatusCode = 401;
                return new JsonResult(new {message = "Authentication Error", error = "Malformed Authorization Header"});
            }

            try {
                Response.StatusCode = 200;
                return new JsonResult(exchange.getBody(creds[0], creds[1], id));
            }
            catch (Exception e) {
                Response.StatusCode = 500;
                return new JsonResult(new {message = "Internal Server Error", error=e.GetType().ToString()});
            }
        }
    }
}