using System;
using System.Collections.Generic;
using System.Text;

namespace TrnDotnetDataAccess.Entidades
{
    public class ItemPedido
    {
      
        public Pedido Pedido { get; private set; }
        public Produto Produto { get; private set; }
        public int Quantidade { get; private set; }

        public ItemPedido(Pedido pedido, Produto produto, int quantidade)
        {
            Pedido = pedido;
            Produto = produto;
            Quantidade = quantidade;
        }

    }
}
