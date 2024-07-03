using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Login
{
    internal class Program
    {
        public class UserInfo
        {
            public string name { get; set; }
            public string password { get; set; }

        }

        /** 
         * Hashes the raw text of the password entered into a hashed copy
         * 
         */
        static string HashPassword(string password)
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

        //static bool VerifyPassword(string hashedPassword)
        //{
        //    //STEP 5 Verify the user - entered password against a stored password
        //    /* Fetch the stored value */
        //    string savedPasswordHash = DBContext.GetUser(u => u.UserName == user).Password;
        //    /* Extract the bytes */
        //    byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
        //    /* Get the salt */
        //    byte[] salt = new byte[16];
        //    Array.Copy(hashBytes, 0, salt, 0, 16);
        //    /* Compute the hash on the password the user entered */
        //    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
        //    byte[] hash = pbkdf2.GetBytes(20);
        //    /* Compare the results */
        //    for (int i = 0; i < 20; i++)
        //        if (hashBytes[i + 16] != hash[i])
        //            throw new UnauthorizedAccessException();

        //}
        /**
        * Asks the user for info which can be used
        * for authentication later during user login
        * 
        * Return: boolean value to indicate if a fault has occured
        */
        static bool AskUserInfo(List<UserInfo> infoStore)
        {
            string userName = Console.ReadLine();
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

            var newUser = new UserInfo { name = userName, password = hashedPassword };
            // Memorize this
            infoStore.Add(newUser);
            return true;

        }
        static void Main(string[] args)
        {
            string welcomeMessage = " Hello user please enter your user name and password:";
            Console.WriteLine(welcomeMessage);

            List<UserInfo> infoStore = new List<UserInfo>();

            AskUserInfo(infoStore);


            // Welcome the user which has been just logged into the system
            Console.WriteLine($"Hi, {infoStore.LastOrDefault().name}");
            Console.WriteLine("the program has finished, press q on your keyboard to exit");

            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    return;
                }
            }
        }
    }
}
