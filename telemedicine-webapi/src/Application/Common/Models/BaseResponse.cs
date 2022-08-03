using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Application.Common.Models;

public class BaseResponse
{
    internal BaseResponse SetError(string error)
    {
        Errors.Add(error);
        return this;
    }

    internal BaseResponse SetErrors(IEnumerable<string> errors)
    {
        Errors.AddRange(errors);
        return this;
    }

    public object? Data { get; set; }
    public List<string> Errors => new List<string>();
    public bool WasSuccesful => !NotSuccesful;
    public bool NotSuccesful { get; internal set; }
}

public class OperationResult
{
    public static BaseResponse Successful()
    {
        return new BaseResponse
        {
            NotSuccesful = false,
        };
    }

    public static BaseResponse Successful(object result)
    {
        return new BaseResponse
        {
            Data = result,
            NotSuccesful = false,
        };
    }

    public static BaseResponse Successful(object result, List<string> invalidItems)
    {
        return new BaseResponse
        {
            Data = result,
            NotSuccesful = false,
        }.SetErrors(invalidItems);
    }

    public static BaseResponse NotSuccessful(string error)
    {
        return new BaseResponse
        {
            NotSuccesful = true,
        }.SetError(error);
    }

    public static BaseResponse NotSuccessful(IEnumerable<string> errors)
    {
        return new BaseResponse
        {
            NotSuccesful = true,
        }.SetErrors(errors);
    }
}
