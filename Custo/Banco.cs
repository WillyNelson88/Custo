using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Custo
{
    class Banco
    {
        private SqlConnection connection; //Conexão com o sql server
        private SqlCommand command;       // Execução de comando no sql server
        private SqlDataReader lerDados; //Lê os resultados de um comando do sql executado no sql server

        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string query = string.Empty; //Usado para criar os comandos no sqls;

        private Login login;
        private Processo processo;
        private Cliente cliente;
        private Modelo modelo;
        private Login usuario;

        private List<Processo> listaProcessos;
        private List<Cliente> listaCliente;
        private List<Modelo> listaModelos;
        private List<Login> listaUsuarios;


        //Método para listar os usuários
        public Login Autenticar(string usuario)
        {

            login = new();
            login.situacao = null;

            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"SELECT situacao, senha, usuario FROM login WHERE usuario = '" + usuario + "'";

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        lerDados = command.ExecuteReader();

                        while (lerDados.Read())
                        {
                            login.situacao = lerDados.GetString(0);
                            login.senha = lerDados.GetString(1);
                            login.usuario = lerDados.GetString(2);
                        }
                    }
                }
            }

            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return login;
        }

        //Método para listar os processo da tabela operacao;
        public List<Processo> GetOperacoes(string where)
        {
            try
            {
                listaProcessos = new List<Processo>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    if (where != null)
                    {
                        query = "SELECT * FROM operacao WHERE operacao LIKE '%" + where + "%';";
                    }
                    else
                    {
                        query = "SELECT * FROM operacao;";
                    }

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                processo = new();

                                processo.Id = lerDados.GetInt32(0);
                                processo.Data = lerDados.GetString(1);
                                processo.Operacao = lerDados.GetString(2);
                                processo.Custo = lerDados.GetDecimal(3);
                                processo.Observacoes = lerDados.GetString(4);

                                listaProcessos.Add(processo);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return listaProcessos;
        }

        //Método para inserir uma operação na tabela operacao; 
        public void InsertOperacao(string operacao, string custo, string obs)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "INSERT INTO OPERACAO (DATA_CRIACAO, OPERACAO, CUSTO, OBSERVACOES)  VALUES (@DATA, @OPERACAO, @CUSTO, @OBS)";

                    string data = DateTime.Now.ToString("dd/MM/yyyy");
                    string preco = custo;
                    preco = preco.Replace(',', '.');

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DATA", data);
                    command.Parameters.AddWithValue("@OPERACAO", operacao);
                    command.Parameters.AddWithValue("@CUSTO", preco);
                    command.Parameters.AddWithValue("@OBS", obs);

                    connection.Open();

                    command.ExecuteNonQuery();

                    MessageBox.Show("Operação cadastrada com sucesso!");
                }

            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

        }

        //Método para excluir uma operação da tabela operacao;
        public void ExcluirOperacao(string id)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (MessageBox.Show("Tem certeza que desejas excluir a operação?", "Excluir operação", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        connection.ConnectionString = connectionString;
                        query = "DELETE FROM OPERACAO WHERE id = @ID";
                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Operacão excluída com sucesso!");
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        //Método para editar uma operação;
        public void EditarOperacao(string operacao, decimal preco, string observacoes, int id)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    query = "UPDATE OPERACAO SET operacao = @OPERACAO, custo = @CUSTO, observacoes = @OBSERVACOES WHERE id = @ID";

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OPERACAO", operacao);
                    command.Parameters.AddWithValue("@CUSTO", preco);
                    command.Parameters.AddWithValue("@OBSERVACOES", observacoes);
                    command.Parameters.AddWithValue("@ID", id);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Operação editada com sucesso!");
                }

            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

        }

        //Função para listar os clientes
        public List<Cliente> GetCliente()
        {
            try
            {
                listaCliente = new List<Cliente>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "SELECT * FROM cliente;";

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                cliente = new();

                                cliente.Id = lerDados.GetInt32(0);
                                cliente.Cli = lerDados.GetString(1);
                                cliente.Razao = lerDados.GetString(2);
                                cliente.Endereco = lerDados.GetString(3);

                                listaCliente.Add(cliente);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return listaCliente;
        }

        //Método para inserir um novo cliente na tabela cliente;
        public void InsertCliente(string cliente, string razao, string endereco)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "INSERT INTO cliente (cliente, razao, endereco)  VALUES (@CLI, @RAZAO, @ENDERECO);";

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CLI", cliente);
                    command.Parameters.AddWithValue("@RAZAO", razao);
                    command.Parameters.AddWithValue("@ENDERECO", endereco);

                    connection.Open();

                    command.ExecuteNonQuery();

                    MessageBox.Show("Cliente adicionado com sucesso!");

                }

            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        //Método para editar as informações de um cliente na tabela cliente;
        public void EditarCliente(string cliente, string razao, string endereco, int id)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    query = "UPDATE cliente SET  cliente = @CLIENTE, razao = @RAZAO, endereco = @ENDERECO WHERE id = @ID";

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CLIENTE", cliente);
                    command.Parameters.AddWithValue("@RAZAO", razao);
                    command.Parameters.AddWithValue("@ENDERECO", endereco);
                    command.Parameters.AddWithValue("@ID", id);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Cliente Editado com sucesso!");
                }

            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        //Método para editar as informações de um cliente
        public void ExcluiCliente(int id)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (MessageBox.Show("Tem certeza que desejas excluir o cliente?", "Excluir Cliente", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        connection.ConnectionString = connectionString;
                        query = "DELETE FROM cliente WHERE id = @ID";

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", id);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cliente excluído com sucesso!");
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        //Função para preencher a combobox cliente;
        public List<Cliente> CbCliente()
        {
            try
            {
                listaCliente = new List<Cliente>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "SELECT id, cliente FROM cliente;";

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                Cliente cli = new();

                                cli.Id = lerDados.GetInt32(0);
                                cli.Cli = lerDados.GetString(1);

                                listaCliente.Add(cli);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return listaCliente;
        }

        //Método para criar um modelo novo e gerar uma oc;
        public int CriarModelo(string referencia, string colecao, int idCliente, string descricao, decimal preco)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"INSERT INTO modelo (data_criacao, referencia, colecao, id_cliente, descricao, preco)
                              VALUES (@DATA, @REF, @COL, @CLI, @DESC, @PRECO) SELECT SCOPE_IDENTITY();";

                    string dia = DateTime.Now.ToString("dd/MM/yyyy");

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DATA", dia);
                    command.Parameters.AddWithValue("@COL", colecao);
                    command.Parameters.AddWithValue("@REF", referencia);
                    command.Parameters.AddWithValue("@CLI", idCliente);
                    command.Parameters.AddWithValue("@DESC", descricao);
                    command.Parameters.AddWithValue("@PRECO", preco);

                    connection.Open();

                    int oc = Convert.ToInt32(command.ExecuteScalar());
                    return oc;
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return 0;
        }

        //Método para salvar os dados da DataGrid montar na tabela montar_custo;
        public void MontaPreco(int oc, int idOperacao, int frequencia, string fase, decimal total)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"INSERT INTO montar_custo (oc, id_operacao, freq, fase, total)
                              VALUES (@OC, @ID_OPERACAO, @FREQ, @FASE, @TOTAL);";


                    command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@oc", oc);
                    command.Parameters.AddWithValue("@ID_OPERACAO", idOperacao);
                    command.Parameters.AddWithValue("@FREQ", frequencia);
                    command.Parameters.AddWithValue("@FASE", fase);
                    command.Parameters.AddWithValue("@TOTAL", total);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        //Função para pegar os modelos da tabela modelo;
        public List<Modelo> GetModelos(string where)
        {
            try
            {
                listaModelos = new List<Modelo>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    if (where != null)
                    {
                        query = @"SELECT mo.oc, cli.cliente, mo.referencia, mo.colecao, mo.descricao, mo.preco " +
                                "FROM modelo mo, cliente cli " +
                                "WHERE cli.id = mo.id_cliente " +
                                "AND mo.referencia LIKE '%" + where + "%' " +
                                "OR cli.cliente LIKE '%" + where + "%';";
                    }
                    else
                    {
                        query = "SELECT mo.oc, cli.cliente, mo.referencia, mo.colecao, mo.descricao, mo.preco FROM modelo mo, cliente cli WHERE cli.id = mo.id_cliente;";
                    }

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                modelo = new();

                                modelo.Oc = lerDados.GetInt32(0);
                                modelo.Cliente = lerDados.GetString(1);
                                modelo.Referencia = lerDados.GetString(2);
                                modelo.Colecao = lerDados.GetString(3);
                                modelo.Descricao = lerDados.GetString(4);
                                modelo.Preco = lerDados.GetDecimal(5);

                                listaModelos.Add(modelo);

                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return listaModelos;
        }

        //Metódo para buscao os DADOS do modelo selecionado no DataGridModelos;
        public Modelo Detalhes(int oc)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"SELECT mo.oc, mo.data_criacao, mo.referencia, mo.colecao, cli.cliente, mo.descricao
                              FROM modelo mo, cliente cli 
                              WHERE mo.id_cliente = cli.id
                              AND oc = " + oc;

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                modelo = new();

                                modelo.Oc = lerDados.GetInt32(0);
                                modelo.Data_criacao = lerDados.GetString(1);
                                modelo.Referencia = lerDados.GetString(2);
                                modelo.Colecao = lerDados.GetString(3);
                                modelo.Cliente = lerDados.GetString(4);
                                modelo.Descricao = lerDados.GetString(5);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return modelo;
        }

        //Metodo para pegar as OPERAÇÕES do modelo selecionado no DataGridModelos, agrupa por fase;
        public List<Processo> GetDetalhes(int oc)
        {
            try
            {
                listaProcessos = new List<Processo>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"SELECT fase, sum(total) as total FROM montar_custo WHERE oc = " + oc + "GROUP BY fase";


                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                processo = new();

                                processo.Fase = lerDados.GetString(0);
                                processo.Custo = lerDados.GetDecimal(1);

                                listaProcessos.Add(processo);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }


            return listaProcessos;
        }

        //Método para salvar os dados na tabela Cobrança
        public void IncluiCobranca(int talao, int oc, string tipo, decimal preco)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "INSERT INTO cobranca (talao, oc, dataCriacao, tipo, preco)  VALUES (@TALAO, @OC, @DATA, @TIPO, @PRECO)";

                    string data = DateTime.Now.ToString("dd/MM/yyyy");

                    command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@TALAO", talao);
                    command.Parameters.AddWithValue("@OC", oc);
                    command.Parameters.AddWithValue("@DATA", data);
                    command.Parameters.AddWithValue("@TIPO", tipo);
                    command.Parameters.AddWithValue("@PRECO", preco);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);

            }
        }

        //Lista os modelos com cobrança feita da tabela cobranca;
        public List<Modelo> GetCobranca(string col, string where)
        {
            try
            {
                listaModelos = new List<Modelo>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    if ((where != null) && (col != null))
                    {
                        query = @"SELECT mo.oc, co.talao, cli.cliente, mo.colecao, mo.referencia, mo.descricao, co.preco " +
                                "FROM modelo mo, cliente cli, cobranca co " +
                                "WHERE co.oc = mo.oc " +
                                "AND cli.id = mo.id_cliente " +
                                "AND " + col + " LIKE '%" + where + "%'";
                    }
                    else
                    {
                        query = @"SELECT mo.oc, co.talao, cli.cliente, mo.colecao, mo.referencia, mo.descricao, co.preco " +
                                "FROM modelo mo, cliente cli, cobranca co " +
                                "WHERE co.oc = mo.oc " +
                                "AND cli.id = mo.id_cliente ";
                    }

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                modelo = new();

                                modelo.Oc = lerDados.GetInt32(0);
                                modelo.Talao = lerDados.GetInt32(1);
                                modelo.Cliente = lerDados.GetString(2);
                                modelo.Colecao = lerDados.GetString(3);
                                modelo.Referencia = lerDados.GetString(4);
                                modelo.Descricao = lerDados.GetString(5);
                                modelo.Preco = lerDados.GetDecimal(6);

                                listaModelos.Add(modelo);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return listaModelos;
        }

        //Listar os processos da oc selecionada;
        public List<Processo> GetOperacaoModelo(int oc)
        {
            try
            {
                listaProcessos = new List<Processo>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"SELECT o.id, o.operacao, mc.freq, mc.total " +
                                    "FROM montar_custo mc, operacao o " +
                                    "WHERE mc.id_operacao = o.id " +
                                    "AND mc.oc = " + oc;
                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                processo = new();

                                processo.Id = lerDados.GetInt32(0);
                                processo.Operacao = lerDados.GetString(1);
                                processo.Freq = lerDados.GetInt32(2);
                                processo.Custo = lerDados.GetDecimal(3);

                                listaProcessos.Add(processo);
                            }
                        }

                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return listaProcessos;
        }

        public List<Login> GetUsuario()
        {
            try
            {
                listaUsuarios = new List<Login>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "SELECT id, usuario, situacao FROM login;";

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (lerDados = command.ExecuteReader())
                        {
                            while (lerDados.Read())
                            {
                                usuario = new();

                                usuario.id = lerDados.GetInt32(0);
                                usuario.usuario = lerDados.GetString(1);
                                usuario.situacao = lerDados.GetString(2);

                                listaUsuarios.Add(usuario);
                            }
                        }
                    }
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }

            return listaUsuarios;
        }

        //Método para adicionar um usuário na tabela login;
        public void AdicionaUsuario(string usuario, string senha, string situacao)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "INSERT INTO login (usuario, senha, situacao)  VALUES (@USER, @PASSWD, @SIT)";

                    string pass = BCrypt.Net.BCrypt.HashPassword(senha);

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@USER", usuario);
                    command.Parameters.AddWithValue("@PASSWD", pass);
                    command.Parameters.AddWithValue("@SIT", situacao);

                    connection.Open();

                    command.ExecuteNonQuery();

                    MessageBox.Show("Usuário adicionado com sucesso!");
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
       }

        //Método para editar um usuário da tabela login;
        public void EditarUsuario(string usuario, string senha, string situacao, int id)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    query = "UPDATE login SET usuario = @USER, senha = @SENHA, situacao = @SIT WHERE id = @ID";
                    
                    string pass = BCrypt.Net.BCrypt.HashPassword(senha);

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@USER", usuario);
                    command.Parameters.AddWithValue("@SENHA", pass);
                    command.Parameters.AddWithValue("@SIT", situacao);
                    command.Parameters.AddWithValue("@ID", id);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                    
                    MessageBox.Show("Usuário editado com sucesso!");
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        //Método para excluir um usuário da tabela login;
        public void ExcluiUsuario(int id)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (MessageBox.Show("Tem certeza que desejas excluir o usuário?", "Excluir Usuário", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        connection.ConnectionString = connectionString;
                        query = "DELETE FROM login WHERE id = @ID";

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", id);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuário Excluído com sucesso!");
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }  
    }
}
