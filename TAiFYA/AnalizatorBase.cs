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
            AnalizatorResponse.Instance.IsSuccess = !isError;
            AnalizatorResponse.Instance.Message = message ?? "";
            AnalizatorResponse.Instance.ErrorIndex = pos;
            return AnalizatorResponse.Instance;
        }
        public abstract AnalizatorResponse StartProgram();
    }
}