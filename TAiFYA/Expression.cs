using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAiFYA
{
    internal class Expression : AnalizatorBase
    {
        private enum State
        {
            StartState,
            OperandState1,
            OperationState,
            OperandState2,
            FinishState,
            ErrorState

        }

        private enum OperandState
        {
            StartState,
            ConstantState,
            IdentState,
            BracketOpenState,
            BracketConstantState,
            BracketIdentState,
            BracketConstantCloseState,
            BracketIdentCloseState,
            FinishState,
            ErrorState
        }
        public override string GetInputText => inputText;

        public override string ChangeInput(string newInput)
        {
            inputText = newInput;
            return inputText;
        }

        public override AnalizatorResponse StartProgram()
        {
            AnalizatorResponse response = AnalizatorResponse.Instance;
            response.InputField = inputText;
            if (string.IsNullOrEmpty(inputText))
            {
                return SetResponse(true, "Пустая строка");
            }

            response.InputField = inputText;

            int i = 0;
            var currentState = State.StartState;
            string text = inputText + ' ';
            string curWord = string.Empty;
            string wordExpected = string.Empty;
            string error = string.Empty;
            int errorIndex = 0;

            string[] operations = ["+", "-", "*", "/"];

            while (!(currentState == State.FinishState || currentState == State.ErrorState))
            {
                if (i >= inputText.Length)
                {
                    if (currentState == State.OperandState1)
                    {
                        var oper1res = OperandCheck(curWord);

                        if (!oper1res.IsSuccess)
                        {
                            error = oper1res.Message;
                            errorIndex += oper1res.ErrorIndex;
                            currentState = State.ErrorState;
                        }
                        else
                        {
                            //response.Identificators.AddRange(oper1res.Identificators);
                            //response.Constants.AddRange(oper1res.Constants);
                        }
                    }
                    else if (currentState == State.OperandState2)
                    {
                        var oper1res = OperandCheck(curWord);

                        if (!oper1res.IsSuccess)
                        {
                            error = oper1res.Message;
                            errorIndex += oper1res.ErrorIndex;
                            currentState = State.ErrorState;
                        }
                        else
                        {
                            //response.Identificators.AddRange(oper1res.Identificators);
                            //response.Constants.AddRange(oper1res.Constants);
                        }
                    }
                    else
                    {
                        error = "Нету конечного символа!";
                    }
                    break;
                }
                   
                pointer = inputText[i];
                i++;

                switch (currentState)
                {
                    case State.StartState:
                        if (pointer != ' ')
                        {
                            curWord += pointer;
                            errorIndex = i;
                            currentState = State.OperandState1;
                        }
                        break;
                    case State.OperandState1:
                        if (IsOperation(pointer.ToString()))
                        {
                            var oper1res = OperandCheck(curWord);

                            if (!oper1res.IsSuccess)
                            {
                                error = oper1res.Message;
                                i = i + oper1res.ErrorIndex;
                                currentState = State.ErrorState;
                                break;
                            }
                            else
                            {
                                //response.Identificators.AddRange(oper1res.Identificators);
                                //response.Constants.AddRange(oper1res.Constants);
                            }
                            curWord = string.Empty;
                            currentState = State.OperationState;
                        }
                        else
                        {
                            curWord += pointer;
                        }
                        break;
                    case State.OperationState:
                        if (pointer == ' ')
                        {

                        }
                        else
                        {
                            errorIndex = i;
                            curWord += pointer;
                            currentState = State.OperandState2;
                        }
                        break;
                    case State.OperandState2:
                        curWord += pointer;
                        break;
                }
            }

            if (error != string.Empty)
            {
                return SetResponse(true, error, errorIndex);
            }


            return response;
        }

        private AnalizatorResponse OperandCheck(string word)
        {
            AnalizatorResponse response = AnalizatorResponse.Instance;
            int i = -1;
            int lastIndex = word.Length;
            string error = string.Empty;
            char pointer;
            string curWord = string.Empty;
            var currentState = OperandState.StartState;
            bool isSpaceBetweenIdAnBr = false;

            

            while (currentState != OperandState.FinishState && currentState != OperandState.ErrorState)
            {
                i++;
                if (i >= lastIndex)
                {
                    if (currentState == OperandState.ConstantState)
                    {
                        int.TryParse(curWord, out var value);
                        if (value < 0 || value > 32767)
                        {
                            error = "Семантическая ошибка, число вне диапазона";
                            currentState = OperandState.ErrorState;
                            break;
                        }

                        response.Constants.Add($"{curWord}", "константа");
                    }
                    else if (currentState == OperandState.IdentState)
                    {
                        if (curWord.Length > 8 || curWord == "do" || curWord == "while")
                        {
                            error = $"Семантическая ошибка";
                            currentState = OperandState.ErrorState;
                            break;
                        }
                        if (response.Identificators.ContainsKey(curWord))
                        {
                            error = $"Семантическая ошибка \nидентификатор уже был использован как {response.Identificators[curWord]}";
                            currentState = OperandState.ErrorState;
                            break;
                        }
                        response.Identificators.Add($"{curWord}", "идентефикатор");
                    }
                    else
                    {
                        error = "Скобки не закрыты!";
                        currentState = OperandState.ErrorState;
                    }
                    break;
                }

                
                pointer = word[i];

                

                switch (currentState)
                {
                    case OperandState.StartState:
                        if (pointer == ' ')
                        {
                        }
                        else
                        {
                            if (char.IsDigit(pointer))
                            {
                                curWord += pointer;
                                currentState = OperandState.ConstantState;
                            }
                            else if (char.IsLetter(pointer))
                            {
                                curWord += pointer;
                                currentState = OperandState.IdentState;
                            }
                            else
                            {
                                error = "Ожидалась константа или идентификатор";
                                currentState = OperandState.ErrorState;
                            }
                        }
                        break;
                    case OperandState.ConstantState:
                        if (isSpaceBetweenIdAnBr && char.IsDigit(pointer))
                        {
                            error = "Предполагалась одна константа";
                            currentState = OperandState.ErrorState;
                        }
                        else if (char.IsDigit(pointer))
                        {
                            curWord += pointer;
                        }
                        else if (pointer == ' ')
                        {
                            isSpaceBetweenIdAnBr = true;
                        }
                        else
                        {
                            error = "Предполагалась константа";
                            currentState = OperandState.ErrorState;
                        }
                        break;
                    case OperandState.IdentState:
                        if (pointer == '(')
                        {
                            if (response.Identificators.ContainsKey(curWord))
                            {
                                error = $"Семантическая ошибка \nидентификатор уже был использован как {response.Identificators[curWord]}";
                                currentState = OperandState.ErrorState;
                                break;
                            }
                            response.Identificators.Add($"{curWord}", "идентификатор-массив");
                            curWord = string.Empty;
                            currentState = OperandState.BracketOpenState;
                        }
                        else if ( pointer == ' ')
                        {
                            isSpaceBetweenIdAnBr = true;
                        }
                        else if (isSpaceBetweenIdAnBr && char.IsLetter(pointer))
                        {
                            error = "ожидалась скобка";
                            currentState = OperandState.ErrorState;
                        }
                        else
                        {
                            curWord += pointer;
                        }
                        break;
                    case OperandState.BracketOpenState:
                        if (pointer == ' ')
                        {

                        }
                        else if (char.IsDigit(pointer))
                        {
                            curWord += pointer;
                            isSpaceBetweenIdAnBr = false;
                            currentState = OperandState.BracketConstantState;
                        }
                        else if (char.IsLetter(pointer))
                        {
                            curWord += pointer;
                            isSpaceBetweenIdAnBr = false;
                            currentState = OperandState.BracketIdentState;
                        }
                        else
                        {
                            error = "ожидался индекса массива";
                        }
                        break;
                    case OperandState.BracketConstantState:
                        if (pointer == ' ')
                        {
                            isSpaceBetweenIdAnBr = true;
                        }
                        else if (pointer == ')')
                        {
                            if (response.Constants.ContainsKey(curWord))
                            {
                                error = $"Семантическая ошибка \nконстанта уже была использован как {response.Identificators[curWord]}";
                                currentState = OperandState.ErrorState;
                                break;
                            }
                            response.Constants.Add($"{curWord}", "константа-индекс");
                            curWord = string.Empty;
                            currentState = OperandState.FinishState;
                        }
                        else if (pointer != ' ' && isSpaceBetweenIdAnBr)
                        {
                            error = "лишний символ в массиве";
                            currentState = OperandState.ErrorState;
                        }
                        else if (char.IsDigit(pointer))
                        {
                            curWord += pointer;
                        }
                        else
                        {
                            error = "ожидалась константа как индекс";
                            currentState = OperandState.ErrorState;
                        }
                        break;
                    case OperandState.BracketIdentState:
                        if (pointer == ' ')
                        {
                            isSpaceBetweenIdAnBr = true;
                        }
                        else if (pointer == ')')
                        {
                            if (response.Identificators.ContainsKey(curWord))
                            {
                                error = $"Семантическая ошибка \nидентификатор уже был использован как {response.Identificators[curWord]}";
                                currentState = OperandState.ErrorState;
                                break;
                            }
                            response.Identificators.Add($"{curWord}", "идентификатор-индекс");
                            curWord = string.Empty;
                            currentState = OperandState.FinishState;
                        }
                        else if (pointer != ' ' && isSpaceBetweenIdAnBr)
                        {
                            error = "лишний символ в массиве";

                            currentState = OperandState.ErrorState;
                        }
                        else if (char.IsLetterOrDigit(pointer))
                        {
                            curWord += pointer;
                        }
                        else
                        {
                            error = "ожидался идентификатор как индекс";
                            currentState = OperandState.ErrorState;
                        }
                        break;
                }
            }


            if (error != string.Empty)
            {
                return SetResponse(true, error, i);
            }

            return response;
        }
    
        private bool IsOperation(string symbol)
        {
            string[] operations = ["+", "-", "/", "*"];
            return operations.Contains(symbol);
        }
    
    }
}
