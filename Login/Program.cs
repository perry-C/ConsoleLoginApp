using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace Login
{
    internal class Program
    {
        /**
         * Class for storing the information needed to create a menu option
         */
        public class Option
        {
            public string Name { get; }
            public Action Action { get; }
            public Option(string name, Action action)
            {
                Name = name;
                Action = action;
            }
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
         * Hashes the raw text of the password entered into a hashed copy
         */
        private static string HashPassword(string password)
        {
            //STEP 1 Create the salt value with a cryptographic PRNG:
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            //STEP 2 Create the Rfc2898DeriveBytes and get the hash value:
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            //STEP 3 Combine the salt and password bytes for later use:
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            //STEP 4 Turn the combined salt+hash into a string for storage
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }


        enum VerifyStatus
        {
            Success,
            UserNotFound,
            WrongPassword,
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
            //STEP 5 Verify the user - entered password against a stored password
            /* Fetch the stored value */
            UserInfo searched = infoStore.Find(info => info.Name == nameEntered);

            if (searched == null)
            {
                return VerifyStatus.UserNotFound;
            }

            string savedPasswordHash = searched.Password;

            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(passwordEntered, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return VerifyStatus.WrongPassword;
                }
            }
            return VerifyStatus.Success;
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

            var hashedPassword = HashPassword(password);

            var newUser = new UserInfo { Name = userName, Password = hashedPassword };
            // Memorize this
            infoStore.Add(newUser);
            return true;
        }

        public static List<Option> options = new List<Option>
            {
                new Option("Sign Up", () =>  EnterSignupProcedure()),
                new Option("Login", () => EnterLoginProcedure()),
                new Option("Exit", () => Environment.Exit(0)),
            };

        public static List<UserInfo> infoStore = new List<UserInfo>();

        static void Main(string[] args)
        {
            // Initialise variables for use
            int indexSelected = 0;
            bool running = true;
            ConsoleKeyInfo keyInfo;
            while (running)
            {
                // For each loop, clear the terminal output and draw a new version of the menu
                Console.Clear();
                DrawMenu(options, indexSelected);

                // Read keyboard info to determine action
                keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    // Update the menu option selected based on the index
                    case ConsoleKey.UpArrow:
                        if (indexSelected - 1 != -1)
                        {
                            indexSelected--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        // Only modify the index if it has not gone out of range
                        if (indexSelected + 1 != options.Count)
                        {
                            indexSelected++;
                        }
                        break;
                    // Quit using Q
                    case ConsoleKey.Q:
                        running = false;
                        break;
                    // Triggers a secondary procedure based on the option selected
                    case ConsoleKey.Enter:
                        options[indexSelected].Action.Invoke();
                        break;
                }
            }
        }
        /**
         * This function draws the menu on the terminal 
         * @params options: 
         *     Supplies the info needed to create each menu item
         
         * @params indexSelected:
         *     Adds a ">" indicator to option[indexSelected]
         */
        private static void DrawMenu(List<Option> options, int indexSelected)
        {
            for (int i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if (i == indexSelected)
                {
                    Console.WriteLine($"> {option.Name}");
                }
                else
                {
                    Console.WriteLine(option.Name);
                }
            }
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
    }
}
