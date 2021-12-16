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
* GET /v1/mail
    * Query Parameters
      * amount (int) The number of emails to retrieve, sorted by most recent
    * HTTP Basic Auth
      * Username in format CURRIC\XXXXXX, Password in normal string format
    * Response Codes
      * 401: Credentials were not provided, malformed, or invalid
      * 400: Some query parameter was invalid or not provided
      * 500: An unhandled exception occurred
      * 200: The call was successful
  
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
    * HTTP Basic Auth
      * Username in format CURRIC\XXXXXX, Password in normal string format
    * Response Codes
      * 401: Credentials were not provided, malformed, or invalid
      * 400: Some query parameter was invalid or not provided
      * 500: An unhandled exception occurred
      * 200: The call was successful

# TODOS
* Speed up requests to EWS server, probably involves leveraging async stuff
* Get https working, the username and password probably sent over plaintext http at the moment
