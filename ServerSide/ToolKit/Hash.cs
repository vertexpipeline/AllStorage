﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ToolKit
{
    [Newtonsoft.Json.JsonConverter(typeof(ToolKit.HashJsonConverter))]
    public class Hash
    {
        private byte[] _hash;
        static SHA256 sha = SHA256.Create();

        public Hash(string base64)
        {
            _hash = Convert.FromBase64String(base64);
        }
        private Hash()
        {

        }

        public static Hash FromString(string data) => new Hash() { _hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data)) };
        public static Hash FromBase64(string base64) => new Hash(base64);
        public static Task<Hash> FromStreamAsync(Stream stream)
        {
            return Task.Run(()=> {
                return new Hash() { _hash = sha.ComputeHash(stream) };
            });
        }

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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var second = obj as Hash;
            if (second == null && this == null)
                return true;
            if (second == null || this == null)
                return false;
            var isEqual = true;

            for (int i = 0; i < this._hash.Length; i++) {
                if (this._hash[i] != second._hash[i]) {
                    isEqual = false;
                    break;
                }
            }

            return isEqual;
        }

        public override string ToString()
        {
            return Convert.ToBase64String(_hash);
        }
    }
}
