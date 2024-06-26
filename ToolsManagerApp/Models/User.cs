﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ToolsManagerApp.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public RoleEnum Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> AssignedToolIds { get; set; } = new List<string>();

        public void Authenticate() { /* Implementation */ }
        public void ChangePassword(string newPassword) { Password = newPassword; }
        public void UpdateInfos(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
    // admin extends user + role admin + methodes de l'admin
    // user extends user + role user + methodes de l'user
}
