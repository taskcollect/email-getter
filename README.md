# Exchange API - Glenunga Edition
.NET Core application to get emails from the Glenunga International High School Exchange

## Dependencies
This API uses docker as it is a microservice intended for use in the larger TaskCollect system. As such, the dependencies should be handled by docker so you only need to install docker on your system. Docker compose also works.

Run with
```
docker build . -t ewsapi:latest

docker run -it -p 5000:5000 ewsapi
or
docker-compose up
```

Even though you won't manually need to install these, the project depends on the following .NET libraries, installed with `dotnet add` or `nuget`
* `Microsoft.AspNetCore`
* `Microsoft.Extensions.Hosting`
* `Microsoft.Exchange.WebServices.NETStandard`

The `Microsoft.Exchange.WebServices.NETStandard` library is particularly important as it is a fork of the Microsoft `Microsoft.Exchange.WebServices` library for .NET core, with some additional features such as async requests. The original library runs with .NET framework, not available on linux.

## HTTP Spec
* GET /v1/email
    * Query Parameters
      * amount (int) The number of emails to retrieve, sorted by most recent
    * Headers
      * username (string) The users username, in form CURRIC\XXXXXX
      * password (string)
  
Returns json in following format
```json
[
    {
        "id": string(long base64 identifier),
        "from": string(Name of sender_,
        "isRead": bool(Whether the user has seen this email or not),
        "subject": string(Email subject line),
        "timeSent": int(UTC timestamp of email sending)
    },
    ...
]
```
* GET /v1/body
  * Query Parameters
    * id (string) The unique identifier of the requested email
  * Headers
    * username (string) The useres username, in form CURRIC\XXXXXX
    * password (string)
  
Returns the string of the email body in html, usually very long

# TODOS
* Speed up requests to EWS server, probably involves leveraging async stuff
* Get https working, the username and password probably sent over plaintext http at the moment
