using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAiFYA
{
    public class AnalizatorResponse
    {
        private static AnalizatorResponse instance;
        public static AnalizatorResponse Instance => instance;

        public AnalizatorResponse() { instance = this; }
        public Dictionary<string, string> IdentifiersAndConsts = new Dictionary<string, string>();
        public string Message { get; set; } = "Ошибка";
        public int ErrorIndex { get; set; } = 0;
        public Dictionary<string, string> Identificators { get; set; } = new();
        public Dictionary<string, string> Constants { get; set; } = new();

        public bool IsSuccess {  get; set; } = true;
        public string InputField { get; set; } = string.Empty;

    }
}
