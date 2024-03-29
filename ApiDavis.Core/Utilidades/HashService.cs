﻿using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RazorEngine;
using RazorEngine.Templating;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Utilidades
{
    public class HashService
    {
        private const string PasswordHash = "pass75dc@avz10";
        private const string SaltKey = "s@lAvz10";
        private const string VIKey = "@1B2c3D4e5F6g7H8";
        private const int KeySize = 128;
        private readonly string _pathRoot;
        private readonly IConfiguration configuration;
        public HashService(IServiceProvider serviceProvider, IServiceScopeFactory factory)
        {
            var env = serviceProvider.GetService<IHostingEnvironment>();
            _pathRoot = $"{env.ContentRootPath}{Constantes.PathFinanciamientoTemplate}";
            configuration = factory.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
        }

        public  string Encriptar(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return default;

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            var keyBytes = new Rfc2898DeriveBytes(PasswordHash, System.Text.Encoding.ASCII.GetBytes(SaltKey)).GetBytes(KeySize / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, System.Text.Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }

                memoryStream.Close();
            }

            return Convert.ToBase64String(cipherTextBytes);
        }
        public  string Desencriptar(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return default;

            var cipherTextBytes = Convert.FromBase64String(encryptedText);
            var keyBytes = new Rfc2898DeriveBytes(PasswordHash, System.Text.Encoding.ASCII.GetBytes(SaltKey)).GetBytes(KeySize / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, System.Text.Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];

            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        public async Task EnviarCorreoAsync<T>(EmailData<T> obj, RecuperarClaveEmail message,string templateKey)
        {
            string smtp = configuration.GetSection("EmailSettings").GetSection("CorreoEnvio").Value;
            string key = configuration.GetSection("EmailSettings").GetSection("Key").Value;
            string ruta = "";
            ruta = $@"{_pathRoot}{obj.HtmlTemplateName}";
            string html = System.IO.File.ReadAllText(ruta);
            string body = Engine.Razor.RunCompile(html, $"{templateKey}", typeof(T), message);
            string correoDestino = string.Join(',', obj.EmailList);
            string correoSend = smtp;
            string clave = key;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
            mail.From = new System.Net.Mail.MailAddress(correoSend);
            mail.To.Add(correoDestino);

            mail.Subject = "Test";
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(correoSend, clave);
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }

        public async Task EnviarCorreoReporteAsync<T>(EmailData<T> obj, ListaReporte message, string templateKey)
        {

            string smtp = configuration.GetSection("EmailSettings").GetSection("CorreoEnvio").Value;
            string key = configuration.GetSection("EmailSettings").GetSection("Key").Value;
            string ruta = "";
            ruta = $@"{_pathRoot}{obj.HtmlTemplateName}";
            string html = System.IO.File.ReadAllText(ruta);
            string body = Engine.Razor.RunCompile(html, $"{templateKey}", typeof(T), message);
            string correoDestino = string.Join(',', obj.EmailList);
            string correoSend = smtp;
            string clave = key;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
            mail.From = new System.Net.Mail.MailAddress(correoSend);
            mail.To.Add(correoDestino);

            mail.Subject = "Test";
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(correoSend, clave);
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }

        public async Task<JwtResponse> ConstruirToken(Usuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("Id", credencialesUsuario.Id.ToString()),
                new Claim("userName", credencialesUsuario.UserName),
                new Claim("email",credencialesUsuario.correo),
                new Claim("rol",credencialesUsuario.RolId.ToString())
            };
         
            var llave = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("ASK9DASDASJD9ASJD9ASJDA9SJDAS9JDAS9JDA9SJD9ASJDAS9JDAS9DJAS9JDAS9DJAS9DJAS9DJAS9DAJS"));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddMinutes(120);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new JwtResponse()
            {
                AuthToken = new JwtSecurityTokenHandler().WriteToken(securityToken),
                ExpireIn = expiracion,
                Message=""
            };
        }
        public void log(string cadena)
        {
            var fileName = $@"Logs\Log-Davis-{DateTime.Now.ToString("ddMMyyyy")}.txt";
            
            FileStream fs = new FileStream(fileName, FileMode.Append);
            byte[] data = System.Text.Encoding.Default.GetBytes("\r\n" + cadena);

            fs.Write(data);
            fs.Close();

        }
        public  string retornarDireccion(string direccion)
        {
            string vientos = "";
            switch (direccion)
            {
                case "North": { vientos = "N"; break; }
                case "North-northeast": { vientos = "NNE"; break; }
                case "Northeast": vientos = "NE"; break;
                case "East-northeast": vientos = "ENE"; break;
                case "East": vientos = "E"; break;
                case "East-southeast": vientos = "ESE"; break;
                case "Southeast": vientos = "SE"; break;
                case "South-southeast": vientos = "SSE"; break;
                case "South": vientos = "S"; break;
                case "South-southwest": vientos = "SSW"; break;
                case "Southwest": vientos = "SW"; break;
                case "West-southwest": vientos = "WSW"; break;
                case "West": vientos = "W"; break;
                case "West-northwest": vientos = "WNW"; break;
                case "Northwest": vientos = "NW"; break;
                case "North-northwest": vientos = "NNW"; break;
            }
            return vientos;
        }

        public string conection()
        {
            return configuration.GetConnectionString("defaultConnection");
        }
    }
}
