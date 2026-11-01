namespace HospitalManagement.Common
{
    public class FileHelper
    {
        // Manual comma counter from Week 1 Lecture 2, page 20.
        public static string parseData(string record, int field)
        {
            int comma = 1;
            string item = "";
            for (int x = 0; x < record.Length; x++)
            {
                if (record[x] == ',')
                    comma++;
                else if (comma == field)
                    item = item + record[x];
            }
            return item;
        }

        // Only loops and conditions: commas would break our file format.
        public static bool validText(string text)
        {
            if (text == null || text == "")
                return false;
            bool hasCharacter = false;
            for (int x = 0; x < text.Length; x++)
            {
                if (text[x] == ',' || text[x] == '\n' || text[x] == '\r')
                    return false;
                if (text[x] != ' ' && text[x] != '\t')
                    hasCharacter = true;
            }
            return hasCharacter;
        }
    }
}
