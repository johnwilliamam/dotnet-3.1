using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace OpenTelemetryDbApp.Controllers
{
    // Controller para manipulação de dados no banco
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly string _connectionString = "Server=localhost;Database=testdb;User=root;Password=root;";

        // Modelo para receber o body da requisição
        public class UserRequest
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        // Rota para inserir dados no banco
        [HttpPost("insert")]
        public IActionResult InsertData([FromBody] UserRequest user)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Comando SQL para inserir dados
                    var sql = "INSERT INTO users (name, email) VALUES (@name, @email);";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@email", user.Email);

                        int rowsAffected = command.ExecuteNonQuery();
                        return Ok(new { message = "Data inserted successfully", rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error inserting data", error = ex.Message });
            }
        }

        // Rota para consultar todos os dados
        [HttpGet("get-all")]
        public IActionResult GetAllData()
        {
            try
            {
                var users = new List<UserRequest>();

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Comando SQL para buscar todos os registros
                    var sql = "SELECT name, email FROM users;";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserRequest
                                {
                                    Name = reader["name"].ToString(),
                                    Email = reader["email"].ToString()
                                });
                            }
                        }
                    }
                }

                return Ok(new { message = "Data retrieved successfully", users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving data", error = ex.Message });
            }
        }
    }
}
