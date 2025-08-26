using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
