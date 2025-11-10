using System;

namespace TDDLab.Core.Infrastructure
{
    public interface IExceptionHandler
    {
        void HandleException(Exception ex);
    }
}