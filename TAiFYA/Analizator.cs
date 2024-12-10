using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace TAiFYA
{
    internal class Analizator : AnalizatorBase
    {
        private enum State 
        {
            StartState,
            DoState,
            Space1State,
            WhileState,
            Space2State,
            BracketOpenState,
            ConditionState,
            BracketCloseState,
            CommaState,
            FinishState,
            ErrorState
        }

        public Analizator()
        {

        }

        public override string GetInputText => inputText;

        public override string ChangeInput(string newInput)
        {
            inputText = newInput;
            return inputText;
        }

        public override AnalizatorResponse StartProgram()
        {
            AnalizatorResponse response = new AnalizatorResponse();

            if (string.IsNullOrEmpty(inputText))
            {
                return SetResponse(true, "Пустая строка");
            }

            response.InputField = inputText;
            string prevText = inputText;
            inputText = inputText.ToLower();

            int i = -1;
            var currentState = State.StartState;
            string text = inputText + ' ';
            string curWord = string.Empty;
            string wordExpected = string.Empty;
            string error = string.Empty;

            while (!(currentState == State.FinishState || currentState == State.ErrorState))
            {
                i++;

                if (i >= text.Length)
                {
                    if (currentState != State.FinishState || currentState != State.ErrorState)
                    {
                        return SetResponse(true, "не обнаружен конец строки", i);
                    }
                    return SetResponse(true, "", i);
                }
                pointer = text[i];
                

                switch (currentState)
                {
                    case State.StartState:
                        if (pointer == ' ')
                            continue;
                        else if (pointer == 'd')
                        {
                            wordExpected = "do";
                            curWord += pointer;
                            currentState = State.DoState;
                        }
                        else
                        {
                            error = "Строка пустая";
                            currentState = State.ErrorState;
                        }
                        break;

                    case State.DoState:
                        if (pointer == ' ')
                        {
                            if (curWord.ToLower() == wordExpected)
                            {
                                wordExpected = " ";
                                curWord = string.Empty;
                                currentState = State.Space1State;
                            }
                            else
                            {
                                currentState = State.ErrorState;
                                error = $"Ожидалось слово DO, получили {curWord}";
                            }
                        }
                        else
                        {
                            curWord += pointer;
                        }
                        break;

                    case State.Space1State:
                        if (pointer != ' ')
                        {
                            wordExpected = "while";
                            curWord += pointer;
                            currentState = State.WhileState;
                        }
                        break;

                    case State.WhileState:
                        if (pointer == ' ')
                        {
                            if (curWord.ToLower() == wordExpected)
                            {
                                wordExpected = " ";
                                curWord = string.Empty;
                                currentState = State.Space2State;
                            }
                            else
                            {
                                currentState = State.ErrorState;
                                error = $"Ожидалось слово WHILE, получили {curWord}";
                            }
                        }
                        else
                        {
                            curWord += pointer;
                        }
                        break;

                    case State.Space2State:
                        if (pointer != ' ')
                        {
                            wordExpected = "(";
                            curWord += pointer;
                            currentState = State.BracketOpenState;
                        }
                        
                        break;

                    case State.BracketOpenState:
                        if (pointer != ' ')
                        {
                            if (curWord.ToString() == wordExpected.ToString())
                            {
                                wordExpected = string.Empty;
                                curWord = pointer.ToString();
                                currentState = State.ConditionState;
                            }
                            else
                            {
                                currentState = State.ErrorState;
                                error = $"Ожидалась скобка (, получили {curWord}";
                            }
                        }
                        else if (pointer == ')')
                        {
                            error = "Не закрытая скобка";
                            currentState = State.ErrorState;
                        }
                        else
                        {
                            //curWord += pointer;
                        }
                        break;

                    case State.ConditionState:
                        if (pointer == ')')
                        {
                            var closesI = inputText.LastIndexOf(')');
                            if (i != closesI)
                            {
                                curWord += pointer;
                                break;
                            }
                            Condition condition = new();
                            condition.ChangeInput( curWord);
                            var resCondition = condition.StartProgram();
                            if (!resCondition.IsSuccess)
                            {
                                error = resCondition.Message;
                                i += resCondition.ErrorIndex;
                                currentState = State.ErrorState;
                                break;
                            }
                            else
                            {
                                response.Constants.AddRange(resCondition.Constants);
                                response.Identificators.AddRange(resCondition.Identificators);
                            }
                            // take cur word into another state machine for conditions
                            currentState = State.BracketCloseState;
                        }
                        else
                        {
                            // need any checks if goes too long 
                            curWord += pointer;
                        }
                        break;

                    case State.BracketCloseState:
                        if (pointer == ' ')
                            continue;
                        if (pointer == ';')
                            currentState = State.FinishState;
                        else
                        {
                            error = "Ожидалось ;";
                            currentState = State.ErrorState;

                        }
                        break;
                }

            }

            if (currentState == State.ErrorState || error != string.Empty)
            {
                return SetResponse(true, error, i);
            }

            return response;
        }
    }
}
