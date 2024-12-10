using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAiFYA
{
    public class AnalizatorResponse
    {
        public string Message { get; set; } = "";
        public int ErrorIndex { get; set; } = 0;
        public List<string> Identificators { get; set; } = new();
        public List<string> Constants { get; set; } = new();

        public bool IsSuccess {  get; set; } = true;
        public string InputField { get; set; } = string.Empty;
    }
}
