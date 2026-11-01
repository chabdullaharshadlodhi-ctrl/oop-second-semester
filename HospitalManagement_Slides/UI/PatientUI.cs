using System;
using System.Collections.Generic;
using HospitalManagement.Model;
using HospitalManagement.Services;

namespace HospitalManagement.UI
{
    public class PatientUI
    {
        public static void adminMenu()
        {
            while (true)
            {
                Console.WriteLine("\nADMIN MENU");
                Console.WriteLine("1. Show all patients");
                Console.WriteLine("2. Search patient by name");
                Console.WriteLine("3. Update patient information");
                Console.WriteLine("4. Delete patient");
                Console.WriteLine("5. Add patient");
                Console.WriteLine("6. View last 10 added records");
                Console.WriteLine("7. Logout");
                Console.Write("Choose option: ");
                string option = Console.ReadLine();

                if (option == "1")
                    showPatients(PatientService.getAllPatients());
                else if (option == "2")
                {
                    string name = ConsoleUtility.readText("Enter exact patient name: ");
                    showPatients(PatientService.searchByName(name));
                }
                else if (option == "3")
                    updatePatient();
                else if (option == "4")
                    deletePatient();
                else if (option == "5")
                {
                    int id = ConsoleUtility.readNumber("Enter patient ID: ");
                    Patient p = takePatientInput(id);
                    Console.WriteLine(PatientService.addPatient(p));
                }
                else if (option == "6")
                    showPatients(PatientService.getHistory());
                else if (option == "7")
                {
                    Console.WriteLine("Logged out.");
                    break;
                }
                else
                    Console.WriteLine("Invalid option.");
                ConsoleUtility.pause();
            }
        }

        public static Patient takePatientInput(int id)
        {
            string name = ConsoleUtility.readText("Enter name: ");
            int age = ConsoleUtility.readNumber("Enter age: ");
            string disease = ConsoleUtility.readText("Enter disease: ");
            Patient p = new Patient(id, name, age, disease);
            return p;
        }

        public static void updatePatient()
        {
            int id = ConsoleUtility.readNumber("Patient ID to update: ");
            Patient old = PatientService.findById(id);
            if (old == null)
            {
                Console.WriteLine("Patient not found.");
                return;
            }
            Console.WriteLine("Old record:");
            showPatient(old);
            // Keep the same ID so the patient's login still points to this record.
            Patient updated = takePatientInput(id);
            Console.WriteLine(PatientService.updatePatient(updated));
        }

        public static void deletePatient()
        {
            int id = ConsoleUtility.readNumber("Patient ID to delete: ");
            Patient p = PatientService.findById(id);
            if (p == null)
            {
                Console.WriteLine("Patient not found.");
                return;
            }
            showPatient(p);
            Console.Write("Delete this patient and their login account? Enter Y: ");
            string answer = Console.ReadLine();
            if (answer == "Y" || answer == "y")
                Console.WriteLine(PatientService.deletePatient(id));
            else
                Console.WriteLine("Deletion cancelled.");
        }

        public static void patientMenu(MUser user)
        {
            while (true)
            {
                Console.WriteLine("\nPATIENT MENU");
                Console.WriteLine("1. View my profile");
                Console.WriteLine("2. Update my name and age");
                Console.WriteLine("3. Logout");
                Console.Write("Choose option: ");
                string option = Console.ReadLine();
                if (option == "1")
                    showPatient(PatientService.findById(user.patientId));
                else if (option == "2")
                {
                    string name = ConsoleUtility.readText("New name: ");
                    int age = ConsoleUtility.readNumber("New age: ");
                    Console.WriteLine(PatientService.selfUpdate(user.patientId, name, age));
                }
                else if (option == "3")
                {
                    Console.WriteLine("Logged out.");
                    break;
                }
                else
                    Console.WriteLine("Invalid option.");
                ConsoleUtility.pause();
            }
        }

        public static void showPatients(List<Patient> records)
        {
            if (records.Count == 0)
            {
                Console.WriteLine("No records found.");
                return;
            }
            Console.WriteLine("ID | Name | Age | Disease");
            for (int x = 0; x < records.Count; x++)
            {
                Patient p = records[x];
                Console.WriteLine(p.id + " | " + p.name + " | " + p.age + " | " + p.disease);
            }
        }

        public static void showPatient(Patient p)
        {
            if (p == null)
            {
                Console.WriteLine("Patient record not found.");
                return;
            }
            Console.WriteLine("ID: " + p.id);
            Console.WriteLine("Name: " + p.name);
            Console.WriteLine("Age: " + p.age);
            Console.WriteLine("Disease: " + p.disease);
        }
    }
}
