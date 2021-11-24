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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Custo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SqlConnection connection; //Conexão com o sql server
        private SqlCommand command;       // Execução de comando no sql server
        private SqlDataReader lerDados; //Lê os resultados de um comando do sql executado no sql server

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=custo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string query = string.Empty; //Usado para criar os comandos no sqls;

        private Login login;
        
        public MainWindow()
        {
            InitializeComponent();
        }
            

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUser.Text;
            string senha = txtPass.Password;            

            login = new Login();
            login.situacao = null;

            try
            {
                using (connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    query = @"SELECT situacao, senha, usuario FROM login WHERE usuario = '" + usuario +"'";

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

            if (login.situacao == null)
            {
                MessageBox.Show("Usuário não encontrado");                
            }
            else 
            {
                string verify = login.senha;

                if (BCrypt.Net.BCrypt.Verify(senha, verify))
                {
                    Sessao.SessaoUsuario = login.usuario;
                    Sessao.SessaoSituacao = login.situacao;

                    Main principal = new Main();
                    principal.Show();
                }
                else
                {
                    MessageBox.Show("Senha incorreta");
                }
            }

            this.Close();
        }
    }
}
