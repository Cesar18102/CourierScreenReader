using System;
using System.Net;
using System.Reflection;
using System.Collections.Generic;

using Newtonsoft.Json;

using ScreenReaderService.Data;
using ScreenReaderService.Data.Exceptions;

namespace ScreenReaderService.Services
{
    public class StatusCodeHandlerService
    {
        private static Dictionary<HttpStatusCode, Type> ErrorStatusCodes = new Dictionary<HttpStatusCode, Type>()
        {
            { HttpStatusCode.BadRequest, typeof(ValidationException) },
            { HttpStatusCode.Unauthorized, typeof(UnauthorizedAccessException) },
            { HttpStatusCode.NotFound, typeof(NotFoundException) },
            { HttpStatusCode.PaymentRequired, typeof(PaymentRequiredException) }
        };

        public void HandleResponseStatus(HttpStatusCode code, string response)
        {
            if (code == HttpStatusCode.OK)
                return;

            ResponseError error = JsonConvert.DeserializeObject<ResponseError>(response);
            if (ErrorStatusCodes.ContainsKey(code))
            {
                ConstructorInfo ctor = ErrorStatusCodes[code].GetConstructor(new Type[] { typeof(string) });
                Exception ex = ctor.Invoke(new object[] { error.Message }) as Exception;

                throw ex;
            }

            throw new Exception(error.Message);
        }
    }
}