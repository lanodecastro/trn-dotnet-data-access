using System;
using System.Collections.Generic;
using System.Text;

namespace TrnDotnetDataAccess.Entidades
{
    public class Cliente
    {
       
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Senha { get; private set; }
        public Cliente(string nome, string email, string senha)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;
            Senha = senha;
        }
        public Cliente(Guid id)
        {
            Id = id;
        }
        public void Atualizar(string nome,string email)
        {          
            Nome = nome;
            Email = email;
        }

    }
}
