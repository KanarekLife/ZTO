using System;
using TDDLab.Core.InvoiceMgmt;

namespace TDDLab.Core.Infrastructure
{
    public interface IWorker : IStartable
    {
        void DoJob();        
    }

}