using System;

public class DuplicateGesturePoseException : Exception
{
    public DuplicateGesturePoseException()
    {
    }

    public DuplicateGesturePoseException(string message)
        : base(message)
    {
    }

    public DuplicateGesturePoseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}