﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Model
{
    public class RequestGreetingModel
    {
        public string Email { get; set; }    
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 
    }
}
