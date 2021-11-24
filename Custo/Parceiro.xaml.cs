using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Custo
{
    /// <summary>
    /// Lógica interna para Parceiroxaml.xaml
    /// </summary>
    public partial class Parceiro : Window
    {

        //ADO.NET
        public SqlConnection connection; //Conexão com o sql server
        public SqlCommand command;       // Execução de comando no sql server
        public SqlDataReader dataReader; //Lê os resultados de um comando do sql executado no sql server

        public string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public string query = string.Empty; //Usado para criar os comandos no sql


        private List<Cliente> listaCliente;
        private Cliente cliente;

        public Parceiro()
        {
            InitializeComponent();
            readFromDatabase();
        }

        //Ler dados da tabela e exibir na grid
        private void readFromDatabase()
        {
            try
            {
                txtCli.Clear();
                txtRazao.Clear();
                txtEndereco.Clear();

                listaCliente = new List<Cliente>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "SELECT * FROM cliente;";

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                cliente = new Cliente();

                                cliente.id = dataReader.GetInt32(0);
                                cliente.Cli = dataReader.GetString(1);
                                cliente.Razao = dataReader.GetString(2);
                                cliente.Endereco = dataReader.GetString(3);

                                listaCliente.Add(cliente);
                            }
                        }
                    }
                }
                DataGridCliente.ItemsSource = listaCliente;
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        //Inserir operações na tabela operações
        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCli.Text) || string.IsNullOrWhiteSpace(txtRazao.Text) || string.IsNullOrWhiteSpace(txtEndereco.Text))
            {
                MessageBox.Show("Não é possível inserir dados vazios");
            }
            else
            {
                try
                {
                    using (connection = new SqlConnection())
                    {
                        connection.ConnectionString = connectionString;

                        query = "INSERT INTO cliente (cliente, razao, endereco)  VALUES (@CLI, @RAZAO, @ENDERECO);";

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@CLI", txtCli.Text);
                        command.Parameters.AddWithValue("@RAZAO", txtRazao.Text);
                        command.Parameters.AddWithValue("@ENDERECO", txtEndereco.Text);

                        connection.Open();

                        command.ExecuteNonQuery();

                    }

                }
                catch (SqlException se)
                {
                    MessageBox.Show(se.Message);
                }
                finally
                {
                    connection.Close();
                    connection = null;
                    command = null;

                    readFromDatabase();

                }
            }            
        }

        //Pegar dados da linha selecionada no dataGrid
        private void DataGridCliente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DataGridCliente.SelectedItem != null)
            {

                var cliente = (Cliente)DataGridCliente.SelectedItem;

                txtCli.Text = cliente.Cli;
                txtRazao.Text = cliente.Razao;
                txtEndereco.Text = cliente.Endereco;
            }
        }

        //Excluir dados da linha selecionada
        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (DataGridCliente.SelectedItem != null)
                    {
                        var cliente = (Cliente)DataGridCliente.SelectedItem;
                        string id = cliente.id.ToString();

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

                }

            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
            finally
            {
                connection.Close();
                connection = null;
                command = null;

                readFromDatabase();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (DataGridCliente.SelectedItem != null)
                    {
                        var cliente = (Cliente)DataGridCliente.SelectedItem;                        
                        
                        connection.ConnectionString = connectionString;
                        query = "UPDATE cliente SET  cliente = @CLIENTE, razao = @RAZAO, endereco = @ENDERECO WHERE id = @ID";

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@CLIENTE", txtCli.Text);
                        command.Parameters.AddWithValue("@RAZAO", txtRazao.Text);
                        command.Parameters.AddWithValue("@ENDERECO",txtEndereco.Text);
                        command.Parameters.AddWithValue("@ID", cliente.id);

                        connection.Open();
                        command.ExecuteNonQuery();

                        MessageBox.Show("Cliente Editado com sucesso!");
                        
                    }

                }

            }
            catch (SqlException se)
            {
                MessageBox.Show(se.Message);
            }
            finally
            {
                connection.Close();
                connection = null;
                command = null;

                readFromDatabase();

            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Sessao.JanelaUsuario = false;
        }
    }

}
