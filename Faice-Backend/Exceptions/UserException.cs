using System.Globalization;

namespace Faice_Backend.Exceptions;

public class UserException
    : Exception
{
    public UserException()
        : base()
    { 

    }

    public UserException(string message)
        : base(message)
    {

    }

    public UserException(string message, params object[] args)
    : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {

    }
}
