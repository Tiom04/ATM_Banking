using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM_Lab
{
    public class Customer : Account
    {
        public (string, int) cardNumPass;
        public double Amount { get; set; }
        public string Status { get; set; }
        public Customer()
        {
            accountType = "Customer";
            Status = "Active";
        }
        public Customer(string log) : base(log)
        {
            accountType = "Customer";
            Status = "Active";
        }
        public Customer(string login, string cardNum, double amount,
            string stat, int pass) : base(login)
        {
            accountType = "Customer";
            cardNumPass.Item1 = cardNum;
            Amount = amount;
            Status = stat;
            cardNumPass.Item2 = pass;
        }

        public override bool Login()
        {
            using (FileStream file = new FileStream(Program.PATH_TO_CUSTOMER_LIST, FileMode.OpenOrCreate))
            {
                StreamReader reader = new StreamReader(file);
                List<Customer> readAccountList = new List<Customer>();
                while (!(reader.EndOfStream))
                {
                    reader.ReadLine();
                    Customer tmpCustAdd = new Customer(reader.ReadLine(), reader.ReadLine(),
                        Convert.ToDouble(reader.ReadLine()), reader.ReadLine(),
                        Convert.ToInt32(reader.ReadLine()));
                    readAccountList.Add(tmpCustAdd);
                    reader.ReadLine();
                }
                foreach (var val in readAccountList)
                {
                    if (val.GetLogin() == AccLogin)//
                    {
                        Console.Write("Enter your Pin code: ");
                        bool passStatus = false;
                        for (int i = 3; i > 0; i--)
                        {
                            Console.WriteLine($"Enter your pin... (you have {i} attemp(s) left)");
                            int tmpCode = Convert.ToInt32(Console.ReadLine());
                            if (tmpCode == val.cardNumPass.Item2)
                            {
                                Console.WriteLine("Logged in");
                                val.Status = "Active";
                                passStatus = true;
                                return true;
                                break;
                            }
                        }
                        if (!passStatus)
                        {
                            Status = "Blocked";
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
            Random random = new Random();
            Console.WriteLine("\t_----Register new customer account----_");
            AccLogin = login;
            accountType = "Customer";
            cardNumPass.Item1 = Convert.ToString(random.Next(10000000, 99999999));
            //may occur error if number already exists. Could add check in release version if needed;
            Console.WriteLine("Your card number is {0}", cardNumPass.Item1);
            Console.WriteLine("Create 5-digit Pin code");
            int pinCode;
            do { pinCode = Convert.ToInt32(Console.ReadLine()); }
            while (pinCode < 10000 || pinCode > 99999);
            cardNumPass.Item2 = pinCode;
            Status = "Active";
            Console.WriteLine("Registered successfully");
        }
        public override string ToString()
        {
            return $"{accountType}\n{AccLogin}\n{cardNumPass.Item1}\n{Amount}" +
                $"\n{Status}\n{cardNumPass.Item2}\n";
        }

        public void WithdrawFastCash()
        {
            Console.WriteLine("Fast cash options:\n1. 500\t\t2. 1000\n3. 2000\t\t4. 5000\n5. 10000\t6. 15000\n7. 20000");
            Console.Write("Enter wanted number: ");
            int chosenVar;
            do { chosenVar = Convert.ToInt32(Console.ReadLine()); }
            while (chosenVar < 1 || chosenVar > 7);

            switch (chosenVar)
            {
                case 1:
                    {
                        WithdrawCash(500);
                    }
                    break;
                case 2:
                    {
                        WithdrawCash(1000);
                    }
                    break;
                case 3:
                    {
                        WithdrawCash(2000);
                    }
                    break;
                case 4:
                    {
                        WithdrawCash(5000);
                    }
                    break;
                case 5:
                    {
                        WithdrawCash(10000);
                    }
                    break;
                case 6:
                    {
                        WithdrawCash(15000);
                    }
                    break;
                case 7:
                    {
                        WithdrawCash(20000);
                    }
                    break;
            }
        }
        public void WithdrawCash(double sum)
        {
            Transaction transaction = new Transaction();
            List<Transaction> transactionList = new List<Transaction>();

            using (FileStream file = new FileStream(Program.PATH_TO_TRANSACTION_LIST, FileMode.OpenOrCreate, FileAccess.Read))
            {
                StreamReader reader = new StreamReader(file);
                while (!(reader.EndOfStream))
                {
                    Transaction tempTran = new Transaction();
                    tempTran.Date = Convert.ToDateTime(reader.ReadLine());
                    tempTran.Amount = Convert.ToDouble(reader.ReadLine());
                    tempTran.Type = reader.ReadLine();
                    tempTran.AccLogin = reader.ReadLine();
                    reader.ReadLine();
                    transactionList.Add(tempTran);
                }
                reader.Close();

                var query = (from tran in transactionList
                             where (tran.Date.Day == DateTime.Now.Day)
                             && (tran.Type == "withdraw")
                             select tran).ToList();

                double totalWithdrawnToday = 0;
                foreach (var val in query)
                {
                    totalWithdrawnToday += val.Amount;
                }
                if (totalWithdrawnToday + sum > 20000)
                {
                    Console.WriteLine("Limit reached, max 20000 per day");

                    if (totalWithdrawnToday != 20000)
                    {
                        Console.WriteLine($"You can withdraw {20000 - totalWithdrawnToday} today");
                        Console.WriteLine($"Do you want to withdraw {20000 - totalWithdrawnToday}? (Y/N)");
                        var option = Console.ReadLine();
                        if (option == "Y" || option == "y")
                        {
                            WithdrawCash(20000 - totalWithdrawnToday);
                        }
                        else if (option == "N" || option == "n")
                        {
                            goto End;
                        }
                        else
                        {
                            Console.WriteLine("Wrong input");
                        }
                    }
                    goto End;
                }
            }
            if (sum > 0)
            {
                if (sum <= Amount)
                {
                    Amount -= sum;
                    transaction.Date = DateTime.Now;
                    transaction.AccLogin = AccLogin;
                    transaction.Amount = sum;
                    transaction.Type = "withdraw";
                    PrintReceipt(transaction);
                }
                else Console.WriteLine("Not enough money");
            }
            else Console.WriteLine("Invalid amount");
            using (FileStream file = new FileStream(Program.PATH_TO_TRANSACTION_LIST, FileMode.Append))
            {
                StreamWriter writer = new StreamWriter(file);
                writer.WriteLine(transaction.ToString());
                writer.Close();
            }
        End:
            Console.WriteLine();
        }
        public void DepositCash(int sum)
        {
            Transaction transaction = new Transaction();
            if (sum > 0)
            {
                Amount += sum;
                transaction.Date = DateTime.Now;
                transaction.AccLogin = AccLogin;
                transaction.Amount = sum;
                transaction.Type = "deposit";
                PrintReceipt(transaction);
            }
            else Console.WriteLine("Invalid amount");

            using (FileStream file = new FileStream(Program.PATH_TO_TRANSACTION_LIST, FileMode.Append))
            {
                StreamWriter writer = new StreamWriter(file);
                writer.WriteLine(transaction.ToString());

                writer.Close();
            }
        }
        public void Balance()
        {
            Console.WriteLine($"Balance by {DateTime.Now.ToString("f")} on" +
                $"card #{cardNumPass.Item1} held by {AccLogin} is {Amount}");
        }
        public void CashTransfer()
        {
            Console.Write("Specify the amount to transfer in multiples of 500: ");
            int transferSum;
            do { transferSum = Convert.ToInt32(Console.ReadLine()); }
            while (transferSum % 500 != 0);
            Console.Write("Enter card number you want to make transfer to: ");
            string cardNumber = Console.ReadLine();
            List<Customer> customerLst = new List<Customer>();
            bool existFlag = false;
            using (FileStream file = new FileStream(Program.PATH_TO_CUSTOMER_LIST, FileMode.OpenOrCreate, FileAccess.Read))
            {
                StreamReader reader = new StreamReader(file);
                while (!(reader.EndOfStream))
                {
                    reader.ReadLine();
                    Customer tmpCustAdd = new Customer(reader.ReadLine(), reader.ReadLine(),
                        Convert.ToDouble(reader.ReadLine()), reader.ReadLine(),
                        Convert.ToInt32(reader.ReadLine()));
                    customerLst.Add(tmpCustAdd);
                    reader.ReadLine();
                }
                foreach (var val in customerLst)
                {
                    if (val.cardNumPass.Item1 == cardNumber)
                    {
                        Console.WriteLine($"Card holder nickname: {val.GetLogin()}");
                        existFlag = true;
                        Console.Write("Enter card number one more time to approve: ");
                        string tmpSecurityCN = Console.ReadLine();
                        if (tmpSecurityCN == cardNumber)
                        {
                            if (transferSum > 0)
                            {
                                if (transferSum <= Amount)
                                {
                                    Amount -= transferSum;
                                    val.Amount += transferSum;
                                }
                                else Console.WriteLine("Not enough money");
                            }
                            else Console.WriteLine("Invalid amount");
                        }
                        else
                        {
                            Console.WriteLine("Numbers don't corespond, try one more time");
                            CashTransfer();
                        }
                    }
                }
                file.Close();
            }
            if (!existFlag)
            {
                Console.WriteLine("Card number is wrong, such account doest't exist! Transaction denied");
            }
            else
            {
                Transaction transaction = new Transaction(DateTime.Now, Convert.ToDouble(transferSum),
                    "transfer to another account", AccLogin);
                Console.WriteLine("Transaction confirmed.");
                PrintReceipt(transaction);
                using (FileStream file = new FileStream(Program.PATH_TO_TRANSACTION_LIST, FileMode.Append))
                {
                    StreamWriter writer = new StreamWriter(file);
                    writer.WriteLine(transaction.ToString());
                    writer.Close();
                }
                using (FileStream file = new FileStream(Program.PATH_TO_CUSTOMER_LIST, FileMode.Truncate))
                {
                    StreamWriter writer = new StreamWriter(file);
                    foreach (var item in customerLst)
                    {
                        writer.WriteLine(item.ToString());
                    }
                    writer.Close();
                }
            }
        }
        public void PrintReceipt(Transaction transaction)
        {
            Console.WriteLine("Do you want to print the receipt? (Y/N)");
            var option = Console.ReadLine();
            if (option == "Y" || option == "y")
            {
                Console.WriteLine($"Login: {transaction.AccLogin}\nDate: {transaction.Date}" +
                    $"\nOperation sum: {transaction.Amount}\nTransaction type: {transaction.Type}" +
                    $"\nCurrent balance: {Amount}");
                Console.WriteLine("Thanks for using our ATM");
            }
            else if (option == "N" || option == "n")
            {
                Console.WriteLine("Thanks for using our ATM");
            }
            else
            {
                Console.WriteLine("Wrong input");
                PrintReceipt(transaction);
            }
        }

    }
}
