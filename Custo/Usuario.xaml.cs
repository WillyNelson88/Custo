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
    /// Lógica interna para Usuario.xaml
    /// </summary>
    public partial class Usuario : Window
    {

        //ADO.NET
        public SqlConnection connection; //Conexão com o sql server
        public SqlCommand command;       // Execução de comando no sql server
        public SqlDataReader dataReader; //Lê os resultados de um comando do sql executado no sql server

        public string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public string query = string.Empty; //Usado para criar os comandos no sql


        private List<Login> listaUsuario;
        private Login usuario;

        public Usuario()
        {
            InitializeComponent();
            readFromDatabase();
        }

        private void readFromDatabase()
        {
            try
            {
                txtSenha.Clear();
                txtUsuario.Clear();

                listaUsuario = new List<Login>();

                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = "SELECT id, usuario, situacao FROM login;";

                    using (command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                usuario = new Login();

                                usuario.id = dataReader.GetInt32(0);
                                usuario.usuario = dataReader.GetString(1);
                                usuario.situacao = dataReader.GetString(2);

                                listaUsuario.Add(usuario);
                            }
                        }
                    }
                }
                DataGridUsuarios.ItemsSource = listaUsuario;
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
            }
        }

        private void cbSituacao_Loaded(object sender, RoutedEventArgs e)
        {
            cbSituacao.Items.Add("USER");
            cbSituacao.Items.Add("ADMIN");
        }

        //Inserir operações na tabela operações
        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtSenha.Text) || string.IsNullOrWhiteSpace(cbSituacao.Text))
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

                        query = "INSERT INTO login (usuario, senha, situacao)  VALUES (@USER, @PASSWD, @SIT)";

                        string pass = BCrypt.Net.BCrypt.HashPassword(txtSenha.Text);                        

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@USER", txtUsuario.Text);
                        command.Parameters.AddWithValue("@PASSWD", pass);
                        command.Parameters.AddWithValue("@SIT", cbSituacao.Text);

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
        private void DataGridOperacoes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DataGridUsuarios.SelectedItem != null)
            {
                var login = (Login)DataGridUsuarios.SelectedItem;

                txtUsuario.Text = login.usuario;
                cbSituacao.SelectedItem = login.situacao;
            }
        }

        //Excluir dados da linha selecionada
        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (DataGridUsuarios.SelectedItem != null)
                    {
                        var login = (Login)DataGridUsuarios.SelectedItem;
                        int id = login.id;

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

        //Editar dados da linha selecionada
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (connection = new SqlConnection())
                {
                    if (DataGridUsuarios.SelectedItem != null)
                    {
                        var login = (Login)DataGridUsuarios.SelectedItem;

                        connection.ConnectionString = connectionString;
                        query = "UPDATE login SET usuario = @USER, senha = @SENHA, situacao = @SIT WHERE id = @ID";

                        string senha = BCrypt.Net.BCrypt.HashPassword(txtSenha.Text);

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@USER", txtUsuario.Text);
                        command.Parameters.AddWithValue("@SENHA", senha);
                        command.Parameters.AddWithValue("@SIT", cbSituacao.Text);
                        command.Parameters.AddWithValue("@ID", login.id);

                        connection.Open();
                        command.ExecuteNonQuery();

                        MessageBox.Show("Usuário editado com sucesso!");
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
