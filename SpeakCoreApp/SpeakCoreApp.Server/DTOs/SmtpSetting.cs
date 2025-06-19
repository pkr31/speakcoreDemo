﻿using System.ComponentModel.DataAnnotations;

namespace SpeakCoreApp.Server.DTOs
{
    public class SmtpSetting
    {

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
    }
}
