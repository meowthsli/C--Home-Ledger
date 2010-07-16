﻿
using System;

namespace Meowth.OperationMachine.Entities.Accounts
{
    /// <summary>
    /// Path to account
    /// </summary>
    public class AccountPathName
    {
        public static readonly string SEPARATOR = ".";

        public virtual string Path
        {
            get { return _asString; }
            protected set { _asString = value; }
        }

        /// <summary>
        /// Creates account path from string
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static AccountPathName FromString(string pathName)
        {
            var components = pathName
                .Trim()
                .ToLowerInvariant()
                .Split(new[]{SEPARATOR}, StringSplitOptions.RemoveEmptyEntries);
            if(components.Length == 0)
                throw new ArgumentException("pathName");

            return new AccountPathName(components);
        }


        public static AccountPathName FromParentNameAndString(
            AccountPathName parentPathName, string accountName)
        {
            return FromString(parentPathName + SEPARATOR + accountName);
        }

        /// <summary> Returns array of name components </summary>
        public virtual string[] GetNameComponents()
        {
            return _asString.Split(SEPARATOR[0]);
        }

        /// <summary> Creates from components </summary>
        private AccountPathName(string[] components)
        {
            _asString = string.Join(SEPARATOR, components);
        }

        public override int GetHashCode()
        {
            return _asString.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
                return false;

            if (obj.GetType() != typeof(AccountPathName))
                return false;

            return Path == ((AccountPathName)obj).Path;
        }

        public override string ToString()
        {
            return _asString;
        }
        
        private string _asString;
    }
}
