# Your hospital project, following your slides

This is the conversion of the supplied hmsfinal(1).cpp, rewritten around the concepts in your Week 1-5 slides. Start with START_HERE.txt. The program is a console application, like your teacher's examples.

## Which architecture did we follow?

There are two different structures in your uploads:

- Lab 5, page 3: BL, DL and UI classes, plus a driver.
- Week 5 Lecture 2, pages 6 and 8: four layers named Model, Repositories, Services and UI. Page 20 asks you to implement scenarios using this four-tier model.

This project follows the later four-layer instruction. **Services are BL, and Repositories are DL.** The patient data class is in Model, as shown on page 14. Program.cs is the entry point, not an additional layer. Common contains a shared helper, as permitted on page 11.

These are folders/classes in one Visual Studio project, not four separate projects.

| Part | Files | Simple meaning |
|---|---|---|
| Model | Patient.cs, MUser.cs | Patient/user ki information |
| Repositories / DL | PatientRepository.cs, UserRepository.cs | Lists aur files ko handle karna |
| Services / BL | PatientService.cs, UserService.cs | Searching, checking rules, add/update/delete |
| UI | PatientUI.cs, UserUI.cs, ConsoleUtility.cs | Screen par output aur keyboard input |
| Entry point | Program.cs | Data load karna aur correct menu start karna |
| Shared helper | FileHelper.cs | Comma-separated line se field nikalna |

No Console input/output is inside a model, service or repository. Only repositories use System.IO. UI never directly reads or writes files. This also follows the 'no input/output in BL' instruction in Week 4 Lecture 3, page 13.

## Concepts and their slide sources

Page numbers below are PDF page numbers, starting at 1.

| Concept used | Where your teacher covers it |
|---|---|
| Console project and Visual Studio | Week 1 Lecture 1, pages 3-20 |
| Variables, input, conditions and loops | Week 1 Lecture 1, pages 30-58 |
| Functions | Week 1 Lecture 2, pages 1-5 |
| File.Exists, StreamReader, StreamWriter, Close | Week 1 Lecture 2, pages 7-22 |
| Manual parseData comma counter | Week 1 Lecture 2, page 20 |
| Public fields and object creation | Week 2 Lecture 1, pages 15-29 |
| Parameterized constructors | Week 2 Lecture 2, pages 26-39 |
| List, Add, Count, indexing, Insert, RemoveAt | Week 2 Lecture 3, pages 12-19 |
| Sign-in/sign-up, roles, admin CRUD, text files | Week 2 Lecture 3, page 31 |
| Copy constructor for history | Week 3 Lecture 1, pages 19-25 |
| MUser and isAdmin | Week 3 Lecture 3, page 30 |
| Read files once before the main loop | Week 3 Lecture 3, pages 28 and 38 |
| Multiple classes working together | Week 4 Lecture 2, pages 12-24 |
| Static fields/methods | Week 5 Lecture 1, pages 5-18 |
| Four layers, common helper, repository/service/UI | Week 5 Lecture 2, pages 6-18 |

The supplied Week 5 example includes some shorthand. This conversion uses explicit types instead of var, string concatenation instead of interpolation, and ordinary WriteLine calls instead of overriding ToString. Static lists/methods come from Week 5 Lecture 1, and avoid having to set up service/repository objects. The four layers and responsibilities still follow Lecture 2.

The code does not use LINQ, lambdas, interfaces, inheritance, properties, async, dependency injection, try/catch, TryParse, Split, AppDomain, automatic directory creation or a database. The .sln/.csproj/App.config files are Visual Studio project configuration; you do not need to explain their XML as application logic.

## Your existing features

| Original C++ feature | C# location |
|---|---|
| Show all patients | PatientService.getAllPatients + PatientUI.showPatients |
| Search by name | PatientService.searchByName |
| Update patient | PatientService.updatePatient |
| Delete patient | PatientService.deletePatient |
| Add patient | PatientService.addPatient |
| Last 10 added records | PatientService.getHistory |
| View own profile | PatientUI.patientMenu + PatientService.findById |
| Update own name/age | PatientService.selfUpdate |
| Admin login | UserService.signIn and MUser.isAdmin |
| Logout | break from the relevant menu loop |

Your original ten patient records and five initial history records are included. The 100-patient limit remains.

## Changes made for your slides

1. Parallel arrays became Patient objects in a List<Patient>.
2. User accounts and roles are saved in users.txt, following Week 2 Lecture 3's conversion task.
3. Sign-up creates a Patient account for an existing patient record. The provided admin account remains admin/123.
4. Each patient account has patientId. After login, the menu uses that ID to show/update its own record.
5. Update/delete use ID instead of name because names can repeat. Name search displays every exact match.
6. IDs cannot be changed during updates because the user account is linked to the ID.
7. Deleting a current patient also removes their linked login account. Historical addition snapshots remain.
8. The patient menu repeats until logout. The original repeated keypress pauses were removed.
9. The option that displayed a current patient record is named 'View my profile'. It is not a visit history.

