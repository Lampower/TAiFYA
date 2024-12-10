namespace TAiFYA
{
    internal abstract class AnalizatorBase
    {

        public Action<string> OnError;
        protected string inputText = string.Empty;

        protected char pointer;

        public abstract string GetInputText { get; }

        public abstract string ChangeInput(string newInput);
        public AnalizatorResponse SetResponse(bool isError, string? message = null, int pos = 0)
        {
            return new AnalizatorResponse() { 
                IsSuccess = !isError, 
                Message = message ?? "", 
                ErrorIndex = pos, 
                InputField = inputText 
            };
        }
        public abstract AnalizatorResponse StartProgram();
    }
}