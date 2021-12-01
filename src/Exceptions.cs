/*
xxori - Patrick
Custom defined errors so we can return better info from the api when the thingy errors out
*/
using System;
public class IncorrectCredentials : Exception {
        public IncorrectCredentials()
    {
    }

    public IncorrectCredentials(string message)
        : base(message)
    {
    }

    public IncorrectCredentials(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class InvalidExchangeServer : Exception {
        public InvalidExchangeServer()
    {
    }

    public InvalidExchangeServer(string message)
        : base(message)
    {
    }

    public InvalidExchangeServer(string message, Exception inner)
        : base(message, inner)
    {
    }
}