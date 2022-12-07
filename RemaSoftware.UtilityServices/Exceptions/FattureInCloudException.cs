using System;

namespace RemaSoftware.UtilityServices.Exceptions;

public class FattureInCloudException : Exception
{
    public FattureInCloudException() { }
    
    public FattureInCloudException (string message) : base(message) {}

    public FattureInCloudException (string message, Exception innerException) 
        : base (message, innerException) {}    
}