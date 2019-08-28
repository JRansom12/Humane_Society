using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "update":
                    var employeeToUpdate = db.Employees.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
                    employeeToUpdate.FirstName = employee.FirstName;
                    employeeToUpdate.LastName = employee.LastName;
                    employeeToUpdate.Email = employee.Email;
                    db.SubmitChanges();
                    break;
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "delete":
                    var employeeToDelete = db.Employees.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
                    db.Employees.DeleteOnSubmit(employeeToDelete);
                    db.SubmitChanges();
                    break;
                case "read":
                    var employeeToReturn = db.Employees.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
                    List<string> info = new List<string>() { "Name: " + employeeToReturn.FirstName + " " + employeeToReturn.LastName, "User Name: " + employeeToReturn.UserName, "Email: " + employeeToReturn.Email};
                    UserInterface.DisplayUserOptions(info);
                    Console.ReadLine();
                    break;
                default:
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal) //program test doesn't work
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            var animal = db.Animals.FirstOrDefault(a => a.AnimalId == id);
            return animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();
            foreach (KeyValuePair<int, string> el in updates)
            {
                switch (el.Key)
                {
                    case 1:
                        animal.CategoryId = int.Parse(el.Value);
                        break;
                    case 2:
                        animal.Name = el.Value;
                        break;
                    case 3:
                        animal.Age = int.Parse(el.Value);
                        break;
                    case 4:
                        animal.Demeanor = el.Value;
                        break;
                    case 5:
                        if (el.Value == "0")
                        {
                            animal.KidFriendly = false;
                        }
                        else
                        {
                            animal.KidFriendly = true;
                        }
                        break;
                    case 6:
                        if (el.Value == "0")
                        {
                            animal.PetFriendly = false;
                        }
                        else
                        {
                            animal.PetFriendly = true;
                        }
                        break;
                    case 7:
                        animal.Weight = int.Parse(el.Value);
                        break;
                    case 8:
                        animal.AnimalId = int.Parse(el.Value);
                        break;
                }
            }
            db.SubmitChanges();
        }

    internal static void RemoveAnimal(Animal animal)
        {
            var animalToRemove = db.Animals.FirstOrDefault(a => a.AnimalId == animal.AnimalId);
            db.Animals.DeleteOnSubmit(animalToRemove);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animals = db.Animals;
            foreach (KeyValuePair<int, string> el in updates)
            {
                switch (el.Key)
                {
                    case 1:
                        var categoryName = db.Categories.Where(c => c.Name == el.Value).FirstOrDefault();
                        var categoryInt = categoryName.CategoryId;
                        animals = animals.Where(a => a.CategoryId == categoryInt);
                        break;
                    case 2:
                        animals = animals.Where(a => a.Name == el.Value);
                        break;
                    case 3:
                        animals = animals.Where(a => a.Age == int.Parse(el.Value));
                        break;
                    case 4:
                        animals = animals.Where(a => a.Demeanor == el.Value);
                        break;
                    case 5:
                        if (el.Value == "0")
                        {
                            animals = animals.Where(a => a.KidFriendly == false);
                        }
                        else
                        {
                            animals = animals.Where(a => a.KidFriendly == true);
                        }
                        break;
                    case 6:
                        if (el.Value == "0")
                        {
                            animals = animals.Where(a => a.PetFriendly == false);
                        }
                        else
                        {
                            animals = animals.Where(a => a.PetFriendly == true);
                        }
                        break;
                    case 7:
                        animals = animals.Where(a => a.Weight == int.Parse(el.Value));
                        break;
                    case 8:
                        animals = animals.Where(a => a.AnimalId == int.Parse(el.Value));
                        break;
                }
            }
            return animals;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var catId = db.Categories.Where(c => c.Name.Equals(categoryName)).FirstOrDefault();
            return catId.CategoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            var room = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietId = db.DietPlans.Where(a => a.Name.Equals(dietPlanName)).FirstOrDefault();
            return dietId.DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            var adoptionClient = db.Clients.FirstOrDefault(a => a.ClientId == client.ClientId);
            var adoptionAnimal = db.Animals.FirstOrDefault(a => a.AnimalId == animal.AnimalId);
            adoption.ClientId = adoptionClient.ClientId;
            adoption.AnimalId = adoptionAnimal.AnimalId;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = null;
            adoption.PaymentCollected = false;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            var pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var adoptToRemove = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoptToRemove);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var shots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return shots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            db.Shots.Where(a => a.Name == shotName);
            db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            db.SubmitChanges();
        }
    }
}