using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Models
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public string FromName { get; set; } = string.Empty;
        public bool UseDefaultCredentials { get; set; } = false;
    }
}
