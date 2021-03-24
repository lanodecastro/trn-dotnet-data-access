using System;
using System.Collections.Generic;
using System.Text;

namespace TrnDotnetDataAccess.Entidades
{
    public class Pedido
    {
        public Guid Id { get; private set; }
        public DateTime Data { get; private set; }
        public Cliente Cliente { get; private set; }
        public IList<ItemPedido> Itens { get; private set; }

        public Pedido(Cliente cliente)
        {
            Id = Guid.NewGuid();
            Data = DateTime.Now;
            Cliente = cliente;
        }
    }
}
