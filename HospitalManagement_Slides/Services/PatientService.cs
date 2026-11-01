using System.Collections.Generic;
using HospitalManagement.Model;
using HospitalManagement.Repositories;
using HospitalManagement.Common;

namespace HospitalManagement.Services
{
    public class PatientService
    {
        // Service = BL. No Console input/output and no StreamReader here.
        public static void loadData()
        {
            PatientRepository.loadData();
        }

        public static List<Patient> getAllPatients()
        {
            return PatientRepository.patients;
        }

        public static List<Patient> getHistory()
        {
            return PatientRepository.history;
        }

        public static Patient findById(int id)
        {
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                if (PatientRepository.patients[x].id == id)
                    return PatientRepository.patients[x];
            }
            return null;
        }

        public static List<Patient> searchByName(string name)
        {
            List<Patient> result = new List<Patient>();
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                if (PatientRepository.patients[x].name == name)
                    result.Add(PatientRepository.patients[x]);
            }
            return result;
        }

        public static bool validPatient(Patient p)
        {
            if (p.id <= 0 || p.age < 0 || p.age > 130)
                return false;
            if (!FileHelper.validText(p.name) || !FileHelper.validText(p.disease))
                return false;
            return true;
        }

        public static string addPatient(Patient p)
        {
            if (!validPatient(p))
                return "Invalid record. Check ID, age, name and disease.";
            if (PatientRepository.patients.Count >= 100)
                return "System full: maximum 100 patients.";
            if (findById(p.id) != null)
                return "This patient ID already exists.";

            PatientRepository.patients.Add(p);
            // Insert and RemoveAt are covered in Week 2 Lecture 3.
            PatientRepository.history.Insert(0, new Patient(p));
            if (PatientRepository.history.Count > 10)
                PatientRepository.history.RemoveAt(10);
            PatientRepository.savePatients();
            PatientRepository.saveHistory();
            return "Patient added and saved.";
        }

        public static string updatePatient(Patient updated)
        {
            Patient stored = findById(updated.id);
            if (stored == null)
                return "Patient not found.";
            if (!validPatient(updated))
                return "Invalid record. Check age, name and disease.";

            stored.name = updated.name;
            stored.age = updated.age;
            stored.disease = updated.disease;
            PatientRepository.savePatients();
            return "Patient updated and saved.";
        }

        public static string selfUpdate(int id, string name, int age)
        {
            Patient stored = findById(id);
            if (stored == null)
                return "Patient record not found.";
            // Keep the stored diagnosis; patients only change name and age.
            Patient updated = new Patient(id, name, age, stored.disease);
            return updatePatient(updated);
        }

        public static string deletePatient(int id)
        {
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                if (PatientRepository.patients[x].id == id)
                {
                    PatientRepository.patients.RemoveAt(x);
                    PatientRepository.savePatients();
                    UserService.removePatientAccount(id);
                    return "Patient and linked account deleted.";
                }
            }
            return "Patient not found.";
        }
    }
}
