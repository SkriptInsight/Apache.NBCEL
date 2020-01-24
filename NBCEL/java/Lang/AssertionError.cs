using System;

namespace ObjectWeb.Misc.Java.Lang
{
    internal class AssertionError : Exception
    {
        public AssertionError(string error = "")
        {
            Error = error;
        }

        public string Error { get; }
    }
}