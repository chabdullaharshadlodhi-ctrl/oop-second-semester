using System.Collections.Generic;
using System.IO;
using HospitalManagement.Model;
using HospitalManagement.Common;

namespace HospitalManagement.Repositories
{
    public class PatientRepository
    {
        // Repository = DL. It owns the lists and handles text files.
        public static List<Patient> patients = new List<Patient>();
        public static List<Patient> history = new List<Patient>();

        public static void loadData()
        {
            patients = readFile("patients.txt");
            history = readFile("history.txt");
        }

        public static List<Patient> readFile(string path)
        {
            List<Patient> records = new List<Patient>();
            if (File.Exists(path))
            {
                StreamReader file = new StreamReader(path);
                string record;
                while ((record = file.ReadLine()) != null)
                {
                    if (record != "")
                    {
                        int id = int.Parse(FileHelper.parseData(record, 1));
                        string name = FileHelper.parseData(record, 2);
                        int age = int.Parse(FileHelper.parseData(record, 3));
                        string disease = FileHelper.parseData(record, 4);
                        Patient p = new Patient(id, name, age, disease);
                        records.Add(p);
                    }
                }
                file.Close();
            }
            return records;
        }

        public static void savePatients()
        {
            writeFile("patients.txt", patients);
        }

        public static void saveHistory()
        {
            writeFile("history.txt", history);
        }

        public static void writeFile(string path, List<Patient> records)
        {
            // false overwrites: the file will match the updated list.
            StreamWriter file = new StreamWriter(path, false);
            for (int x = 0; x < records.Count; x++)
            {
                Patient p = records[x];
                file.WriteLine(p.id + "," + p.name + "," + p.age + "," + p.disease);
            }
            file.Close();
        }
    }
}
