using System.Collections.Generic;
using System.IO;
using HospitalManagement.Model;
using HospitalManagement.Common;

namespace HospitalManagement.Repositories
{
    public class UserRepository
    {
        public static List<MUser> users = new List<MUser>();

        public static void loadData()
        {
            users = new List<MUser>();
            if (File.Exists("users.txt"))
            {
                StreamReader file = new StreamReader("users.txt");
                string record;
                while ((record = file.ReadLine()) != null)
                {
                    if (record != "")
                    {
                        string name = FileHelper.parseData(record, 1);
                        string password = FileHelper.parseData(record, 2);
                        string role = FileHelper.parseData(record, 3);
                        int id = int.Parse(FileHelper.parseData(record, 4));
                        users.Add(new MUser(name, password, role, id));
                    }
                }
                file.Close();
            }
        }

        public static void saveUsers()
        {
            StreamWriter file = new StreamWriter("users.txt", false);
            for (int x = 0; x < users.Count; x++)
            {
                MUser u = users[x];
                file.WriteLine(u.name + "," + u.password + "," + u.role + "," + u.patientId);
            }
            file.Close();
        }
    }
}
