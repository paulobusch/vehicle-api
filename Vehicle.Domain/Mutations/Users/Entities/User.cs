﻿using Questor.Vehicle.Domain.Utils.Random;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;
using Questor.Vehicle.Domain.Utils.Hash;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Questor.Vehicle.Domain.Mutations.Users.Entities
{
    [Table("users")]
    public class User
    {
        [Required] [Key]
        public string Id { get; private set; }

        [Required] [MaxLength(150)]
        public string Name { get; private set; }

        [Required] [MaxLength(50)] [Index("UQ_users_login", IsUnique = true)]
        public string Login { get; private set; }

        [Required] [MaxLength(80)]
        public string Password { get; private set; }

        public User() { }

        public User(
            string id,
            string name,
            string login,
            string password
        ) : this() {
            this.Id = string.IsNullOrWhiteSpace(id) ? RandomId.NewId() : id;
            this.Password = MD5Crypto.Encode(VehicleStartup.Secret + password);
            this.SetData(
                name: name,
                login: login
            );
        }

        public void SetData(
            string name,
            string login
        ) {
            this.Name = name;
            this.Login = login;
        }

        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            var hash = MD5Crypto.Encode(VehicleStartup.Secret + password);
            return hash.Equals(Password);
        }
    }
}
