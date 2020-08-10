using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Net;
using System.Net.Http;

using System.Web.Http;
using System.Web.Http.ModelBinding;

using Newtonsoft.Json;

using Server.Models.Exceptions;

namespace Server.Controllers
{
    public class ControllerBase : ApiController
    {
        private static Dictionary<Type, HttpStatusCode> ErrorStatusCodes = new Dictionary<Type, HttpStatusCode>()
        {
            { typeof(ValidationException), HttpStatusCode.BadRequest },
            { typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized },
            { typeof(NotFoundException), HttpStatusCode.NotFound },
            { typeof(PaymentRequiredException), HttpStatusCode.PaymentRequired }
        };

        protected class Error
        {
            [JsonProperty("code")]
            public HttpStatusCode Code { get; private set; }

            [JsonProperty("error")]
            public string Message { get; private set; }

            public Error(HttpStatusCode code, string message) 
            {
                Code = code;
                Message = message;
            }
        }

        public HttpResponseMessage Execute(Func<object> executor)
        {
            return ProtectedExecute(executor, null, false);
        }

        public HttpResponseMessage Execute(Func<object, object> executor, object parameter)
        {
            return ProtectedExecute(() => executor(parameter), parameter, true);
        }

        public async Task<HttpResponseMessage> Execute(Func<Task<object>> executor)
        {
            return await Task.Run(
                () => ProtectedExecute(() => executor().GetAwaiter().GetResult(), null, false)
            );
        }

        public async Task<HttpResponseMessage> Execute(Func<object, Task<object>> executor, object parameter)
        {
            return await Task.Run(
                () => ProtectedExecute(() => executor(parameter).GetAwaiter().GetResult(), parameter, true)
            );
        }

        protected virtual void Validate()
        {
            ICollection<string> errors = new List<string>();
            foreach (KeyValuePair<string, ModelState> fieldState in ModelState)
                foreach (ModelError error in fieldState.Value.Errors)
                    errors.Add(error.ErrorMessage ?? error.Exception?.Message);

            if (errors.Count != 0)
                throw new ValidationException(errors);
        }

        private HttpResponseMessage CreateErrorResponse(Exception ex)
        {
            Type type = ex.GetType();

            if(ErrorStatusCodes.ContainsKey(type))
            {
                Error error = new Error(ErrorStatusCodes[type], ex.Message);
                return Request.CreateResponse(error.Code, error);
            }

            throw ex;
        }

        private HttpResponseMessage ProtectedExecute(Func<object> executor, object parameter, bool mustHaveParameter)
        {
            try
            {
                if (mustHaveParameter && parameter == null)
                    throw new ValidationException("no data passed");

                Validate();

                return Request.CreateResponse(HttpStatusCode.OK, executor());
            }
            catch(Exception ex) { return CreateErrorResponse(ex); }
        }
    }
}
