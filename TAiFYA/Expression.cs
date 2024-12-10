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
            BracketConstantState,
            BracketIdentState,
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
            AnalizatorResponse response = new();
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

            string[] operations = ["+", "-", "*", "/"];

            while (!(currentState == State.FinishState || currentState == State.ErrorState))
            {
                if (i >= inputText.Length)
                {
                    if (currentState == State.OperationState)
                    {
                        
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
                            }
                            else
                            {
                                response.Identificators.AddRange(oper1res.Identificators);
                                response.Constants.AddRange(oper1res.Constants);
                            }
                            currentState = State.OperationState;
                        }
                        else if (char.IsLetterOrDigit(pointer))
                        {
                            curWord += pointer;
                        }
                        else if (pointer == ' ')
                        {

                        }
                        else
                        {
                            error = $"ожидался символ операции получен: {pointer}";
                            currentState = State.ErrorState;
                        }
                        break;
                    case State.OperationState:
                        if (pointer == ' ')
                        {

                        }
                        else if (IsOperation(pointer.ToString()))
                        {
                            currentState = State.OperandState2;
                        }
                        else
                        {
                           
                        }
                        break;
                    case State.OperandState2:
                        if (pointer == ' ')
                        {

                        }
                        else
                        {

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

        private AnalizatorResponse OperandCheck(string word)
        {
            AnalizatorResponse response = new()
            {
                
            };
            int i = -1;
            int lastIndex = word.Length;
            string error = string.Empty;
            char pointer;
            string curWord = string.Empty;
            var currentState = OperandState.StartState;

            while (currentState != OperandState.FinishState && currentState != OperandState.ErrorState)
            {
                i++;
                if (i >= lastIndex)
                {
                    if (currentState == OperandState.ConstantState)
                    {
                        response.Constants.Add(curWord);
                    }
                    else if (currentState == OperandState.IdentState)
                    {
                        response.Identificators.Add(curWord);
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
                        break;
                    case OperandState.IdentState:
                        if (pointer == '(')
                        {
                            response.Constants.Add($"{curWord} - константа-массив");
                            curWord = string.Empty;
                            if (i >= lastIndex)
                            { 
                                currentState = OperandState.ErrorState;
                                break;
                            } 
                            i++;
                            pointer = word[i];
                            if (char.IsDigit(pointer))
                            {
                                curWord += pointer;
                                currentState = OperandState.BracketConstantState;
                            }
                            else if (char.IsLetter(pointer))
                            {
                                curWord += pointer;
                                currentState = OperandState.BracketIdentState;
                            }
                            else
                            {
                                error = "В скобках должно что то быть!";
                                currentState = OperandState.ErrorState;
                            }
                        }
                        else if (pointer == ' ')
                        {
                            continue;
                        }
                        else
                        {
                            curWord += pointer;
                        }
                        break;
                    case OperandState.ConstantState:
                        if (!char.IsDigit(pointer))
                        {
                            error = "Ожидалась константа";
                            currentState = OperandState.ErrorState;
                        }
                        else
                        {
                            curWord += pointer;
                        }
                        break;
                    case OperandState.BracketIdentState:
                        if (pointer == ')')
                        {
                            response.Identificators.Add($"{curWord} - идентификатор-массива");
                            currentState = OperandState.FinishState;
                        }
                        else if (!char.IsLetterOrDigit(pointer))
                        {
                            error = "В массиве не константа и не идентификатор";
                            currentState = OperandState.ErrorState;
                        }
                        else
                        {
                            curWord += pointer;
                        }
                        break;
                    case OperandState.BracketConstantState:
                        if (pointer == ')')
                        {
                            response.Identificators.Add($"{curWord} - константа-массива");
                            currentState = OperandState.FinishState;
                        }
                        else if (!char.IsDigit(pointer))
                        {
                            error = "В массиве не константа и не идентификатор";
                            currentState = OperandState.ErrorState;
                        }
                        else
                        {
                            curWord += pointer;
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
