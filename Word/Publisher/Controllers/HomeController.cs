using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Publisher.Models;
using Publisher.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher.Controllers
{
    public class HomeController : Controller
    {
        private RabbitMQPublisher _rabbitMQPublisher;
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, RabbitMQPublisher rabbitMQPublisher)
        {
            _rabbitMQPublisher = rabbitMQPublisher;
               _logger = logger;
           
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WordToPdfPage()
        {

            return View();
        }
        [HttpPost]
        public IActionResult WordToPdfPage(Word word, Message message)
        {
            
           
            _rabbitMQPublisher.Publish(message, word);

            

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
