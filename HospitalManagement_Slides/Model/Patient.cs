namespace HospitalManagement.Model
{
    public class Patient
    {
        public int id;
        public string name;
        public int age;
        public string disease;

        // Parameterized constructor: Week 2 Lecture 2.
        public Patient(int id, string name, int age, string disease)
        {
            this.id = id;
            this.name = name;
            this.age = age;
            this.disease = disease;
        }

        // Copy constructor: Week 3 Lecture 1.
        // History needs a separate object so updates do not change old history.
        public Patient(Patient p)
        {
            id = p.id;
            name = p.name;
            age = p.age;
            disease = p.disease;
        }
    }
}
