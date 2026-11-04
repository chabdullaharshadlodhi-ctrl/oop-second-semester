using System.Collections.Generic;
using HospitalManagement.Model;
using HospitalManagement.Repositories;
using HospitalManagement.Common;

namespace HospitalManagement.Services
{
    public class PatientService
    {
        // Service = BL
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

        /*
         public static List<patients> paginatedView 
       
         
         
         
         */

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


        public static bool matchesText(string value, string query, int mode)
        {
            if (value == null || query == null || query.Trim() == "") return false;
            value = value.Trim().ToLower();
            query = query.Trim().ToLower();
            if (mode == 1) return value == query;
            if (mode == 2) return value.Contains(query);
            if (mode == 3) return value.StartsWith(query);
            if (mode == 4) return value.EndsWith(query);
            return false;
        }

        public static List<Patient> filterText(string query, int mode, bool useDisease)
        {
            List<Patient> result = new List<Patient>();
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                Patient p = PatientRepository.patients[x];
                string value = p.name;
                if (useDisease) value = p.disease;
                if (matchesText(value, query, mode)) result.Add(p);
            }
            return result;
        }
        

        // Inclusive bounds. Exact value: pass the same number as both bounds.
        public static List<Patient> filterRange(int minimum, int maximum, bool useAge)
        {
            List<Patient> result = new List<Patient>();
            if (minimum < 0 || maximum < minimum) return result;
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                Patient p = PatientRepository.patients[x];
                int value = p.id;
                if (useAge) value = p.age;
                if (value >= minimum && value <= maximum) result.Add(p);
            }
            return result;
        }

        public static List<Patient> filterDiseaseAndAge(string disease, int minimum, int maximum, bool useOr)
        {
            List<Patient> result = new List<Patient>();
            if (disease == null || disease.Trim() == "" || minimum < 0 || maximum < minimum) return result;
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                Patient p = PatientRepository.patients[x];
                bool diseaseMatches = matchesText(p.disease, disease, 1);
                bool ageMatches = p.age >= minimum && p.age <= maximum;
                if (useOr)
                {
                    if (diseaseMatches || ageMatches) result.Add(p);
                }
                else if (diseaseMatches && ageMatches) result.Add(p);
            }
            return result;
        }

        public static List<Patient> filterTwoDiseases(string first, string second)
        {
            List<Patient> result = new List<Patient>();
            if (first == null || second == null || first.Trim() == "" || second.Trim() == "") return result;
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                Patient p = PatientRepository.patients[x];
                if (matchesText(p.disease, first, 1) || matchesText(p.disease, second, 1)) result.Add(p);
            }
            return result;
        }

        public static List<Patient> filterWithoutDisease(string disease)
        {
            List<Patient> result = new List<Patient>();
            if (disease == null || disease.Trim() == "") return result;
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                Patient p = PatientRepository.patients[x];
                if (!matchesText(p.disease, disease, 1)) result.Add(p);
            }
            return result;
        }

        public static int totalAge(List<Patient> records)
        {
            int total = 0;
            for (int x = 0; x < records.Count; x++) total = total + records[x].age;
            return total;
        }

        public static double averageAge(List<Patient> records)
        {
            if (records.Count == 0) return 0;
            return (double)totalAge(records) / records.Count;
        }

        // Return every tied youngest/oldest patient, not just the first one.
        public static List<Patient> extremeAge(List<Patient> records, bool oldest)
        {
            List<Patient> result = new List<Patient>();
            if (records.Count == 0) return result;
            int best = records[0].age;
            for (int x = 1; x < records.Count; x++)
            {
                if (oldest && records[x].age > best) best = records[x].age;
                if (!oldest && records[x].age < best) best = records[x].age;
            }
            for (int x = 0; x < records.Count; x++)
                if (records[x].age == best) result.Add(records[x]);
            return result;
        }

        // Bubble sort a separate list; the original patient order stays the same.
        public static List<Patient> sortFiltered(List<Patient> records, bool byName, bool descending)
        {
            List<Patient> result = new List<Patient>();
            for (int x = 0; x < records.Count; x++) result.Add(records[x]);
            for (int pass = 0; pass < result.Count - 1; pass++)
            {
                for (int x = 0; x < result.Count - 1 - pass; x++)
                {
                    int comparison = result[x].age.CompareTo(result[x + 1].age);
                    if (byName) comparison = string.Compare(result[x].name, result[x + 1].name, true);
                    bool swap = comparison > 0;
                    if (descending) swap = comparison < 0;
                    if (swap)
                    {
                        Patient temporary = result[x];
                        result[x] = result[x + 1];
                        result[x + 1] = temporary;
                    }
                }
            }
            return result;
        }

        public static int deleteFiltered(List<Patient> records, string confirmation)
        {
            if (confirmation != "YES") return 0;
            List<int> ids = new List<int>();
            for (int x = 0; x < records.Count; x++) ids.Add(records[x].id);
            int deleted = 0;
            for (int x = 0; x < ids.Count; x++)
            {
                if (findById(ids[x]) != null)
                {
                    deletePatient(ids[x]);
                    deleted++;
                }
            }
            return deleted;
        }
    }
}
