using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ATM_Lab
{
    public abstract class Account
    {
        public string AccLogin { get; set; }
        public string accountType;
        public string GetLogin() { return AccLogin; }
        public string GetType() { return accountType; }
        public Account()
        {
        }
        public Account(string log)
        {
            AccLogin = log;
        }
        public abstract bool Login();
        public abstract void Register(string login);
        public override abstract string ToString();
    }
}
