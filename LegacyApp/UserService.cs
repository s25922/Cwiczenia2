using System;

namespace LegacyApp
{
    public class UserService
    {
        private ClientRepository clientRepository;
        private UserCreditService userCreditService;

        public UserService()
        {
            clientRepository = new ClientRepository();
            userCreditService = new UserCreditService();
        }

        public UserService(ClientRepository clientRepository, UserCreditService userCreditService)
        {
            this.clientRepository = clientRepository;
            this.userCreditService = userCreditService;
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!IsValidUserInput(firstName, lastName, email, dateOfBirth))
            {
                return false;
            }

            var client = clientRepository.GetById(clientId);
            var user = CreateUser(firstName, lastName, email, dateOfBirth, client);

            if (!IsValidUserBasedOnClientType(user))
            {
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }

        private bool IsValidUserInput(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                return false;
            }

            if (CalculateAge(dateOfBirth) < 21)
            {
                return false;
            }

            return true;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
            {
                age--;
            }

            return age;
        }

        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client)
        {
            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email,
                DateOfBirth = dateOfBirth,
                Client = client,
                HasCreditLimit = true,
                CreditLimit = 0
            };
        }

        private bool IsValidUserBasedOnClientType(User user)
        {
            switch (user.Client)
            {
                case "VeryImportantClient":
                    user.HasCreditLimit = false;
                    break;
                case "ImportantClient":
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    user.CreditLimit = creditLimit * 2;
                    break;
                default:
                    user.CreditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    break;
            }

            return !user.HasCreditLimit || user.CreditLimit >= 500;
        }
    }
}
