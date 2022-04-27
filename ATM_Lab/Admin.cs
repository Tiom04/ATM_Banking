using System;
using System.Collections.Generic;
using System.IO;

namespace ATM_Lab
{
    internal class Admin : Account
    {
        int _pinCode;
        public int PinCode { set { _pinCode = value; } }

        public Admin()
        {
            accountType = "Admin";
        }
        public Admin(string log) : base(log)
        {
            accountType = "Admin";
        }
        public override bool Login()
        {
            using (FileStream file = new FileStream(Program.PATH_TO_ADMIN_LIST, FileMode.OpenOrCreate))
            {
                StreamReader reader = new StreamReader(file);
                List<Admin> readAdminList = new List<Admin>();
                while (!(reader.EndOfStream))
                {
                    reader.ReadLine();
                    Admin tmpAdmAdd = new Admin(reader.ReadLine());
                    tmpAdmAdd._pinCode = Convert.ToInt32(reader.ReadLine());
                    readAdminList.Add(tmpAdmAdd);
                }
                foreach (var val in readAdminList)
                {
                    if (val.GetLogin() == AccLogin)//
                    {
                        Console.Write("Enter your Pin code: ");
                        bool passStatus = false;
                        for (int i = 3; i > 0; i--)
                        {
                            Console.WriteLine($"Enter your pin... (you have {i} attemp(s) left)");
                            int tmpCode = Convert.ToInt32(Console.ReadLine());
                            if (tmpCode == val._pinCode)
                            {
                                Console.WriteLine("Logged in");
                                passStatus = true;
                                return true;
                            }
                        }
                        if (!passStatus)
                        {
                            Console.WriteLine("Wrong pass, try later");
                            return false;
                        }
                        break;
                    }
                }
            }
            return false;
        }
        public override void Register(string login)
        {
            Console.WriteLine("\t_----Register new admin account----_");
            AccLogin = login;
            Console.WriteLine("Create 5-digit Pin code");
            int pinCode;
            do { pinCode = Convert.ToInt32(Console.ReadLine()); }
            while (pinCode < 10000 || pinCode > 99999);
            _pinCode = pinCode;
            Console.WriteLine("Registered successfully");
        }

        public override string ToString()
        {
            return $"{accountType}\n{AccLogin}\n{_pinCode}";
        }
        internal void CreateNewAccount()
        {
            Console.WriteLine("Creating new account");
            Customer customerAccount = new Customer();
            Random random = new Random();
            Console.Write("Login: ");
            customerAccount.AccLogin = Console.ReadLine();
            Console.Write("Starting amount: ");
            customerAccount.Amount = Convert.ToDouble(Console.ReadLine());
            customerAccount.cardNumPass.Item1 = Convert.ToString(random.Next(10000000, 99999999));
            Console.WriteLine("Customer card number is {0}", customerAccount.cardNumPass.Item1);
            Console.Write("Create 5-digit Pin code :");
            int pinCode;
            do { pinCode = Convert.ToInt32(Console.ReadLine()); }
            while (pinCode < 10000 || pinCode > 99999);
            customerAccount.cardNumPass.Item2 = pinCode;
            Program.customerList.Add(customerAccount);
            Console.WriteLine("Created successfully");
        }

