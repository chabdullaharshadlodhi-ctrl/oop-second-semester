using System;
using HospitalManagement.Common;

namespace HospitalManagement.UI
{
    public class ConsoleUtility
    {
        public static void showMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static void pause()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        public static string readText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = Console.ReadLine();
                if (FileHelper.validText(text))
                    return text;
                Console.WriteLine("Enter non-blank text without commas.");
            }
        }

        public static int readNumber(string prompt)
        {
            // A small loop checks the characters before using int.Parse.
            // Nine digits fit in int, so an enormous input cannot overflow.
            while (true)
            {
                Console.Write(prompt);
                string text = Console.ReadLine();
                bool valid = true;
                if (text == null || text == "")
                    valid = false;
                else if (text.Length > 9)
                    valid = false;
                else
                {
                    for (int x = 0; x < text.Length; x++)
                    {
                        if (text[x] < '0' || text[x] > '9')
                            valid = false;
                    }
                }
                if (valid)
                    return int.Parse(text);
                Console.WriteLine("Enter a non-negative whole number (maximum 9 digits).");
            }
        }
    }
}
