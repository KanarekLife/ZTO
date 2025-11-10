namespace TDDLab.Core.InvoiceMgmt
{
    public enum InvoiceResult
    {
        Failed=1,
        Succeeded = 2
    }

    public class ProcessingResult
    {
        public InvoiceResult Result { get; set; }
        public static ProcessingResult Failed()
        {
            return new ProcessingResult() { Result = InvoiceResult.Failed };
        }

        public static ProcessingResult Succeeded()
        {
            return new ProcessingResult() { Result = InvoiceResult.Succeeded};
        }

        #region equality

        public bool Equals(ProcessingResult obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Result, Result);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ProcessingResult)) return false;
            return Equals((ProcessingResult) obj);
        }

        public override int GetHashCode()
        {
            return Result.GetHashCode();
        }
        #endregion

    }
}