﻿using SQLite;

namespace BibliotecaProyectoIntegrado.Models;
public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? NumeroSocio { get; set; }
    public string? Contrasena { get; set; }
}
