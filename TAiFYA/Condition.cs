using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAiFYA
{
    internal class Condition : AnalizatorBase
    {
        private enum State
        {
            StartState,
            ExpressionState1,
            OperationState,
            ExpressionState2,
            FinishState,
            ErrorState
        }

        char[] operationExpressions = ['+', '-', '/', '*'];
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
                return SetResponse(true,"Пустая строка");
            }

            response.InputField = inputText;

            int i = -1;
            var currentState = State.StartState;
            string text = inputText;
            string curWord = string.Empty;
            string wordExpected = string.Empty;
            string error = string.Empty;
            int errorIndex = 0;

            while (!(currentState == State.FinishState || currentState == State.ErrorState))
            {
                i++;

                if (i >= text.Length)
                {
                    if (currentState == State.ExpressionState1 || currentState == State.ExpressionState2)
                    {
                        // end there
                        // expression class
                        var expressionAnal = new Expression();
                        expressionAnal.ChangeInput(curWord);
                        var expRes = expressionAnal.StartProgram();
                        if (expRes.IsSuccess)
                        {
                            //response.Identificators.AddRange(expRes.Identificators);
                            //response.Constants.AddRange(expRes.Constants);
                        }
                        else
                        {
                            error = expRes.Message;
                            errorIndex += expRes.ErrorIndex;
                            response.IsSuccess = false;
                        }
                        break;
                    }
                    else if (currentState != State.FinishState || currentState != State.ErrorState)
                    {
                        error = $"Ошибка синтаксиса";
                        break;
                    }
                    else
                    {
                        currentState = State.ErrorState;
                        error = "Ошибка размера строки";
                        break;
                    }
                }
                pointer = text[i];

                switch (currentState)
                {
                    case State.StartState:
                        if (pointer != ' ')
                        {
                            curWord += pointer;
                            errorIndex = i;
                            currentState = State.ExpressionState1;
                        }
                        break;
                    case State.ExpressionState1:
                        if (IsAllowedOperation(pointer))
                        {
                            //Expression Class
                            var expressionAnal = new Expression();
                            expressionAnal.ChangeInput(curWord);
                            var expRes = expressionAnal.StartProgram();
                            if (expRes.IsSuccess)
                            {
                                //response.Identificators.AddRange(expRes.Identificators);
                                //response.Constants.AddRange(expRes.Constants);
                                curWord = string.Empty;
                                currentState = State.OperationState;

                            }
                            else
                            {
                                //error = expRes.Message;
                                //i += expRes.ErrorIndex;
                                //response.IsSuccess = false;
                                currentState = State.ErrorState;
                                break;
                            }
                        }
                        else
                        {
                            curWord += pointer;
                        }
                        break;
                    case State.OperationState:
                        errorIndex = i;
                        currentState = State.ExpressionState2;
                        break;
                    case State.ExpressionState2:
                        curWord += pointer;
                        break;
                    case State.ErrorState:
                        break;
                    case State.FinishState:
                        break;
                }


            }

            if (currentState == State.ErrorState || error != string.Empty)
            {
                return SetResponse(true, error, i);
            }

            return response;

        }
        private bool IsAllowedOperation(char operation)
        {
            char[] operations = ['>', '<', '#', '='];
            return operations.Contains(operation);
        }

    }
}
