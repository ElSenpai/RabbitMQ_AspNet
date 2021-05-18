using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher.Models
{
    public class Word
    {
        public string Email { get; set; }
        public IFormFile WordFile { get; set; }

       
    }
}
