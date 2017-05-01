using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Models
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
        }

        private string messageError = "";
        [Display(Name = "Текст ошибки")]
        public string MessageError { get { return messageError; } set { messageError = value; } }

        private string stackTraceError = "";
        [Display(Name = "Подробности ошибки")]
        public string StackTraceError { get { return stackTraceError; } set { stackTraceError = value; } }

        private string backUrl = "";
        public string BackUrl { get { return backUrl; } set { backUrl = value; } }
    }
}