        internal void DeleteExistingAccount(string customerLogin)
        {
            bool contains = false;
            foreach (var val in Program.customerList)
            {
                if (val.GetLogin() == customerLogin)
                {
                    contains = true;
                    Program.customerList.Remove(val);
                }
            }
            if (contains == false)
            {
                Console.WriteLine("No account with such login exists");
            }
        }
        internal void UpdateAccountInformation(string customerLogin)
        {
            bool contains = false;
            foreach (var val in Program.customerList)
            {
                if (val.GetLogin() == customerLogin)
                {
                    contains = true;

                    Console.WriteLine($"{val.accountType}\n{val.AccLogin}\n{val.cardNumPass.Item1}\n{val.Amount}" +
                    $"\n{val.Status}n");
                    Console.WriteLine("Please enter in the fields you wish to update (leave blank otherwise):");
                    Customer tmpCust = new Customer();
                    Console.Write("Login: ");
                    tmpCust.AccLogin = Console.ReadLine();
                    if (!(string.IsNullOrWhiteSpace(tmpCust.AccLogin)))
                    {
                        val.AccLogin = tmpCust.AccLogin;
                    }
                    Console.Write("Pin code: ");
                    tmpCust.cardNumPass.Item2 = Convert.ToInt32(Console.ReadLine());
                    if (tmpCust.cardNumPass.Item2 >= 10000 && tmpCust.cardNumPass.Item2 <= 99999)
                    {
                        val.cardNumPass.Item2 = tmpCust.cardNumPass.Item2;
                    }
                    Console.Write("Status: ");
                    tmpCust.Status = Console.ReadLine();
                    if (tmpCust.Status == "Admin" || tmpCust.Status == "admin")
                    {
                        val.Status = "Admin";
                    }
                    if (tmpCust.Status == "Customer" || tmpCust.Status == "customer")
                    {
                        val.Status = "Customer";
                    }
                }
            }
            Console.WriteLine("Your account has been successfully updated");
            if (contains == false)
            {
                Console.WriteLine("No account with such login exists");
            }
        }
        internal void SearchForAccount()
        {
            Customer searchCust = new Customer();
            Console.WriteLine("SEARCH MENU:\n");
            bool log = false, bal = false, stat = false;

            Console.Write("Account login:");
            searchCust.AccLogin = Console.ReadLine();
            if (!(string.IsNullOrWhiteSpace(searchCust.AccLogin)))
            {
                log = true;
            }
            Console.Write("Balance:");
            searchCust.Amount = Convert.ToDouble(Console.ReadLine());
            if (double.IsPositiveInfinity(searchCust.Amount))
            {
                bal = true;
            }
            Console.Write("Statis (Active or Blocked)");
            searchCust.Status = Console.ReadLine();
            if (searchCust.Status == "Active" || searchCust.Status == "Blocked")
            {
                stat = true;
            }

            if (log == true)
            {
                foreach (var val in Program.customerList)
                {
                    if (val.GetLogin() == searchCust.GetLogin())
                    {
                        Console.WriteLine($"Account Login: {val.GetLogin()}" +
                            $"\nBalance: {val.Amount}\nStatus: {val.Status}\n");
                    }
                }
            }
            else
            {
                if (bal == true)
                {
                    if (stat == true)
                    {
                        foreach (var val in Program.customerList)
                        {
                            if (val.Amount == searchCust.Amount && val.Status == searchCust.Status)
                            {
                                Console.WriteLine($"Account Login: {val.GetLogin()}" +
                            $"\nBalance: {val.Amount}\nStatus: {val.Status}\n");
                            }
                        }
                    }
                    else
                    {
                        foreach (var val in Program.customerList)
                        {
                            if (val.Amount == searchCust.Amount)
                            {
                                Console.WriteLine($"Account Login: {val.GetLogin()}" +
                           $"\nBalance: {val.Amount}\nStatus: {val.Status}\n");
                            }
                        }
                    }
                }
                else
                {
                    if (stat == true)
                    {
                        foreach (var val in Program.customerList)
                        {
                            if (val.Status == searchCust.Status)
                            {
                                Console.WriteLine($"Account Login: {val.GetLogin()}" +
                           $"\nBalance: {val.Amount}\nStatus: {val.Status}\n");
                            }
                        }
                    }
                }
            }
        }
        internal void UnblockUsers()
        {
            foreach (var val in Program.unblockRequestList)
            {
                Console.WriteLine(val.ToString());
            }
            if (Program.unblockRequestList.Count == 0)
            {
                Console.WriteLine("No requests");
            }
            else
            {
            UnblockOrExit:
                Console.WriteLine("Do you want to unblock someone's acc? (Y/N)");
                var option = Console.ReadLine();
                if (option == "Y" || option == "y")
                {
                    bool unblocked = false;
                    Console.Clear();
                    Console.Write("Enter account login you want to unblock: ");
                    string unblockLogin = Console.ReadLine();
                    foreach (var val in Program.unblockRequestList)
                    {
                        if (val.CustomerAccount == unblockLogin)
                        {
                            foreach (var cust in Program.customerList)
                            {
                                if (cust.GetLogin() == unblockLogin)
                                {
                                    cust.Status = "Active";
                                }
                            }
                            unblocked = true;
                        }
                    }
                    if (unblocked == false)
                    {
                    NotFoundLogin:
                        Console.Write("No such login found, do you want to try one more time? (Y/N): ");
                        var choiceLogFail = Console.ReadLine();
                        if (choiceLogFail == "Y" || choiceLogFail == "y")
                        {
                            goto UnblockOrExit;
                        }
                        else if (choiceLogFail == "N" || choiceLogFail == "n")
                        {
                            Console.WriteLine("Ok, bye");
                        }
                        else
                        {
                            Console.WriteLine("Wrong input, please re-enter your choice");
                            goto NotFoundLogin;
                        }
                    }
                    else//if unblocked
                    {
                        using (FileStream file = new FileStream(Program.PATH_TO_UNBLOCK_REQUEST_LIST, FileMode.OpenOrCreate, FileAccess.Read))
                        {
                            StreamReader reader = new StreamReader(file);
                            while (!(reader.EndOfStream))
                            {
                                string log, mes;
                                log = reader.ReadLine();
                                mes = reader.ReadLine();
                                reader.ReadLine();
                                UnblockRequest unblockRequest = new UnblockRequest(mes, log);
                                if (unblockRequest.CustomerAccount == unblockLogin)
                                {
                                    Program.unblockRequestList.Remove(unblockRequest);
                                }
                            }
                        }
                    }
                }
                else if (option == "N" || option == "n")
                {
                    Console.WriteLine("Not going to unblock any acc..");
                }
                else
                {
                    Console.WriteLine("Wrong input, please re-enter your oprion");
                    goto UnblockOrExit;
                }
            }
        }
        internal void ViewReports()
        {
            List<Transaction> transactions = new List<Transaction>();
            Transaction readTran;

            using (FileStream file = new FileStream(Program.PATH_TO_TRANSACTION_LIST, FileMode.OpenOrCreate))
            {
                StreamReader reader = new StreamReader(file);

                while (!(reader.EndOfStream))
                {
                    readTran = new Transaction(Convert.ToDateTime(reader.ReadLine()),
                    Convert.ToDouble(reader.ReadLine()), reader.ReadLine(), reader.ReadLine());
                    reader.ReadLine();
                    transactions.Add(readTran);
                }
                file.Close();
            }
            foreach (var val in transactions)
            {
                Console.WriteLine(val.ToString());
            }
        }
    }
}