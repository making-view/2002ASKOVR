using System;

public class DuplicatePoseNameException : Exception
{
    public DuplicatePoseNameException()
    {
    }

    public DuplicatePoseNameException(string message)
        : base(message)
    {
    }

    public DuplicatePoseNameException(string message, Exception inner)
        : base(message, inner)
    {
    }
}