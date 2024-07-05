using System;
using System.Collections.Generic;
using System.Threading;

namespace Login
{
    internal class Program
    {
        public enum VerifyStatus
        {
            Success,
            UserNotFound,
            WrongPassword,
        }

        /**
         * Class for storing user data like user name and password, 
         * these can be then used for login authentication
         */
        public class UserInfo
        {
            public string Name { get; set; }
            public string Password { get; set; }
        }

        /**
        * Asks the user for info which can be used
        * for authentication later during user login
        * 
        * @return: boolean value to indicate if a fault has occured
        */
        static bool LogUserInfo(List<UserInfo> infoStore)
        {
            Console.WriteLine("Enter name:");
            string userName = Console.ReadLine();
            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();

            if (userName == null)
            {
                Console.WriteLine("your user name can not be empty");
                return false;
            }
            if (password == null)
            {
                Console.WriteLine("your password can not be empty");
                return false;
            }

            var newUser = new UserInfo { Name = userName, Password = password };
            // Memorize this
            infoStore.Add(newUser);
            return true;
        }

        /**
        * VerifyPassword verifys the user
        * by checking the password against the one stored inside infoStore 
        * matching the user name entered. 
        * 
        * @return: an Enum type which indicates the verification status 
        * 
        */
        static VerifyStatus VerifyPassword(List<UserInfo> infoStore, string nameEntered, string passwordEntered)
        {

            UserInfo searched = infoStore.Find(info => info.Name == nameEntered);

            if (searched == null)
            {
                return VerifyStatus.UserNotFound;
            }
            if (searched.Password != passwordEntered)
            {
                return VerifyStatus.WrongPassword;
            }

            return VerifyStatus.Success;
        }

        private static void EnterLoginProcedure()
        {
            Console.Clear();
            string welcomeMessage = " Hello registered user! please sign in with your user name and password:";
            Console.WriteLine(welcomeMessage);
            Console.WriteLine("Enter name:");
            string nameEntered = Console.ReadLine();
            Console.WriteLine("Enter password:");
            string passwordEntered = Console.ReadLine();

            VerifyStatus vs = VerifyPassword(infoStore, nameEntered, passwordEntered);

            if (vs == VerifyStatus.UserNotFound)
            {
                Console.WriteLine("User name not found, maybe try register first?");
            }
            else if (vs == VerifyStatus.WrongPassword)
            {
                Console.WriteLine("Password entered does not match user name");
            }
            else
            {
                // Welcome the user which has been just logged into the system
                Console.WriteLine($"Hi, {nameEntered}");
            }

            // Wait a little bit for the user to see the message before going back
            Thread.Sleep(3000);
        }

        private static void EnterSignupProcedure()
        {
            Console.Clear();
            string welcomeMessage = " Hello new user! please create your user name and password:";
            Console.WriteLine(welcomeMessage);
            LogUserInfo(infoStore);
        }

        private static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Hi user! Welcome come to my simple login system");
            Console.WriteLine("Please proceed by one of the following numbers:");

            Console.WriteLine("");
            for (int i = 0; i < menuItems.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {menuItems[i]}");
            }
            Console.WriteLine("");

            Console.Write("The number you choose is:");
        }

        public static List<UserInfo> infoStore = new List<UserInfo>();
        public static List<string> menuItems = new List<string> { "Sign up", "Log in", "exit" };
        static void Main(string[] args)
        {

            bool running = true;
            while (running)
            {

                DisplayMainMenu();

                string optionSelected = Console.ReadLine();

                switch (optionSelected)
                {
                    case "1":
                        EnterSignupProcedure();
                        break;
                    case "2":
                        EnterLoginProcedure();
                        break;
                    case "3":
                        running = false;
                        break;
                }

            }

        }
    }
}

