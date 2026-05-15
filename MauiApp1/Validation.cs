using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class Validation
    {
        private static string PatternFIO1 = @"^[А-ЯЁ][а-яё]+(-[А-ЯЁ][а-яё]+)? [А-ЯЁ]\.+([А-ЯЁ]\.)?$";

        private static string PatternFIO2 = @"^[А-ЯЁ][а-яё]+(-[А-ЯЁ][а-яё]+)? [А-ЯЁ][а-яё]+( [А-ЯЁ][а-яё]+)?$";

        public static bool ValidationFIO(string input, out string? error)
        {            
            if(string.IsNullOrWhiteSpace(input))
            {
                error = "Есть незаполненные поля";
                return false;
            }

            input = input.TrimEnd();

            if (input.Length > 49)
            {
                error = $"Слишком длинное ФИО {input}\nНе больше 50 символов";
                return false;
            }

            if (Regex.IsMatch(input, PatternFIO1) || Regex.IsMatch(input, PatternFIO2))
            {
                error = null;

                return true;
            }
            else
            {
                error = $"Некорректное ФИО {input}\nВозможные форматы \"Иванов А.В. или Иванов Александр Викторович\"\nПлюс двойные фамилии \"Салтыков-Щедрин Михаил Евграфович или Салтыков-Щедрин М.Е.\"\nТакже убирайте лишние пробелы, людей без отчества вводить по тем же правилам, но не прописывая отчество";

                return false;
            }
        }

        public static bool ValidationNumber(string number, out string? error)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                error = "Есть незаполненные номера";
                return false;
            }

            number = number.TrimEnd();

            int num;

            if(!int.TryParse(number, out num))
            {
                error = $"Некорректный номер { number }";
                return false;
            }

            if(num > 99)
            {
                error = $"Слишком длинный номер {number}";
                return false;
            }

            error = null;

            return true;
        }
    }
}
