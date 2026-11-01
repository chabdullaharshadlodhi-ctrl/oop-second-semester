namespace HospitalManagement.Model
{
    public class MUser
    {
        public string name;
        public string password;
        public string role;
        public int patientId;

        // Same user-and-role idea 
        // patientId connects a patient account to its patient record.
        public MUser(string name, string password, string role, int patientId)
        {
            this.name = name;
            this.password = password;
            this.role = role;
            this.patientId = patientId;
        }

        public bool isAdmin()
        {
            if (role == "Admin")
                return true;
            return false;
        }
    }
}
 //here is lpgin account structure.