using AllinOne.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AllinOne.ResultPattern
{
    public record InternalDataTransfer<T>
    {
        public bool Status => Error is null;
        public T? Data { get; set; }
        public string? Description { get; init; }
        public InternalErrorObject? Error { get; init; } = null;
        public ProjectErrorCodes? ProjectErrorCodes { get; set; }


        public InternalDataTransfer(T data, string? description = null)
        {
            Data = data;
            Description = description;
        }
        public InternalDataTransfer(T data, ProjectErrorCodes code)
        {
            Data = data;
            ProjectErrorCodes = code;
        }

        public InternalDataTransfer(Exception e)
        {
            Error = new InternalErrorObject(e);
        }

        public InternalDataTransfer(Exception e, string description)
        {
            Error = new InternalErrorObject(e, description);
        }

        public InternalDataTransfer(InternalErrorObject error)
        {
            Error = error;
        }

        public class InternalErrorObject
        {
            public string Error { get; init; }
            public string? Description { get; init; }
            public bool IsExceptionTypeError { get; init; }
            public ProjectErrorCodes? Code { get; init; }

            public InternalErrorObject()
            {
            }

            public InternalErrorObject(Exception e)
            {
                Error = e.Message;
                Description = e?.StackTrace ?? "Error occurred";
                IsExceptionTypeError = true;

            }

            public InternalErrorObject(Exception e, string? description = null)
            {
                if (description == null)
                {
                    Error = e.Message;
                    Description = e?.StackTrace ?? "Error occurred";
                    IsExceptionTypeError = true;
                }
                else
                {
                    Error = $"Message: {e.Message}, StackTrace: {e.StackTrace}";
                    Description = description;
                    IsExceptionTypeError = true;
                }

            }

            public InternalErrorObject(ProjectErrorCodes code)
            {
                Error = "Error";
                Code = code;
                IsExceptionTypeError = false;
            }

        }
    }
}