## Understand a Patient object

```csharp
Patient p = new Patient(11, "Ali Khan", 20, "Fever");
```

Patient is the class/type. p is the reference variable. new creates an object and calls the constructor. The constructor puts the supplied values into id, name, age and disease.

```csharp
List<Patient> patients = new List<Patient>();
patients.Add(p);
```

This list stores Patient objects. `patients[0].name` gives the first patient's name. `patients.Count` gives the number of current patients, so we no longer maintain patientCount manually.

## Add Patient: explain this to your teacher

1. PatientUI reads ID, name, age and disease and creates a Patient object.
2. It calls PatientService.addPatient(p).
3. The service checks data, the 100-patient limit and whether the ID already exists.
4. The service adds p to PatientRepository.patients.
5. It creates a separate copy for history using new Patient(p).
6. The repository writes the updated lists into the text files.
7. The service returns a message; UI prints it.

**Roman Urdu:** UI input leti hai, Model object mein information rakhta hai, Service rules check karti hai, aur Repository data file mein save karti hai.

## Why copy the history object?

`history.Add(p)` would store a reference to the same patient object. Updating that patient's name could then change history too.

`new Patient(p)` calls the copy constructor from Week 3 Lecture 1. It creates a separate object containing the old values. We use this copy only where needed, for the historical snapshot.

History keeps the last 10 additions, newest additions inserted at index 0. The five supplied starter entries preserve your C++ order. It is not a log of visits or edits. Removing/updating a current patient does not change previous addition snapshots.

## Why static?

The repositories have one shared patient list, history list and user list. Static methods use those shared lists and are called by class name:

```csharp
PatientService.addPatient(p);
```

Each individual Patient and MUser is still a separate non-static object. Static was covered in Week 5 Lecture 1; it does not mean all patients share the same name or age.

## How the files work

| File | Format | Example |
|---|---|---|
| patients.txt | id,name,age,disease | 1,Ramzan,18,Flu |
| history.txt | id,name,age,disease | 1,Ramzan,18,Flu |
| users.txt | username,password,role,patientId | ramzan,123,Patient,1 |

Admin uses patientId 0 because an admin is not linked to a patient record.

Data loads once before the main loop, matching Week 3 Lecture 3's correction. Changes update the list and rewrite the corresponding file. StreamWriter(path, false) overwrites the old file; otherwise deleted records would come back on restart. Close ends the file operation and flushes the writer.

parseData counts commas exactly like your teacher's example. It does not use Split. Spaces inside a name or disease are allowed; commas are rejected because commas separate fields.

The Visual Studio launch working directory is set to the project folder, so the three visible text files are the normal working files when you run the solution. Copies are also included beside the built executable. If you run the EXE separately, it uses files in its current working directory; that may be a different copy. For consistent class demonstrations, run through the supplied solution.

## Input checking

readNumber uses a loop to check that every character is between '0' and '9', then calls int.Parse, as used in the slides. It accepts up to 9 digits, which fit in an int. It rejects negative signs, decimal points, letters and blanks. PatientService then checks that IDs are positive and ages are 0-130.

validText checks text with a loop: it cannot be blank, contain only spaces/tabs, or contain commas/newlines. These are applications of loops and conditions, not a new library or framework.

## How to demonstrate it

1. Sign in using admin / 123; show the ten initial patients.
2. Add ID 11, Ali Khan, age 20, Body Pain.
3. Search for Ali Khan.
4. Update ID 11 to age 21.
5. Logout, then Sign Up as ali / 123 with patient ID 11.
6. Sign in as ali / 123, view the profile, and change the name/age.
7. Exit and run again: the saved values should still be present.
8. Sign in as admin; view history. Ali Khan's addition snapshot still has its original age 20.
9. Delete ID 11. Both its current record and ali's login are removed, while its history snapshot stays.

## Classroom limits

The supplied file format is deliberately simple, like the slides. Do not manually damage the files: numeric fields are read with int.Parse, and file/permission errors are not handled with exceptions because that topic is outside this version's scope. Run one instance at a time. Saves are not transactional across multiple files.

Passwords are plain text, as in the slides. Sign-up accepts an unclaimed existing patient ID; knowing that ID does not prove real-world identity. This is a coursework demonstration using fictional data, not a production hospital authentication system.

## Verification performed

The application source compiled successfully with the Mono C# compiler, with zero warnings. Twenty-six service checks passed, covering sample data, authentication, duplicate IDs/usernames, validation, add/search/update/delete, account links, history copies, history limits, capacity and persistence. Console integration checks also passed, including a second process loading the saved files, invalid numeric input and three failed login attempts.

The Windows Visual Studio application was not available here. The solution/project XML, configuration and included-file paths were checked structurally. Mono is only a testing tool used during preparation; you do not need to install it to run this Visual Studio project.
