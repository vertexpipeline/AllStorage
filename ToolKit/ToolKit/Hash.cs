using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ToolKit
{
    class Hash
    {
        private byte[] _hash;
        static SHA256 sha = SHA256.Create();

        public Hash(string base64)
        {
            _hash = sha.ComputeHash(Convert.FromBase64String(base64));
        }
        private Hash()
        {

        }

        public static Hash FromString(string data) => new Hash() { _hash = sha.ComputeHash(Encoding.Unicode.GetBytes(data)) };
        public static Hash FromBase64(string base64) => new Hash(base64);
        public static Hash FromStream(Stream stream) => new Hash() { _hash = sha.ComputeHash(stream) };

        public static Hash operator ^(Hash first, Hash second)
        {
            var hash = new byte[first._hash.Length];

            for (int i = 0; i < hash.Length; i++) {
                hash[i] = (byte)(first._hash[i] ^ second._hash[i]);
            }

            return new Hash() { _hash = hash };
        }
        public static Hash operator &(Hash first, Hash second)
        {
            var hash = new byte[first._hash.Length];

            for (int i = 0; i < hash.Length; i++) {
                hash[i] = (byte)(first._hash[i] & second._hash[i]);
            }

            return new Hash() { _hash = hash };
        }
        public static Hash operator |(Hash first, Hash second)
        {
            var hash = new byte[first._hash.Length];

            for (int i = 0; i < hash.Length; i++) {
                hash[i] = (byte)(first._hash[i] | second._hash[i]);
            }

            return new Hash() { _hash = hash };
        }

        public static bool operator ==(Hash first, Hash second)
        {
            var isEqual = true;

            for (int i = 0; i < first._hash.Length; i++) {
                if (first._hash[i] != second._hash[i]) {
                    isEqual = false;
                    break;
                }
            }

            return isEqual;
        }
        public static bool operator !=(Hash first, Hash second) => !(first == second);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if(obj is Hash) {
                return (Hash)obj == this;
            }

            return base.Equals(obj);
        }
    }
}
