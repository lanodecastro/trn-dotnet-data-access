using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TrnDotnetDataAccess.Entidades;

namespace TrnDotnetDataAccess
{
    class Program
    {
        private static SqlConnection sqlConnection;
        static void Main(string[] args)
        {

            CriarPedido();
            Console.ReadKey();
        }
        private static void IniciarConexao()
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dbLoja;Integrated Security=True;Connect Timeout=30;";

            sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
           
        }
        private static void GravarNovoCliente()
        {
            IniciarConexao();
            sqlConnection.Open();

            var sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "insert into Cliente values(@id,@nome,@email,@senha)";

            var cliente = new Cliente("Maria da Silva", "marias274@gmail.com", "123456");

            sqlCommand.Parameters.Add(new SqlParameter("@id",cliente.Id));
            sqlCommand.Parameters.Add(new SqlParameter("@nome", cliente.Nome));
            sqlCommand.Parameters.Add(new SqlParameter("@email", cliente.Email));
            sqlCommand.Parameters.Add(new SqlParameter("@senha", cliente.Senha));

            var qtdRows=sqlCommand.ExecuteNonQuery();

            if (qtdRows > 0)
            {
                Console.WriteLine("Cliente cadastrado com sucesso");
            }

            sqlConnection.Close();
            sqlConnection.Dispose();
        }
        private static void ExcluirCliente()
        {
            IniciarConexao();
            sqlConnection.Open();
            var sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "delete from Cliente where id=@id";

            var clienteId = "B1A7280E-D71D-4169-97F3-083ED04C0ACB";
            sqlCommand.Parameters.Add(new SqlParameter("@id", clienteId));

            var qtdRows = sqlCommand.ExecuteNonQuery();

            if (qtdRows > 0)
            {
                Console.WriteLine("Cliente excluído com sucesso");
            }

            sqlConnection.Close();
            sqlConnection.Dispose();


        }
        private static void ListarClientes()
        {
            IniciarConexao();
            sqlConnection.Open();
            var sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "select Id,Nome,Email from Cliente";

            var sqlDataReader = sqlCommand.ExecuteReader();

            List<Cliente> listaClientes = new List<Cliente>();

            while (sqlDataReader.Read())
            {
                Guid id = Guid.Parse(sqlDataReader[0].ToString());
                var cliente = new Cliente(id);
                cliente.Atualizar(sqlDataReader[1].ToString(), sqlDataReader[2].ToString());
                listaClientes.Add(cliente);
            }

            sqlDataReader.Close();
            sqlConnection.Close();
            sqlConnection.Dispose();

            foreach (var item in listaClientes)
            {
                Console.WriteLine($"Nome: {item.Nome}  - Email: {item.Email}");
            }


        }
        private static void CriarPedido()
        {
            var cliente = RecuperarCliente(Guid.Parse("65BB0F01-D5F5-4470-876E-CDCA600D656B"));
            var produto = RecuperarProduto(Guid.Parse("18BAA6A7-6983-488A-9441-A55727AD5BAA"));

            IniciarConexao();

            using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    Console.WriteLine("Iniciou a transação");
                    var sqlCommandPedido = new SqlCommand();
                    sqlCommandPedido.Connection = sqlConnection;
                    sqlCommandPedido.Transaction = sqlTransaction;
                    sqlCommandPedido.CommandText = "Insert into Pedido (Id,Data,ClienteID) values(@ID,@Data,@ClienteID)";

                    var pedido = new Pedido(cliente);
                    sqlCommandPedido.Parameters.Add(new SqlParameter("@Id", pedido.Id));
                    sqlCommandPedido.Parameters.Add(new SqlParameter("@Data", pedido.Data));
                    sqlCommandPedido.Parameters.Add(new SqlParameter("@ClienteID", pedido.Cliente.Id));

                    sqlCommandPedido.ExecuteNonQuery();


                    var sqlCommandItemPedido = new SqlCommand();
                    sqlCommandItemPedido.Connection = sqlConnection;
                    sqlCommandItemPedido.Transaction = sqlTransaction;
                    sqlCommandItemPedido.CommandText = "Insert into ItemPedido (PedidoID,ProdutoID,Quantidade) values(@PedidoID,@ProdutoID,@Quantidade)";

                    var itemPedido = new ItemPedido(pedido, produto, 10);
                    sqlCommandItemPedido.Parameters.Add(new SqlParameter("@PedidoId", itemPedido.Pedido.Id));
                    sqlCommandItemPedido.Parameters.Add(new SqlParameter("@ProdutoID", itemPedido.Produto.Id));
                    sqlCommandItemPedido.Parameters.Add(new SqlParameter("@Quantidade", itemPedido.Quantidade));

                    sqlCommandItemPedido.ExecuteNonQuery();

                    sqlTransaction.Commit();
                    Console.WriteLine("Transação executada com sucesso");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    sqlTransaction.Rollback();
                    Console.WriteLine("Transação possui erros e foi desfeita");
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }           

          
        }
        private static Cliente RecuperarCliente(Guid clienteId)
        {
            IniciarConexao();
            
            var sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "select Id,Nome,Email from Cliente where Id=@id";

            sqlCommand.Parameters.Add(new SqlParameter("@id", clienteId));
            var sqlDataReader = sqlCommand.ExecuteReader();

            Cliente cliente = null;

            if (sqlDataReader.HasRows)
            {             
                while (sqlDataReader.Read())
                {
                    Guid id = Guid.Parse(sqlDataReader[0].ToString());
                    cliente = new Cliente(id);
                    cliente.Atualizar(sqlDataReader[1].ToString(), sqlDataReader[2].ToString());
                   
                }
                
            }

            sqlDataReader.Close();
            sqlConnection.Close();
            sqlConnection.Dispose();

            return cliente;



        }
        private static Produto RecuperarProduto(Guid produtoId)
        {
            IniciarConexao();
            
            var sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "select Id,Nome,PrecoUnitario,QuantidadeEstoque from Produto where Id=@id";

            sqlCommand.Parameters.Add(new SqlParameter("@id", produtoId));
            var sqlDataReader = sqlCommand.ExecuteReader();

            Produto produto = null;

            if (sqlDataReader.HasRows)
            {
                while (sqlDataReader.Read())
                {
                    Guid id = Guid.Parse(sqlDataReader[0].ToString());
                    produto = new Produto(id);
                    produto.Atualizar(sqlDataReader[1].ToString(), Convert.ToDecimal(sqlDataReader[2]),Convert.ToInt32(sqlDataReader[3]));

                }

            }

            sqlDataReader.Close();
            sqlConnection.Close();
            sqlConnection.Dispose();

            return produto;



        }
    }
}
