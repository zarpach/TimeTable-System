using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Common
{
    public class ResultViewModel
    {
        public ResultViewModel()
        {
        }

        public ResultViewModel(bool success)
        {
            Success = success;
        }

        public ResultViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
