using System;
using System.Collections.Generic;
using Budget.EventSourcing.Events;

using MongoDB.Bson.Serialization.Attributes;

namespace Budget.Users.MongoDbAdapters.Entities
{
    public class User
    {
        public User()
        {
        }

        public User(Budget.Users.Domain.Model.WriteModel.User user)
        {
            Id = user.Id;
            Changes = user.Changes;   
        }

        [BsonId]
        public Guid Id { get; set; } 

        public IEnumerable<Event> Changes { get; set; } 
    }
}