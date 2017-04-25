using MVCPeopleAwards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Helpers
{
    public static class ErrorHelper
    {
        public static ErrorViewModel GetErrorModel(string messageError, string stackTraceError = "", string backUrl = "")
        {
            ErrorViewModel errorModel = new ErrorViewModel();
            {
                errorModel.MessageError = messageError;
                errorModel.StackTraceError = stackTraceError;
                errorModel.BackUrl = backUrl;
            }
            return errorModel;
        }
    }
}